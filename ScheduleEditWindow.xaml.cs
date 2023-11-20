using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    public partial class ScheduleEditWindow : Window
    {
        public event EventHandler TemplateCreated;

        public int ID_Template;

        private ScheduleTemplateModel SelectedTemplate;

        private ObservableCollection<ScheduleTemplateModel> StaticDays { get; set; }
        private ObservableCollection<ScheduleTemplateModel> FlexibleDays { get; set; }

        public ScheduleEditWindow(ScheduleTemplateModel selectedTemplate)
        {
            InitializeComponent();
           
            SelectedTemplate = selectedTemplate;
            WorkingDaysCountCMB.ItemsSource = Enumerable.Range(1, 6);
            WorkingDaysCountCMB.SelectedIndex = 0;
            TemplateNameTBX.Text = $"{SelectedTemplate.TemplateName}";

            AssignDays();

            DataContext = this;
        }

        public void UpdateFlexibleDaysCollection()
        {
            int workingDaysCount = WorkingDaysCountCMB.SelectedIndex + 1;

            while (FlexibleDays.Count > workingDaysCount)
            {
                FlexibleDays.RemoveAt(FlexibleDays.Count - 1);
            }

            var existingDays = FlexibleDays.Take(workingDaysCount).ToList();

            for (int i = 0; i < workingDaysCount; i++)
            {
                if (i < existingDays.Count)
                {
                    existingDays[i].Day = $"День {i + 1}";
                }
                else
                {
                    FlexibleDays.Add(new ScheduleTemplateModel
                    {
                        Day = $"День {i + 1}"
                    });
                }
            }

            OnPropertyChanged(nameof(FlexibleDays));
        }

        private void UpdateRestingDaysComboBox()
        {
            int workingDaysCount = (int)WorkingDaysCountCMB.SelectedValue;
            RestingDaysCountCMB.ItemsSource = Enumerable.Range(1, 7 - workingDaysCount);
            RestingDaysCountCMB.SelectedIndex = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AssignDays()
        {
            if (!SelectedTemplate.isFlexible)
            {
                this.MinHeight = 430; this.Height = this.MinHeight;
                FlexibleScheduleTI.Visibility = Visibility.Collapsed;
                List<ScheduleTemplateModel> staticDays = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select ID_Day, Day, WorkBegin, WorkEnd, LunchTimeBegin, LunchTimeEnd, isRestingDay from Zarplats.dbo.Schedule_StaticDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template)).ToList();
                StaticDays = new ObservableCollection<ScheduleTemplateModel>(staticDays);
                StaticDaysIC.ItemsSource = StaticDays;
            }
            else
            {
                StaticScheduleTI.Visibility = Visibility.Collapsed;
                ScheduleTC.SelectedIndex = 1;
                List<ScheduleTemplateModel> flexibleDays = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select ID_Day, WorkBegin, WorkEnd, LunchTimeBegin, LunchTimeEnd from Zarplats.dbo.Schedule_FlexibleDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template)).ToList();
                FlexibleDays = new ObservableCollection<ScheduleTemplateModel>(flexibleDays.Select((day, index) => new ScheduleTemplateModel
                {
                    ID_Day = day.ID_Day,
                    Day = $"День {index + 1}",
                    WorkBegin = day.WorkBegin,
                    WorkEnd = day.WorkEnd,
                    LunchTimeBegin = day.LunchTimeBegin,
                    LunchTimeEnd = day.LunchTimeEnd
                }));
                FlexibleDaysIC.ItemsSource = FlexibleDays;
                
                WorkingDaysCountCMB.SelectedIndex = SelectedTemplate.WorkingDaysCount - 1;
                RestingDaysCountCMB.ItemsSource = Enumerable.Range(1, 7 - (int)WorkingDaysCountCMB.SelectedValue);
                RestingDaysCountCMB.SelectedIndex = SelectedTemplate.RestingDaysCount - 1;
            }

            if (SelectedTemplate.TemplateName.Contains("("))
            {
                int indexOfOpen = SelectedTemplate.TemplateName.IndexOf("(");
                string textBefore = SelectedTemplate.TemplateName.Substring(0, indexOfOpen);
                string textAfter = SelectedTemplate.TemplateName.Substring(indexOfOpen);

                TemplateNameTBX.Text = textBefore.Trim();
                TemplateAdditionalNameTBX.Text = textAfter.Trim();
            }
        }

        #region UI
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

        private void MaskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null && string.IsNullOrEmpty(maskedTextBox.Text))
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.CaretIndex = 0; }));
            }
        }

        private void MTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null)
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.Select(0, 0); }));
            }
        }
        #endregion

        private void WorkingDaysCountCMB_DropDownClosed(object sender, EventArgs e)
        {
            TemplateAdditionalNameTBX.Text = $"({WorkingDaysCountCMB.SelectedValue}/{RestingDaysCountCMB.SelectedValue})";
            UpdateRestingDaysComboBox();
            UpdateFlexibleDaysCollection();
        }

        private void RestingDaysCountCMB_DropDownClosed(object sender, EventArgs e)
        {
            TemplateAdditionalNameTBX.Text = $"({WorkingDaysCountCMB.SelectedValue}/{RestingDaysCountCMB.SelectedValue})";
            UpdateFlexibleDaysCollection();
        }

        private void SaveFixedTemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TemplateNameTBX.Text))
            {
                MessageBox.Show("Введите название графика.");
                return;
            }

            string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    int restingDaysCount = StaticDays.Count(day => day.isRestingDay);
                    int workingDaysCount = StaticDays.Count(day => !day.isRestingDay);
                    int checkExisting = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT 1 FROM Zarplats.dbo.Schedule_Template WHERE TemplateName LIKE @TemplateName) SELECT 1 ELSE SELECT 0", new SqlParameter("TemplateName", $"%{TemplateNameTBX.Text}%")).SingleOrDefault();
                    if (!Convert.ToBoolean(checkExisting))
                    {
                        using (SqlCommand updateTemplateCommand = new SqlCommand("UPDATE Zarplats.dbo.Schedule_Template SET TemplateName = @TemplateName, RestingDaysCount = @RestingDaysCount, WorkingDaysCount = @WorkingDaysCount WHERE ID_Template = @templateid", connection))
                        {
                            updateTemplateCommand.Parameters.AddWithValue("@templateid", SelectedTemplate.ID_Template);
                            updateTemplateCommand.Parameters.AddWithValue("@TemplateName", TemplateNameTBX.Text + " " + TemplateAdditionalNameTBX.Text);
                            updateTemplateCommand.Parameters.AddWithValue("@RestingDaysCount", restingDaysCount);
                            updateTemplateCommand.Parameters.AddWithValue("@WorkingDaysCount", workingDaysCount);

                            foreach (var day in StaticDays)
                            {
                                using (SqlCommand staticDaysCommand = new SqlCommand("UPDATE Zarplats.dbo.Schedule_StaticDays SET WorkBegin = @WorkBegin, WorkEnd = @WorkEnd, LunchTimeBegin = @LunchTimeBegin, LunchTimeEnd = @LunchTimeEnd, isRestingDay = @isRestingDay WHERE Template_ID = @templateid and ID_Day = @idday", connection))
                                {
                                    staticDaysCommand.Parameters.AddWithValue("@WorkBegin", day.WorkEnd != null && day.WorkBegin.Any(char.IsDigit) ? day.WorkBegin : string.Empty);
                                    staticDaysCommand.Parameters.AddWithValue("@WorkEnd", day.WorkEnd != null && day.WorkEnd.Any(char.IsDigit) ? day.WorkEnd : string.Empty);
                                    staticDaysCommand.Parameters.AddWithValue("@LunchTimeBegin", day.LunchTimeBegin ?? string.Empty);
                                    staticDaysCommand.Parameters.AddWithValue("@LunchTimeEnd", day.LunchTimeEnd ?? string.Empty);
                                    staticDaysCommand.Parameters.AddWithValue("@isRestingDay", day.isRestingDay);
                                    staticDaysCommand.Parameters.AddWithValue("@templateid", SelectedTemplate.ID_Template);
                                    staticDaysCommand.Parameters.AddWithValue("@idday", day.ID_Day);

                                    staticDaysCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Шаблон с таким названием уже существует!");
                        return;
                    }
                }
                MessageBox.Show("График обновлён.");
                TemplateCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении фиксированного графика: {ex.Message}");
            }
        }

        private void SaveFlexibleTemplateBtn_Click(object sender, RoutedEventArgs e)
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
                int checkExisting = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT 1 FROM Zarplats.dbo.Schedule_Template WHERE TemplateName LIKE @TemplateName AND ID_Template <> @templateid) SELECT 1 ELSE SELECT 0", new SqlParameter("TemplateName", $"%{TemplateNameTBX.Text}%"), new SqlParameter("templateid", SelectedTemplate.ID_Template)).SingleOrDefault();
                if (!Convert.ToBoolean(checkExisting))
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        using (SqlCommand deleteCommand = new SqlCommand("DELETE FROM Zarplats.dbo.Schedule_FlexibleDays WHERE Template_ID = @templateid", connection, transaction))
                        {
                            deleteCommand.Parameters.AddWithValue("@templateid", SelectedTemplate.ID_Template);
                            deleteCommand.ExecuteNonQuery();
                        }

                        // Обновление существующей записи в Schedule_Template
                        using (SqlCommand updateTemplateCommand = new SqlCommand("UPDATE Zarplats.dbo.Schedule_Template SET TemplateName = @TemplateName, RestingDaysCount = @RestingDaysCount, WorkingDaysCount = @WorkingDaysCount WHERE ID_Template = @templateid", connection, transaction))
                        {
                            updateTemplateCommand.Parameters.AddWithValue("@templateid", SelectedTemplate.ID_Template);
                            updateTemplateCommand.Parameters.AddWithValue("@TemplateName", TemplateNameTBX.Text + " " + TemplateAdditionalNameTBX.Text);
                            updateTemplateCommand.Parameters.AddWithValue("@RestingDaysCount", RestingDaysCountCMB.SelectedValue);
                            updateTemplateCommand.Parameters.AddWithValue("@WorkingDaysCount", WorkingDaysCountCMB.SelectedValue);

                            updateTemplateCommand.ExecuteNonQuery();
                        }

                        // Вставка новых записей в Schedule_FlexibleDays
                        foreach (var day in FlexibleDays)
                        {
                            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_FlexibleDays (WorkBegin, WorkEnd, LunchTimeBegin, LunchTimeEnd, Template_ID) VALUES (@WorkBegin, @WorkEnd, @LunchTimeBegin, @LunchTimeEnd, @Template_ID);", connection, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@WorkBegin", day.WorkBegin ?? String.Empty);
                                insertCommand.Parameters.AddWithValue("@WorkEnd", day.WorkEnd ?? String.Empty);
                                insertCommand.Parameters.AddWithValue("@LunchTimeBegin", day.LunchTimeBegin ?? string.Empty);
                                insertCommand.Parameters.AddWithValue("@LunchTimeEnd", day.LunchTimeEnd ?? string.Empty);
                                insertCommand.Parameters.AddWithValue("@Template_ID", SelectedTemplate.ID_Template);

                                insertCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("График обновлён.");
                        TemplateCreated?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при обновлении графика: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Шаблон с таким названием уже существует!");
                    return;
                }
            }
        }

        private void isRestingDayCB_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                ScheduleTemplateModel day = checkBox.DataContext as ScheduleTemplateModel;

                if (day != null)
                {
                    day.WorkBegin = string.Empty;
                    day.WorkEnd = string.Empty;
                    day.LunchTimeBegin = string.Empty;
                    day.LunchTimeEnd = string.Empty;
                    day.isRestingDay = true;
                }
            }
        }
    }
}
