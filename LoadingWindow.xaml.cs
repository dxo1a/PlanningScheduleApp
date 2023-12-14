using System.IO;
using System;
using System.Windows;
using Bitrix24Export;

namespace PlanningScheduleApp
{
    public partial class LoadingWindow : Window
    {
        public bool CanClose { get; set; }
        public static readonly string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string filePath, saveFileName, selectedMessageText, selectedUrlText, webhook, urlPreview;
        public int selectedFolder, selectedChat, chatType;
        public Export export;
        private ExportToExcelFilterWindow Exporter;

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public LoadingWindow(string task, ExportToExcelFilterWindow exporter)
        {
            InitializeComponent();
            TaskTB.Text = $"Выполняется: {task}.";
            ResizeMode = ResizeMode.NoResize;
            App.DisableAllWindowsExcept(this);
            CanClose = false;
            Closing += LoadingWindow_Closing;

            Exporter = exporter;
            Exporter.ProgressChanged += UpdateProgress;
        }

        private void LoadingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
            }
            else if (CanClose)
            {
                e.Cancel = false;
                App.EnableAllWindows();
            }
        }

        private void UpdateProgress(string status, int current, int total)
        {
            Dispatcher.Invoke(() => ProgressTB.Text = $"{status} ({current}/{total})");
        }

        public void ChangeText(string text, bool ready, string FilePath, string SaveFileName, string SelectedMessageText, string SelectedUrlText, string Webhook, string UrlPreview, int SelectedFolder, int SelectedChat, int ChatType)
        {
            filePath = FilePath; saveFileName = SaveFileName; selectedMessageText = SelectedMessageText;
            selectedUrlText = SelectedUrlText; webhook = Webhook; urlPreview = UrlPreview;
            selectedFolder = SelectedFolder; selectedChat = SelectedChat; chatType = ChatType;

            TaskTB.Text = text;
            Spinner.Visibility = Visibility.Collapsed;
            if (ready)
            {
                CanClose = true;
                OpenFolderBtn.IsEnabled = true;
                Bitrix24Export.IsEnabled = true;
                CloseBtn.IsEnabled = true;
                ProgressTB.Visibility = Visibility.Collapsed;
            }
            else
            {
                OpenFolderBtn.IsEnabled = false;
                Bitrix24Export.IsEnabled = false;
                CloseBtn.IsEnabled = true;
            }
        }

        private void CloseWindow()
        {
            if (CanClose)
            {
                this.Close();
            }
        }

        private void OpenFolderBtn_Click(object sender, RoutedEventArgs e)
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

        private void Bitrix24Export_Click(object sender, RoutedEventArgs e)
        {
            if (IsFileInUse(filePath))
            {
                MessageBox.Show($"Файл открыт или используется другим процессом и не может быть загружен.\n\n[ {filePath} ]", "Bitrix24", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var result = MessageBox.Show("Выгрузить файл? (Файл будет загружен на диск и отправлен в группу)", "Bitrix24", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    export = new Export(filePath, saveFileName, selectedMessageText, selectedUrlText, webhook, urlPreview, selectedFolder, selectedChat, chatType);
                    export.StartExport();
                    MessageBox.Show("Сообщение отправлено!", "Bitrix24", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
