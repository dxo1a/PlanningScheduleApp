using MathCore.WPF;
using MathCore.WPF.Shaders;
using Microsoft.Office.Interop.Excel;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using PlanningScheduleApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    public partial class ExportToExcelFilterWindow : System.Windows.Window
    {
        DateTime StartDate, FinishDate;

        List<StaffModel> AllStaffList = new List<StaffModel>();
        List<StaffModel> TotalHoursList = new List<StaffModel>();
        List<SZAndScheduleModel> SZAndScheduleList = new List<SZAndScheduleModel>();

        List<StaffModel> staffModels = new List<StaffModel>();

        private LoadingWindow loadingWindow;

        //double AcceptableFreeHoursFreeHours;
        double FreeHours;

        #region Переменные для Bitrix24

        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string filePath, saveFileName;
        string unloadingDate, unloadingTime, webhook;

        int selectedChat, selectedFolder, chatType;
        string selectedMessageText, selectedUrlText, urlPreview;
        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";

        #endregion

        public ExportToExcelFilterWindow()
        {
            InitializeComponent();
            SettingsManager settingsManager = new SettingsManager();
            AppSettings settings = settingsManager.LoadSettings();

            webhook = settings.webhook;
            chatType = settings.chatType;
            selectedChat = settings.chatid;                       // ID диалога (если чат, то заменить DIALOG_ID на CHAT_ID в SendMessageToChatWebhook.messageData)
            selectedFolder = settings.folderid;                  // ID папки (личная 305001, сменные 305175)
            selectedMessageText = "Загруженность сменных заданий";
            selectedUrlText = "Таблица со сменными заданиями";
            urlPreview = "https://bitrix24public.com/steklm.bitrix24.ru/docs/pub/84ca3be9cf47ecddbe03382923800783/showPreview/?&token=51i2y7x57cuw";

            StartDP.SelectedDate = DateTime.Today;
        }

        private async void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StartDP.SelectedDate.HasValue && FinishDP.SelectedDate.HasValue)
            {
                StartDate = StartDP.SelectedDate ?? DateTime.MinValue;
                FinishDate = FinishDP.SelectedDate ?? DateTime.MaxValue;

                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            int ResultDate = Convert.ToInt32((FinishDate - StartDate).TotalDays) + 1;
            if (ResultDate >= 8)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены что хотите выгрузить данные за {ResultDate} дней?\n(Это может занять много времени)", "Выгрузка в Excel", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    loadingWindow = new LoadingWindow("выгрузка в Excel", this);
                    loadingWindow.Show();

                    await ExportToExcel();

                    //loadingWindow.CanClose = true;
                    loadingWindow.ChangeText("Запрос выполнен.\n\nВы можете открыть папку с файлом или отправить его в чат.", true, filePath, saveFileName, selectedMessageText, selectedUrlText, webhook, urlPreview, selectedFolder, selectedChat, chatType);
                }
            }
            else
            {
                loadingWindow = new LoadingWindow("выгрузка в Excel", this);
                loadingWindow.Show();

                await ExportToExcel();

                //loadingWindow.CanClose = true;
                loadingWindow.ChangeText("Запрос выполнен.\n\nВы можете открыть папку с файлом или отправить его в чат.", true, filePath, saveFileName, selectedMessageText, selectedUrlText, webhook, urlPreview, selectedFolder, selectedChat, chatType);
            }
        }

        private void StartDP_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FinishDP.SelectedDate == null)
                FinishDP.SelectedDate = StartDP.SelectedDate;
            staffModels = new List<StaffModel>();
            if (StartDP.SelectedDate.HasValue && FinishDP.SelectedDate.HasValue)
            {
                StartDate = StartDP.SelectedDate ?? DateTime.MinValue;
                FinishDate = FinishDP.SelectedDate ?? DateTime.MaxValue;

                for (DateTime date = StartDate; date <= FinishDate; date = date.AddDays(1))
                {
                    staffModels.Add(new StaffModel { DTA = date, AcceptableFreeHours = 0 }); // Устанавливаем начальное значение для AcceptableFreeHours
                }
            }
            FreeHoursDataGrid.ItemsSource = staffModels;
        }

        private void FinishDP_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FinishDP.SelectedDate < StartDP.SelectedDate)
            {
                FinishDP.SelectedDate = StartDP.SelectedDate;
                MessageBox.Show("Конечная дата не может быть раньше начальной!", "Дата", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                staffModels = new List<StaffModel>();
                if (StartDP.SelectedDate.HasValue && FinishDP.SelectedDate.HasValue)
                {
                    StartDate = StartDP.SelectedDate ?? DateTime.MinValue;
                    FinishDate = FinishDP.SelectedDate ?? DateTime.MaxValue;

                    for (DateTime date = StartDate; date <= FinishDate; date = date.AddDays(1))
                    {
                        staffModels.Add(new StaffModel { DTA = date, AcceptableFreeHours = 0 }); // Устанавливаем начальное значение для AcceptableFreeHours
                    }
                }
                FreeHoursDataGrid.ItemsSource = staffModels;
            }
        }

        int currentCounter;
        public int processCounter;
        public event Action<string, int, int> ProgressChanged;
        private async Task ExportToExcel()
        {
            processCounter = 0;
            await Task.Run(async () =>
            {
                AllStaffList = Odb.db.Database.SqlQuery<StaffModel>($"SELECT DISTINCT b.FIO AS SHORT_FIO, LTRIM(b.Tabel) AS TABEL_ID, CAST((DATEDIFF(MINUTE, WorkBegin, WorkEnd) - ISNULL(CASE WHEN LunchTimeBegin IS NOT NULL AND LunchTimeEnd IS NOT NULL THEN DATEDIFF(MINUTE, LunchTimeBegin, LunchTimeEnd) ELSE 0 END, 0)) / 60.0 AS FLOAT) AS WorkingHours, a.DTA, a.STAFF_ID, ID_Schedule, b.Position FROM Zarplats.dbo.Staff_Schedule AS a LEFT JOIN Zarplats.dbo.StaffView AS b ON a.STAFF_ID = b.STAFF_ID and isActual = 1 WHERE b.VALID = 1 and a.DTA between @startDate and @endDate ORDER BY b.Position", new SqlParameter("startDate", StartDate), new SqlParameter("endDate", FinishDate)).ToList();
                #region Вычисление свободных часов
                Dictionary<(int, DateTime), double> freeHoursDictionary = new Dictionary<(int, DateTime), double>();
                foreach (var staff in AllStaffList)
                {
                    double totalHours = await GetTotalHoursAsync(staff.STAFF_ID, staff.DTA);
                    double workingHours = await GetWorkingHoursAsync(staff.STAFF_ID, staff.DTA);

                    if (totalHours < workingHours && totalHours > 0)
                    {
                        FreeHours = workingHours - totalHours;
                        freeHoursDictionary[(staff.STAFF_ID, staff.DTA)] = FreeHours;
                        staff.FreeHours = FreeHours;
                        staff.TotalHours = totalHours;
                        //Console.WriteLine($"Условие [totalHours < workingHours] для {staff.SHORT_FIO} сработало.");
                    }
                    processCounter++;
                    ProgressChanged?.Invoke("Подготовка...", processCounter, AllStaffList.Count + 1);
                }
                #endregion

                // "Свободные часы > 0"
                List<StaffModel> staffWithPositiveFreeHours = AllStaffList.Where(s => s.FreeHours > 0).ToList();

                processCounter++;
                ProgressChanged?.Invoke("Подготовка...", processCounter, AllStaffList.Count + 1);
                Dictionary<int, List<SZAndScheduleModel>> szandschedulesDataDictionary = new Dictionary<int, List<SZAndScheduleModel>>();

                processCounter = 0;
                List<Company> companies = new List<Company>();

                foreach(StaffModel staff in staffWithPositiveFreeHours)
                {
                    double totalHours = await GetTotalHoursAsync(staff.STAFF_ID, staff.DTA);
                    double workingHours = await GetWorkingHoursAsync(staff.STAFF_ID, staff.DTA);

                    var dataGridRow = FreeHoursDataGrid.Items.Cast<StaffModel>().FirstOrDefault(item => item.DTA == staff.DTA);

                    if (dataGridRow != null)
                    {
                        staff.AcceptableFreeHours = dataGridRow.AcceptableFreeHours;
                    }

                    // Проверяем условия и добавляем сотрудника в отдел
                    if (totalHours < workingHours && totalHours > 0 && staff.FreeHours > staff.AcceptableFreeHours)
                    {
                        #region Общая загруженность
                        // Определение компании и отдела для каждого сотрудника
                        string companyName = ExtractCompanyName(staff.Position);
                        string departmentName = ExtractDepartmentName(staff.Position);

                        // Найти или создать компанию
                        Company company = companies.FirstOrDefault(c => c.Name == companyName);
                        if (company == null)
                        {
                            company = new Company { Name = companyName };
                            companies.Add(company);
                        }

                        // Найти или создать отдел в компании
                        Department department = company.Departments.FirstOrDefault(d => d.Name == departmentName);
                        if (department == null)
                        {
                            department = new Department { Name = departmentName };
                            company.Departments.Add(department);
                        }

                        // Добавить сотрудника в отдел
                        department.StaffMembers.Add(staff);
                        #endregion

                        #region Сменные задания
                        SZAndScheduleList = await GetSZAndScheduleAsync(staff.STAFF_ID, StartDate, FinishDate);
                        szandschedulesDataDictionary[staff.STAFF_ID] = SZAndScheduleList;
                        #endregion
                    }

                    processCounter++;
                    ProgressChanged?.Invoke("Выгрузка...", processCounter, staffWithPositiveFreeHours.Count);
                };

                #region Excel
                Excel.Application excelApp = new Excel.Application();
                excelApp.DisplayAlerts = true;
                excelApp.Visible = true;

                Workbook workbook = excelApp.Workbooks.Add();

                #region Сменные задания
                Worksheet worksheet2 = (Excel.Worksheet)workbook.Worksheets[1];
                worksheet2.Name = "Сменные задания";


                Excel.Range headerRange2 = worksheet2.get_Range("A1", "I1");
                headerRange2.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.Orange);
                headerRange2.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                headerRange2.Borders.LineStyle = XlLineStyle.xlContinuous;

                worksheet2.Cells[1, 1] = "Таб. ном.";
                worksheet2.Cells[1, 2] = "ФИО";
                worksheet2.Cells[1, 3] = "Изделие";
                worksheet2.Cells[1, 4] = "ПП";
                worksheet2.Cells[1, 5] = "ПрП";
                worksheet2.Cells[1, 6] = "Деталь";
                worksheet2.Cells[1, 7] = "Количество";
                worksheet2.Cells[1, 8] = "Время";
                worksheet2.Cells[1, 9] = "Дата";

                int rowCountSZ = 2;

                bool isAnotherInterior = true;
                foreach (var staffData in szandschedulesDataDictionary)
                {
                    int staffId = staffData.Key;
                    List<SZAndScheduleModel> szAndScheduleList = staffData.Value;
                    szAndScheduleList = szAndScheduleList.OrderBy(sz => sz.FIO).ThenBy(sz => sz.DTA).ToList();
                    
                    foreach (var sz in szAndScheduleList)
                    {
                        if (sz.TotalHours.HasValue)
                        {
                            double totalHoursValue = sz.TotalHours.Value;
                            double workingHoursValue = sz.WorkingHours.Value;
                            double freeHours = freeHoursDictionary.ContainsKey((staffId, sz.DTA)) ? freeHoursDictionary[(staffId, sz.DTA)] : 0;
                            sz.FreeHours = freeHours;
                            if (sz.Product != null && sz.Detail != null && sz.NUM != null && sz.Cost != 0 && sz.Count != 0)
                            {
                                worksheet2.Cells[rowCountSZ, 1] = sz.TABEL_ID;
                                worksheet2.Cells[rowCountSZ, 2] = sz.FIO;
                                worksheet2.Cells[rowCountSZ, 3] = sz.Product;
                                worksheet2.Cells[rowCountSZ, 4].NumberFormat = "@";
                                worksheet2.Cells[rowCountSZ, 4] = sz.PP.ToString();
                                worksheet2.Cells[rowCountSZ, 5] = sz.NUM;
                                worksheet2.Cells[rowCountSZ, 6] = sz.Detail;
                                worksheet2.Cells[rowCountSZ, 7] = sz.Count;
                                worksheet2.Cells[rowCountSZ, 8] = sz.Cost;
                                worksheet2.Cells[rowCountSZ, 9] = sz.DTA.ToShortDateString();

                                Range range = worksheet2.Range[worksheet2.Cells[rowCountSZ, 1], worksheet2.Cells[rowCountSZ, 9]];

                                if (isAnotherInterior)
                                {
                                    range.Interior.Color = Color.White;
                                }
                                else
                                {
                                    range.Interior.Color = Color.FromArgb(240, 240, 240).ToArgb();
                                }
                                range.Borders.LineStyle = XlLineStyle.xlContinuous;
                                range.Borders.Color = Color.FromArgb(208, 215, 229).ToArgb();

                                rowCountSZ++;
                            }
                        }
                    }
                    isAnotherInterior = !isAnotherInterior;
                }
                worksheet2.Columns.AutoFit();
                #endregion

                #region Общая загруженность
                Worksheet worksheet3 = (Worksheet)workbook.Worksheets.Add();
                worksheet3.Name = "Общая загруженность";

                worksheet3.Cells[1, 1] = "Таб. ном.";
                worksheet3.Cells[1, 2] = "Сотрудник";
                worksheet3.Cells[1, 3] = "Дата";
                worksheet3.Cells[1, 4] = "Общие часы";
                worksheet3.Cells[1, 5] = "Свободные часы";

                Range headerRange3 = worksheet3.get_Range("A1", "E1");
                headerRange3.Interior.Color = ColorTranslator.ToOle(Color.Orange);
                headerRange3.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                headerRange3.Borders.LineStyle = XlLineStyle.xlContinuous;

                int rowCount3 = 2;
                foreach (Company company in companies)
                {
                    worksheet3.Cells[rowCount3, 2] = company.Name;
                    Range companyRange = worksheet3.get_Range($"A{rowCount3}", $"E{rowCount3}");
                    companyRange.Interior.Color = ColorTranslator.ToOle(ColorTranslator.FromHtml("#69c5c2"));
                    companyRange.Font.Size += 2.5;
                    companyRange.Font.Bold = true;
                    companyRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                    companyRange.Merge();
                    rowCount3++;

                    // Итерирование по отделам в компании
                    foreach (Department department in company.Departments)
                    {
                        if (department.Name != String.Empty)
                        {
                            worksheet3.Cells[rowCount3, 2] = department.Name;
                            Range departmentRange = worksheet3.get_Range($"A{rowCount3}", $"E{rowCount3}");
                            departmentRange.Interior.Color = ColorTranslator.ToOle(ColorTranslator.FromHtml("#73a2a0"));
                            departmentRange.Font.Size += 1;
                            departmentRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                            rowCount3++;
                        }

                        var sortedStaffMembers = department.StaffMembers.OrderBy(staff => staff.SHORT_FIO).ThenBy(staff => staff.DTA).ToList();
                        // Итерирование по сотрудникам в отделе
                        foreach (StaffModel staff in sortedStaffMembers)
                        {
                            Range rowRange = worksheet3.Range[worksheet3.Cells[rowCount3, 1], worksheet3.Cells[rowCount3, 5]];
                            rowRange.Borders.LineStyle = XlLineStyle.xlContinuous;

                            worksheet3.Cells[rowCount3, 1] = staff.TABEL_ID;
                            worksheet3.Cells[rowCount3, 2] = staff.SHORT_FIO;
                            worksheet3.Cells[rowCount3, 3] = staff.DTA.ToShortDateString();
                            worksheet3.Cells[rowCount3, 4] = staff.TotalHours.HasValue ? Math.Round(staff.TotalHours.Value, 2) : 0;
                            worksheet3.Cells[rowCount3, 5] = freeHoursDictionary.ContainsKey((staff.STAFF_ID, staff.DTA)) ? Math.Round(freeHoursDictionary[(staff.STAFF_ID, staff.DTA)], 2) : 0;

                            rowCount3++;
                        }
                    }
                }
                worksheet3.Columns.AutoFit();
                #endregion

                workbook.Title = $"Загруженность сотрудников";

                // Ставится дата и время на момент сохранения документа
                unloadingDate = DateTime.Now.ToShortDateString();
                unloadingTime = DateTime.Now.ToString("HH_mm");

                string folderPath = $"{documentsPath}\\График сотрудников";
                saveFileName = $"Общая загруженность сотрудников {StartDate.ToShortDateString()}-{FinishDate.ToShortDateString()} ({unloadingDate} {unloadingTime}).xlsx";
                filePath = $"{documentsPath}\\График сотрудников\\{saveFileName}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                workbook.SaveAs(filePath);


                Marshal.ReleaseComObject(worksheet3);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelApp);
            });
            #endregion
        }

        private string ExtractCompanyName(string position)
        {
            int hyphenIndex = position.IndexOf(" - ");
            if (hyphenIndex != -1)
            {
                return position.Substring(0, hyphenIndex).Trim();
            }

            return position.Trim();
        }

        private string ExtractDepartmentName(string position)
        {
            int hyphenIndex = position.IndexOf(" - ");
            if (hyphenIndex != -1)
            {
                return position.Substring(hyphenIndex + 3).Trim();
            }

            return string.Empty;
        }

        private async Task<double> GetTotalHoursAsync(int staffId, DateTime dta)
        {
            double totalHours = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours FROM [Zarplats].[dbo].[Staff_Schedule] as a LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE WHERE b.STAFF_ID = @staffid and a.DTA = @dta and CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) IS NOT NULL", connection))
                {
                    command.Parameters.AddWithValue("@staffid", staffId);
                    command.Parameters.AddWithValue("@dta", dta);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            totalHours = reader.GetDouble(0);
                        }
                    }
                }
            }

            return totalHours;
        }

        private async Task<double> GetWorkingHoursAsync(int staffId, DateTime dta)
        {
            double workingHours = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(
                    @"
                    SELECT DISTINCT
					    CAST(a.WorkingHours as FLOAT) as WorkingHours
				    FROM [Zarplats].[dbo].[Staff_Schedule] as a
				    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
				    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
				    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    WHERE b.STAFF_ID = @staffid and a.DTA = @dta
                    ", connection))
                {
                    command.Parameters.AddWithValue("@staffid", staffId);
                    command.Parameters.AddWithValue("@dta", dta);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            workingHours = reader.GetDouble(0);
                        }
                    }
                }
            }

            return workingHours;
        }

        public async Task<List<SZAndScheduleModel>> GetSZAndScheduleAsync(int staffId, DateTime startDate, DateTime finishDate)
        {
            List<SZAndScheduleModel> SZAndScheduleList = new List<SZAndScheduleModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = "SELECT DISTINCT c.SHORT_FIO as FIO, sz.Product, sz.Detail as DetailNum, dv.НазваниеД as DetailName, sz.NUM, dv.Договор as PP, CAST(ROUND(sz.Cost, 2) as FLOAT) as Cost, sz.Count, a.WorkingHours, a.DTA, CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours, LTRIM(c.TABEL_ID) as TABEL_ID FROM [Zarplats].[dbo].[Staff_Schedule] as a LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF LEFT JOIN PERCO...subdiv_ref AS d ON b.SUBDIV_ID = d.ID_REF LEFT JOIN PERCO...appoint_ref AS e ON b.APPOINT_ID = e.ID_REF LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE LEFT JOIN Cooperation.dbo.DetailsView as dv on sz.NUM = dv.ПрП and sz.Detail = dv.НомерД WHERE b.STAFF_ID = @staffid and a.DTA BETWEEN @startDate and @finishDate";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@staffid", staffId);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@finishDate", finishDate);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var szAndScheduleItem = new SZAndScheduleModel
                            {
                                TABEL_ID = reader["TABEL_ID"].ToString(),
                                FIO = reader["FIO"].ToString(),
                                Product = reader["Product"].ToString(),
                                DetailNum = reader["DetailNum"].ToString(),
                                DetailName = reader["DetailName"].ToString(),
                                NUM = reader["NUM"].ToString(),
                                PP = reader["PP"].ToString(),
                                Cost = reader["Cost"] != DBNull.Value ? Convert.ToDouble(reader["Cost"]) : 0,
                                Count = reader["Count"] != DBNull.Value ? Convert.ToDouble(reader["Count"]) : 0,
                                WorkingHours = reader["WorkingHours"] != DBNull.Value ? Convert.ToDouble(reader["WorkingHours"]) : 0,
                                DTA = Convert.ToDateTime(reader["DTA"]),
                                TotalHours = reader["TotalHours"] != DBNull.Value ? Convert.ToDouble(reader["TotalHours"]) : 0
                            };

                            SZAndScheduleList.Add(szAndScheduleItem);
                        }
                    }
                }
            }

            return SZAndScheduleList;
        }
    }
}
