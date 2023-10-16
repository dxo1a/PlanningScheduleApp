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
        double filterValue = 0;

        List<StaffModel> AllStaffList = new List<StaffModel>();
        List<StaffModel> TotalHours = new List<StaffModel>();
        List<SZAndScheduleModel> SZAndScheduleList = new List<SZAndScheduleModel>();

        List<StaffModel> staffModels = new List<StaffModel>();

        private LoadingWindow loadingWindow;

        double AcceptableFreeHoursFreeHours;
        double FreeHours;

        #region Переменные для Bitrix24

        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string webhookUrl = "https://steklm.bitrix24.ru/rest/771/qmsi75y8cfdg2vv1/";
        string filePath, saveFileName;
        string unloadingDate, unloadingTime;

        int selectedChat = 1222;                       // ID диалога (если чат, то заменить DIALOG_ID на CHAT_ID в SendMessageToChatWebhook.messageData)
        int selectedFolder = 305175;                  // ID папки (личная 305001, сменные 305175)
        string selectedMessageText = "Загруженность сменных заданий";
        string selectedUrlText = "Таблица со сменными заданиями";
        //string selectedDescriptionText = "Описание";

        #endregion

        public ExportToExcelFilterWindow()
        {
            InitializeComponent();
            
            AllStaffList = Odb.db.Database.SqlQuery<StaffModel>($"select distinct b.FIO as FIO, LTRIM(b.Tabel) as Tabel, a.WorkingHours, a.DTA, a.STAFF_ID, ID_Schedule from SerialNumber.dbo.Staff_Schedule as a left join SerialNumber.dbo.StaffView as b on a.STAFF_ID = b.STAFF_ID where b.VALID = 1 ORDER BY a.DTA DESC").ToList();
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

            await Task.Run(() => { ExportToExcel(); });

            loadingWindow.CanClose = true;
            loadingWindow.Close();
        }

        private void FinishDP_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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

        private void ExportToBitrix24Btn_Click(object sender, RoutedEventArgs e)
        {
            ExportToBitrix24(filePath, selectedFolder, selectedChat, selectedMessageText, selectedUrlText);
        }

        private void ExportToExcel()
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
                    Console.WriteLine($"Условие [totalHours < workingHours] для {staff.FIO} сработало.");
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
            try
            {
                excelApp.DisplayAlerts = false;
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
                                    worksheet1.Cells[rowCount, 1] = totalHour.FIO;
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
                filePath = $"{documentsPath}\\Общая загруженность сотрудников ({unloadingDate} {unloadingTime}).xlsx";

                workbook.SaveAs(filePath);

                Dispatcher.Invoke(() => { ExportToBitrix24Btn.IsEnabled = true; });
                saveFileName = $"Общая загруженность сотрудников ({unloadingDate} {unloadingTime}).xlsx";
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
            finally
            {
                Marshal.ReleaseComObject(excelApp);
            }
            #endregion
        }

        private async void ExportToBitrix24(string filePath, int folder, int chat, string messageText, string urlText)
        {
            var result = MessageBox.Show("Выгрузить файл? (Файл будет загружен на диск и отправлен в группу)", "Bitrix24", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                int fileId = await UploadFileToBitrix24(filePath, folder);

                await SendMessageToChatWebhook(chat, messageText, urlText, fileId);
            }
        }

        public async Task<int> UploadFileToBitrix24(string filePath, int folderId)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);         // используется документ который был создан при создании файла эксель
                string base64File = Convert.ToBase64String(fileBytes);

                var requestData = new
                {
                    id = folderId, // ID папки, в которую загружается файл
                    data = new
                    {
                        NAME = saveFileName
                    },
                    fileContent = base64File,
                    generateUniqueName = true // Уникализировать имя файла, если файл с таким именем уже существует
                };

                string jsonRequestData = JsonConvert.SerializeObject(requestData);
                Console.WriteLine($"{jsonRequestData}");
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{webhookUrl}disk.folder.uploadfile.json", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic responseData = JsonConvert.DeserializeObject(responseBody);
                    Console.WriteLine($"Файл [{saveFileName}] успешно сохранён в папку [{folderId}].");

                    if (responseData.result != null && responseData.result.ID != null)
                    {
                        int fileId = responseData.result.ID;
                        Console.WriteLine($"Присвоен ID: [{fileId}]");
                        return fileId;
                    }
                    else
                    {
                        MessageBox.Show("ID файла в ответе от сервера является null или недопустимого формата.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw new Exception("ID файла в ответе от сервера является null или недопустимого формата.");
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при загрузке файла на диск: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception($"Ошибка при загрузке файла на личный диск: {response.StatusCode} - {errorContent}");
                }
            }
        }

        public async Task<string> GetFileUrlById(int fileId)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{webhookUrl}disk.file.get.json?id={fileId}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic responseData = JsonConvert.DeserializeObject(responseBody);

                    if (responseData.result != null && responseData.result.DOWNLOAD_URL != null)
                    {
                        string fileUrl = responseData.result.DETAIL_URL;
                        Console.WriteLine($"Ссылка на файл с ID [{fileId}]: {fileUrl}");
                        return fileUrl;
                    }
                    else
                    {
                        MessageBox.Show("Невозможно получить ссылку на файл. Ответ от сервера не содержит ожидаемых данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw new Exception("Невозможно получить ссылку на файл. Ответ от сервера не содержит ожидаемых данных.");
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при получении ссылки на файл: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception($"Ошибка при получении ссылки на файл: {response.StatusCode} - {errorContent}");
                }
            }
        }

        

        public async Task<string> SendMessageToChatWebhook(int chatId, string message, string nameForUrl, int fileId)
        {
            string fileUrl = await GetFileUrlById(fileId);

            using (HttpClient client = new HttpClient())
            {
                var attachments = new[]
                {
                    new
                    {
                        LINK = new
                        {
                            PREVIEW = fileUrl,
                            WIDTH = 1000,
                            HEIGHT = 638,
                            NAME = nameForUrl,
                            LINK = fileUrl
                        }
                    }
                };

                var messageData = new
                {
                    DIALOG_ID = chatId.ToString(),
                    //CHAT_ID = chatId.ToString(), если чат
                    MESSAGE = message,
                    ATTACH = attachments
                };

                string jsonMessageData = JsonConvert.SerializeObject(messageData);
                var content = new StringContent(jsonMessageData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{webhookUrl}im.message.add.json", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Файл выгружен и сообщение отправлено!", "Bitrix24", MessageBoxButton.OK, MessageBoxImage.Information);
                    return responseBody;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при отправке сообщения в чат: {response.StatusCode} - {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new Exception($"Ошибка при отправке сообщения в чат: {response.StatusCode} - {errorContent}");
                }
            }
        }
    }
}
