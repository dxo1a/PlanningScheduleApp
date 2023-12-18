using PlanningScheduleApp.Models;
using PlanningScheduleApp.MVVM.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using PlanningScheduleApp.MVVM.Commands;
using Bitrix24Export;
using System.Windows;
using Window = System.Windows.Window;
using System.Threading;

namespace PlanningScheduleApp.MVVM.ViewModel
{
    public class ProcessExportToExcelViewModel : OnPropertyChanged
    {
        List<StaffModel> AllStaffList = new List<StaffModel>();
        List<StaffModel> TotalHoursList = new List<StaffModel>();
        List<SZAndScheduleModel> SZAndScheduleList = new List<SZAndScheduleModel>();

        List<StaffModel> staffModels = new List<StaffModel>();

        private CancellationTokenSource _cancellationTokenSource;

        private string unloadingDate, unloadingTime;

        double FreeHours;

        public RelayCommand ExportToBitrixCommand { get; }
        public RelayCommand OpenFolderCommand { get; }
        public RelayCommand CloseWindowCommand { get; }

        #region Переменные для Bitrix24
        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";
        public Export export;
        #endregion

        public ProcessExportToExcelViewModel()
        {
            ExportToBitrixCommand = new RelayCommand(ExportToBitrixExecute);
            OpenFolderCommand = new RelayCommand(OpenFolderExecute);
            CloseWindowCommand = new RelayCommand(CloseWindowExecute);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        #region Variables
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _finishDate;
        public DateTime FinishDate
        {
            get { return _finishDate; }
            set
            {
                _finishDate = value;
                NotifyPropertyChanged(nameof(FinishDate));
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyPropertyChanged(nameof(FilePath));
            }
        }

        private string _saveFileName;
        public string SaveFileName
        {
            get { return _saveFileName; }
            set
            {
                _saveFileName = value;
                NotifyPropertyChanged(nameof(SaveFileName));
            }
        }

        private string _selectedMessageText;
        public string SelectedMessageText
        {
            get { return _selectedMessageText; }
            set
            {
                _selectedMessageText = value;
                NotifyPropertyChanged(nameof(SelectedMessageText));
            }
        }

        private string _selectedUrlText;
        public string SelectedUrlText
        {
            get { return _selectedUrlText; }
            set
            {
                _selectedUrlText = value;
                NotifyPropertyChanged(nameof(SelectedUrlText));
            }
        }

        private string _webhook;
        public string Webhook
        {
            get { return _webhook; }
            set
            {
                _webhook = value;
                NotifyPropertyChanged(nameof(Webhook));
            }
        }

        private string _urlPreview;
        public string UrlPreview
        {
            get { return _urlPreview; }
            set
            {
                _urlPreview = value;
                NotifyPropertyChanged(nameof(UrlPreview));
            }
        }

        private int _selectedFolder;
        public int SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                NotifyPropertyChanged(nameof(SelectedFolder));
            }
        }

        private int _selectedChat;
        public int SelectedChat
        {
            get { return _selectedChat; }
            set
            {
                _selectedChat = value;
                NotifyPropertyChanged(nameof(SelectedChat));
            }
        }

        private int _chatType;
        public int ChatType
        {
            get { return _chatType; }
            set
            {
                _chatType = value;
                NotifyPropertyChanged(nameof(ChatType));
            }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                NotifyPropertyChanged(nameof(CurrentProgress));
            }
        }

        private int _totalProgress;
        public int TotalProgress
        {
            get { return _totalProgress; }
            set
            {
                _totalProgress = value;
                NotifyPropertyChanged(nameof(TotalProgress));
            }
        }

        private bool _isSuccess;
        public bool IsSuccess
        {
            get { return _isSuccess; }
            set
            {
                _isSuccess = value;
                NotifyPropertyChanged(nameof(IsSuccess));
            }
        }

        private bool _canClose;
        public bool CanClose
        {
            get { return _canClose; }
            set
            {
                _canClose = value;
                NotifyPropertyChanged(nameof(CanClose));
            }
        }

        private bool _isProcessPanelVisible;
        public bool IsProcessPanelVisible
        {
            get { return _isProcessPanelVisible; }
            set
            {
                _isProcessPanelVisible = value;
                NotifyPropertyChanged(nameof(IsProcessPanelVisible));
            }
        }

        private bool _isResultPanelVisible;
        public bool IsResultPanelVisible
        {
            get { return _isResultPanelVisible; }
            set
            {
                _isResultPanelVisible = value;
                NotifyPropertyChanged(nameof(IsResultPanelVisible));
            }
        }

        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                NotifyPropertyChanged(nameof(TaskName));
            }
        }

        private string _taskTextResult;
        public string TaskTextResult
        {
            get { return _taskTextResult; }
            set
            {
                _taskTextResult = value;
                NotifyPropertyChanged(nameof(TaskTextResult));
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyPropertyChanged(nameof(WindowTitle));
            }
        }
        #endregion

        public async Task ExportToExcel(DataGrid dataGrid)
        {
            CurrentProgress = 0;
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
                    CurrentProgress++;
                    TotalProgress = AllStaffList.Count + 1;
                }
                #endregion

                // "Свободные часы > 0"
                List<StaffModel> staffWithPositiveFreeHours = AllStaffList.Where(s => s.FreeHours > 0).ToList();

                CurrentProgress++;
                TotalProgress = AllStaffList.Count + 1;
                Dictionary<int, List<SZAndScheduleModel>> szandschedulesDataDictionary = new Dictionary<int, List<SZAndScheduleModel>>();

                CurrentProgress = 0;
                List<Company> companies = new List<Company>();
                List<Task<List<SZAndScheduleModel>>> tasks = new List<Task<List<SZAndScheduleModel>>>();
                List<int> staffIds = new List<int>();
                foreach (StaffModel staff in staffWithPositiveFreeHours)
                {
                    double totalHours = await GetTotalHoursAsync(staff.STAFF_ID, staff.DTA);
                    double workingHours = await GetWorkingHoursAsync(staff.STAFF_ID, staff.DTA);

                    var dataGridRow = dataGrid.Items.Cast<StaffModel>().FirstOrDefault(item => item.DTA == staff.DTA);

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
                        staffIds.Add(staff.STAFF_ID);
                        #endregion

                        #region Сменные задания
                        tasks.Add(GetSZAndScheduleAsync(staff.STAFF_ID, StartDate, FinishDate, dataGrid));
                        #endregion
                    }
                    CurrentProgress++;
                    TotalProgress = staffWithPositiveFreeHours.Count + 1;
                };
                await Task.WhenAll(tasks);

                for (int i = 0; i < staffIds.Count; i++)
                {
                    szandschedulesDataDictionary[staffIds[i]] = tasks[i].Result;
                }
                CurrentProgress++;
                TotalProgress = staffWithPositiveFreeHours.Count + 1;

                #region Excel
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.DisplayAlerts = true;
                excelApp.Visible = true;

                Workbook workbook = excelApp.Workbooks.Add();

                #region Сменные задания
                Worksheet worksheet2 = (Worksheet)workbook.Worksheets[1];
                worksheet2.Name = "Сменные задания";


                Range headerRange2 = worksheet2.get_Range("A1", "I1");
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
                SaveFileName = $"Общая загруженность сотрудников {StartDate.ToShortDateString()}-{FinishDate.ToShortDateString()} ({unloadingDate} {unloadingTime}).xlsx";
                FilePath = $"{documentsPath}\\График сотрудников\\{SaveFileName}";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                workbook.SaveAs(FilePath);


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

        public async Task<List<SZAndScheduleModel>> GetSZAndScheduleAsync(int staffId, DateTime startDate, DateTime finishDate, DataGrid dataGrid)
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
                            var sz = new SZAndScheduleModel
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

                            var dataGridRow = dataGrid.Items.Cast<StaffModel>().FirstOrDefault(item => item.DTA == sz.DTA);

                            if (dataGridRow != null)
                            {
                                sz.AcceptableFreeHours = dataGridRow.AcceptableFreeHours;
                            }

                            double? FreeHours = sz.WorkingHours - sz.TotalHours;
                            if (sz.Product != null && sz.Detail != null && sz.NUM != null && sz.Cost != 0 && sz.Count != 0 && sz.TotalHours < sz.WorkingHours && sz.TotalHours > 0 && FreeHours > sz.AcceptableFreeHours)
                                SZAndScheduleList.Add(sz);
                        }
                    }
                }
            }

            return SZAndScheduleList;
        }

        private void ExportToBitrixExecute(object parameter)
        {
            if (IsFileInUse(FilePath))
            {
                MessageBox.Show($"Файл открыт или используется другим процессом и не может быть загружен.\n\n[ {FilePath} ]", "Bitrix24", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var result = MessageBox.Show("Выгрузить файл? (Файл будет загружен на диск и отправлен в группу)", "Bitrix24", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    export = new Export(FilePath, SaveFileName, SelectedMessageText, SelectedUrlText, Webhook, UrlPreview, SelectedFolder, SelectedChat, ChatType);
                    export.StartExport();
                    MessageBox.Show("Сообщение отправлено!", "Bitrix24", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void OpenFolderExecute(object parameter)
        {
            try
            {
                System.Diagnostics.Process.Start($"{documentsPath}\\График сотрудников");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть папку: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindowExecute(object parameter)
        {
            if (CanClose)
            {
                _cancellationTokenSource.Cancel();
                (parameter as Window)?.Close();
            }   
        }

        public bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}