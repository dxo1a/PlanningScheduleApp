using Microsoft.Office.Interop.Excel;
using PlanningScheduleApp.Models;
using PlanningScheduleApp.MVVM.View;
using PlanningScheduleApp.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    public partial class ExportToExcelFilterWindow : System.Windows.Window
    {
        DateTime StartDate, FinishDate;

        List<StaffModel> staffModels = new List<StaffModel>();

        #region Переменные для Bitrix24

        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string filePath, saveFileName, webhook, selectedMessageText, selectedUrlText, urlPreview;
        int selectedChat, selectedFolder, chatType;

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
                    await OpenProcessWindowForExportExcel();
                }
            }
            else
            {
                await OpenProcessWindowForExportExcel();
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

        private async Task OpenProcessWindowForExportExcel()
        {
            ProcessExportToExcelViewModel processViewModel = new ProcessExportToExcelViewModel();
            processViewModel.FilePath = filePath;
            processViewModel.SaveFileName = saveFileName;
            processViewModel.SelectedMessageText = selectedMessageText;
            processViewModel.SelectedUrlText = selectedUrlText;
            processViewModel.Webhook = webhook;
            processViewModel.UrlPreview = urlPreview;
            processViewModel.SelectedFolder = selectedFolder;
            processViewModel.SelectedChat = selectedChat;
            processViewModel.ChatType = chatType;
            processViewModel.StartDate = StartDate;
            processViewModel.FinishDate = FinishDate;
            processViewModel.WindowTitle = "Экспорт в Excel";

            processViewModel.TaskName = "выгрузка в Excel";


            ProcessWindow processWindow = new ProcessWindow();
            processWindow.DataContext = processViewModel;
            processWindow.Show();

            App.DisableAllWindows();
            processViewModel.CanClose = false;
            processViewModel.IsProcessPanelVisible = true;
            processViewModel.IsResultPanelVisible = false;
            await processViewModel.ExportToExcel(FreeHoursDataGrid);
            processViewModel.IsProcessPanelVisible = false;
            processViewModel.IsResultPanelVisible = true;
            processViewModel.CanClose = true;
            App.EnableAllWindows();

            processViewModel.TaskTextResult = "Запрос выполнен.\n\nВы можете открыть папку с файлом или отправить его в чат.";
        }
    }
}
