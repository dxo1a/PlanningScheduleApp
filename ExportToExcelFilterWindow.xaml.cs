using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace PlanningScheduleApp
{
    public partial class ExportToExcelFilterWindow : System.Windows.Window
    {
        DateTime StartDate, FinishDate;

        List<StaffModel> AllStaffList = new List<StaffModel>();
        List<StaffModel> TotalHours = new List<StaffModel>();
        List<SZAndScheduleModel> SZAndScheduleList = new List<SZAndScheduleModel>();

        List<StaffModel> staffModels = new List<StaffModel>();

        private LoadingWindow loadingWindow;

        double AcceptableFreeHoursFreeHours;
        double FreeHours;

        #region Переменные для Bitrix24

        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string filePath, saveFileName;
        string unloadingDate, unloadingTime, webhook;

        int selectedChat, selectedFolder, chatType;
        string selectedMessageText, selectedUrlText, urlPreview;
        //string selectedDescriptionText = "Описание";

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
            AllStaffList = Odb.db.Database.SqlQuery<StaffModel>($"select distinct b.FIO as FIO, LTRIM(b.Tabel) as Tabel, CAST((DATEDIFF(MINUTE, WorkBegin, WorkEnd) - ISNULL(LunchMinutes, 0)) / 60 AS FLOAT) AS WorkingHours, a.DTA, a.STAFF_ID, ID_Schedule from SerialNumber.dbo.Staff_Schedule as a left join SerialNumber.dbo.StaffView as b on a.STAFF_ID = b.STAFF_ID where b.VALID = 1 ORDER BY a.DTA DESC").ToList();
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
            loadingWindow = new LoadingWindow("выгрузка в Excel");
            loadingWindow.Show();

            await ExportToExcel();

            //loadingWindow.CanClose = true;
            loadingWindow.ChangeText("Запрос выполнен.\n\nВы можете открыть папку с файлом или отправить его в чат.", true, filePath, saveFileName, selectedMessageText, selectedUrlText, webhook, urlPreview, selectedFolder, selectedChat, chatType);
        }

        private void StartDP_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FinishDP.SelectedDate == null)
                FinishDP.SelectedDate = StartDP.SelectedDate;
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

        private async Task ExportToExcel()
        {
            await Task.Run(() =>
            {
                try
                {
                    #region Вычисление свободных часов
                    Dictionary<(int, DateTime), double> freeHoursDictionary = new Dictionary<(int, DateTime), double>();
                    foreach (var staff in AllStaffList)
                    {
                        double totalHours = Odb.db.Database.SqlQuery<double>(@"
                    SELECT DISTINCT 
                        CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours
                    FROM [SerialNumber].[dbo].[Staff_Schedule] as a
                    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
                    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
                    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    WHERE b.STAFF_ID = @staffid and a.DTA = @dta and CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) IS NOT NULL
                    ", new SqlParameter("staffid", staff.STAFF_ID), new SqlParameter("dta", staff.DTA)).SingleOrDefault();
                        double workingHours = Odb.db.Database.SqlQuery<double>(@"
                    SELECT DISTINCT
					    CAST(a.WorkingHours as FLOAT) as WorkingHours
				    FROM [SerialNumber].[dbo].[Staff_Schedule] as a
				    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
				    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
				    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    WHERE b.STAFF_ID = @staffid and a.DTA = @dta", new SqlParameter("staffid", staff.STAFF_ID), new SqlParameter("dta", staff.DTA)).SingleOrDefault();

                        // работает корректно, 4 записи с разными свободными часами на каждый день
                        if (totalHours < workingHours && totalHours > 0)
                        {
                            FreeHours = workingHours - totalHours;
                            freeHoursDictionary[(staff.STAFF_ID, staff.DTA)] = FreeHours;
                            foreach (var dt in freeHoursDictionary)
                            {
                                Console.WriteLine($"[Key: {dt.Key}] [Value: {dt.Value}]");
                            }
                            staff.FreeHours = FreeHours;
                            Console.WriteLine($"Условие [totalHours < workingHours] для {staff.SHORT_FIO} сработало.");
                        }
                    }
                    #endregion

                    #region Добавление словарей и "Свободные часы > 0"
                    List<StaffModel> staffWithPositiveFreeHours = AllStaffList.Where(s => s.FreeHours > 0).ToList(); // выбор только тех у кого свободные часы > 0

                    Dictionary<int, List<SZAndScheduleModel>> szandschedulesDataDictionary = new Dictionary<int, List<SZAndScheduleModel>>();
                    Dictionary<int, List<StaffModel>> totalHoursDataDictionary = new Dictionary<int, List<StaffModel>>();

                    #region Lists
                    foreach (var staff in staffWithPositiveFreeHours) //проверка уже в списке тех у которых свободных часов > 0
                    {
                        #region Добавление листов в словари
                        TotalHours = Odb.db.Database.SqlQuery<StaffModel>(@"
                    SELECT DISTINCT 
                     c.SHORT_FIO as FIO,
                     a.DTA,
                     CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours,
                     a.WorkingHours
                    FROM [SerialNumber].[dbo].[Staff_Schedule] as a
                    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
                    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
                    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    WHERE a.STAFF_ID = @staffid and a.DTA BETWEEN @startDate and @finishDate
                ", new SqlParameter("staffid", staff.STAFF_ID), new SqlParameter("startDate", StartDate.ToShortDateString()), new SqlParameter("finishDate", FinishDate.ToShortDateString())).ToList();
                        totalHoursDataDictionary[staff.STAFF_ID] = TotalHours;
                        //MessageBox.Show($"[Excel] Проверка загруженности: {staff.FIO} ({staff.STAFF_ID})");

                        SZAndScheduleList = Odb.db.Database.SqlQuery<SZAndScheduleModel>(@"
                SELECT DISTINCT
                    c.SHORT_FIO as FIO,
                    sz.Product,
                    sz.Detail as DetailNum,
                    dv.НазваниеД as DetailName,
                    sz.NUM,
                    dv.Договор as PP,
	                CAST(ROUND(sz.Cost, 2) as FLOAT) as Cost,
	                sz.Count,
	                a.WorkingHours,
	                a.DTA,
	                CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours
                FROM [SerialNumber].[dbo].[Staff_Schedule] as a
                LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
                LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
                LEFT JOIN PERCO...subdiv_ref AS d ON b.SUBDIV_ID = d.ID_REF
                LEFT JOIN PERCO...appoint_ref AS e ON b.APPOINT_ID = e.ID_REF
                LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                LEFT JOIN Cooperation.dbo.DetailsView as dv on sz.NUM = dv.ПрП and sz.Detail = dv.НомерД
                WHERE b.STAFF_ID = @staffid and a.DTA BETWEEN @startDate and @finishDate
            ", new SqlParameter("staffid", staff.STAFF_ID), new SqlParameter("startDate", StartDate), new SqlParameter("finishDate", FinishDate)).ToList();
                        szandschedulesDataDictionary[staff.STAFF_ID] = SZAndScheduleList;
                        //ActionTB.Text = $"[Excel] Проверка сменных заданий: {staff.FIO} ({staff.Tabel})";
                        #endregion
                    }
                    #endregion
                    #endregion

                    #region Excel

                    Excel.Application excelApp = new Excel.Application();
                    excelApp.DisplayAlerts = true;
                    excelApp.Visible = true;

                    Workbook workbook = excelApp.Workbooks.Add();

                    #region Сменные задания
                    Worksheet worksheet2 = (Excel.Worksheet)workbook.Worksheets[1];
                    worksheet2.Name = "Сменные задания";


                    Excel.Range headerRange2 = worksheet2.get_Range("A1", "H1");
                    headerRange2.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    headerRange2.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    worksheet2.Cells[1, 1] = "ФИО";
                    worksheet2.Cells[1, 2] = "Изделие";
                    worksheet2.Cells[1, 3] = "ПП";
                    worksheet2.Cells[1, 4] = "ПрП";
                    worksheet2.Cells[1, 5] = "Деталь";
                    worksheet2.Cells[1, 6] = "Количество";
                    worksheet2.Cells[1, 7] = "Время";
                    worksheet2.Cells[1, 8] = "Дата";

                    int rowCountSZ = 2;

                    foreach (var staffData in szandschedulesDataDictionary)
                    {
                        int staffId = staffData.Key;
                        List<SZAndScheduleModel> szAndScheduleList = staffData.Value;

                        foreach (var sz in szAndScheduleList)
                        {
                            if (sz.TotalHours.HasValue)
                            {
                                double totalHoursValue = sz.TotalHours.Value;
                                double workingHoursValue = sz.WorkingHours.Value;
                                double freeHours = freeHoursDictionary.ContainsKey((staffId, sz.DTA)) ? freeHoursDictionary[(staffId, sz.DTA)] : 0;
                                sz.FreeHours = freeHours;
                                foreach (StaffModel model in staffModels)
                                {
                                    double acceptableHours = model.AcceptableFreeHours;

                                    if (sz.Product != null && sz.Detail != null && sz.NUM != null && sz.Cost != null && sz.Count != null && totalHoursValue < workingHoursValue && sz.FreeHours > acceptableHours && sz.DTA == model.DTA)
                                    {
                                        worksheet2.Cells[rowCountSZ, 1] = sz.FIO;
                                        worksheet2.Cells[rowCountSZ, 2] = sz.Product;
                                        worksheet2.Cells[rowCountSZ, 3].NumberFormat = "@";
                                        worksheet2.Cells[rowCountSZ, 3] = sz.PP.ToString();
                                        worksheet2.Cells[rowCountSZ, 4] = sz.NUM;
                                        worksheet2.Cells[rowCountSZ, 5] = sz.Detail;
                                        worksheet2.Cells[rowCountSZ, 6] = sz.Count;
                                        worksheet2.Cells[rowCountSZ, 7] = sz.Cost;
                                        worksheet2.Cells[rowCountSZ, 8] = sz.DTA.ToShortDateString();

                                        rowCountSZ++;
                                    }
                                }
                            }
                        }
                    }
                    worksheet2.Columns.AutoFit();
                    #endregion

                    #region Общая загруженность
                    Worksheet worksheet1 = (Worksheet)workbook.Worksheets.Add();
                    worksheet1.Name = "Общая загруженность";

                    worksheet1.Cells[1, 1] = "ФИО";
                    worksheet1.Cells[1, 2] = "Дата";
                    worksheet1.Cells[1, 3] = "Общая нагруженность";
                    worksheet1.Cells[1, 4] = "Свободные часы";
                    Excel.Range headerRange1 = worksheet1.get_Range("A1", "D1");
                    headerRange1.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    headerRange1.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    int rowCount = 2;

                    foreach (var totalHoursData in totalHoursDataDictionary)
                    {
                        int staffId = totalHoursData.Key;
                        List<StaffModel> totalHoursList = totalHoursData.Value;

                        foreach (var totalHour in totalHoursList)
                        {
                            if (totalHour.TotalHours.HasValue)
                            {
                                double totalHoursValue = totalHour.TotalHours.Value;
                                double workingHoursValue = totalHour.WorkingHours.Value;
                                double freeHours = freeHoursDictionary.ContainsKey((staffId, totalHour.DTA)) ? freeHoursDictionary[(staffId, totalHour.DTA)] : 0;
                                totalHour.FreeHours = freeHours;
                                foreach (StaffModel model in staffModels)
                                {
                                    double acceptableHours = model.AcceptableFreeHours;

                                    if (totalHoursValue < workingHoursValue && totalHour.FreeHours > acceptableHours && totalHour.DTA == model.DTA)
                                    {
                                        worksheet1.Cells[rowCount, 1] = totalHour.SHORT_FIO;
                                        worksheet1.Cells[rowCount, 2] = totalHour.DTA.ToShortDateString();
                                        worksheet1.Cells[rowCount, 3] = totalHour.TotalHours.HasValue ? totalHour.TotalHours.Value.ToString() : "N/A";
                                        worksheet1.Cells[rowCount, 4] = freeHoursDictionary.ContainsKey((staffId, totalHour.DTA)) ? freeHoursDictionary[(staffId, totalHour.DTA)].ToString() : "N/A";

                                        rowCount++;
                                    }
                                }
                            }

                        }
                    }
                    worksheet1.Columns.AutoFit();
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


                    Marshal.ReleaseComObject(worksheet2);
                    Marshal.ReleaseComObject(worksheet1);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выгрузке в Excel: [{ex}]\nSource: [{ex.Source}]\nMethod: [{ex.TargetSite}]\nInnerException: [{ex.InnerException}]");
                }
            });

            #endregion
        }
    }
}
