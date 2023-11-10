using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    /// <summary>
    /// Логика взаимодействия для ScheduleAddWindow.xaml
    /// </summary>
    public partial class ScheduleAddWindow : Window
    {
        public ObservableCollection<ScheduleTemplateModel> Days { get; set; }

        public ScheduleAddWindow()
        {
            InitializeComponent();
            InitializeDays();
            DataContext = this;
        }

        #region Проверка TBX
        private void TBX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Разрешить ввод только цифр и одной точки или запятой
            if (!IsNumericInput(e.Text) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
            }
            else if (e.Text == "." || e.Text == ",")
            {
                // Если введена точка или запятая, заменить ее на запятую
                if (textBox != null)
                {
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, ",");
                    textBox.CaretIndex = caretIndex + 1;
                    e.Handled = true;
                }
            }
        }

        private void TBX_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Разрешить удаление символа
            if (e.Key == Key.Back)
                return;

            // Запретить ввод пробела
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private bool IsNumericInput(string text)
        {
            // Проверить, является ли введенный текст числом, запятой или точкой
            return double.TryParse(text, out double result) || text == "," || text == ".";
        }
        #endregion

        private void MTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null)
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.Select(0, 0); }));
            }
        }

        private void InitializeDays()
        {
            Days = new ObservableCollection<ScheduleTemplateModel>
            {
                new ScheduleTemplateModel { Day = "Понедельник" },
                new ScheduleTemplateModel { Day = "Вторник" },
                new ScheduleTemplateModel { Day = "Среда" },
                new ScheduleTemplateModel { Day = "Четверг" },
                new ScheduleTemplateModel { Day = "Пятница" },
                new ScheduleTemplateModel { Day = "Суббота" },
                new ScheduleTemplateModel { Day = "Воскресенье" }
            };
        }

        private void AddFixedTemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TemplateNameTBX.Text))
            {
                MessageBox.Show("Введите название графика.");
                return;
            }

            string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int restingDaysCount = Days.Count(day => !day.isRestingDay);
                // Создание объекта Schedule_Template и вставка в базу данных
                using (SqlCommand command = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_Template (TemplateName, isFlexible, RestingDaysCount, WorkingDaysCount) VALUES (@TemplateName, @isFlexible, @RestingDaysCount, @WorkingDaysCount); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@TemplateName", TemplateNameTBX.Text);
                    command.Parameters.AddWithValue("@isFlexible", false); // Замените на ваше значение
                    command.Parameters.AddWithValue("@RestingDaysCount", restingDaysCount); // Замените на ваше значение
                    command.Parameters.AddWithValue("@WorkingDaysCount", Days.Count(day => day.isRestingDay));

                    int templateId = Convert.ToInt32(command.ExecuteScalar());

                    // Создание объекта Schedule_StaticDays для каждого дня и вставка в базу данных
                    foreach (var day in Days)
                    {
                        using (SqlCommand staticDaysCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_StaticDays (Day, WorkBegin, WorkEnd, LunchTime, Template_ID, isRestingDay) VALUES (@Day, @WorkBegin, @WorkEnd, @LunchTime, @Template_ID, @isRestingDay);", connection))
                        {
                            staticDaysCommand.Parameters.AddWithValue("@Day", day.Day);
                            staticDaysCommand.Parameters.AddWithValue("@WorkBegin", day.WorkBegin ?? String.Empty);
                            staticDaysCommand.Parameters.AddWithValue("@WorkEnd", day.WorkEnd ?? String.Empty);
                            staticDaysCommand.Parameters.AddWithValue("@LunchTime", day.LunchTime ?? 0);
                            staticDaysCommand.Parameters.AddWithValue("@Template_ID", templateId);
                            staticDaysCommand.Parameters.AddWithValue("@isRestingDay", !day.isRestingDay);

                            staticDaysCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            MessageBox.Show("График успешно добавлен в базу данных.");
        }

        private void MaskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null && string.IsNullOrEmpty(maskedTextBox.Text))
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.CaretIndex = 0; }));
            }
        }
    }
}
