﻿using PlanningScheduleApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    public partial class ScheduleAddWindow : Window
    {
        public ObservableCollection<ScheduleTemplateModel> StaticDays { get; set; }
        public ObservableCollection<ScheduleTemplateModel> FlexibleDays { get; set; }

        public event EventHandler TemplateCreated;

        public ScheduleAddWindow()
        {
            InitializeComponent();
            InitializeDays();

            FlexibleDays = new ObservableCollection<ScheduleTemplateModel>();

            WorkingDaysCountCMB.ItemsSource = Enumerable.Range(1, 6);
            WorkingDaysCountCMB.SelectedIndex = 0;
            UpdateRestingDaysComboBox();

            DataContext = this;

            this.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                    this.Close();
            };
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
            StaticDays = new ObservableCollection<ScheduleTemplateModel>
            {
                new ScheduleTemplateModel { Day = "Monday" },
                new ScheduleTemplateModel { Day = "Tuesday" },
                new ScheduleTemplateModel { Day = "Wednesday" },
                new ScheduleTemplateModel { Day = "Thursday" },
                new ScheduleTemplateModel { Day = "Friday" },
                new ScheduleTemplateModel { Day = "Saturday" },
                new ScheduleTemplateModel { Day = "Sunday" }
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
                int restingDaysCount = StaticDays.Count(day => day.isRestingDay);
                int workingDaysCount = StaticDays.Count(day => !day.isRestingDay);
                int checkExisting = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT 1 FROM Zarplats.dbo.Schedule_Template WHERE TemplateName LIKE @TemplateName) SELECT 1 ELSE SELECT 0", new SqlParameter("TemplateName", $"%{TemplateNameTBX.Text}%")).SingleOrDefault();
                if (!Convert.ToBoolean(checkExisting))
                {
                    // Создание объекта Schedule_Template и вставка в базу данных
                    using (SqlCommand command = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_Template (TemplateName, isFlexible, RestingDaysCount, WorkingDaysCount) VALUES (@TemplateName, @isFlexible, @RestingDaysCount, @WorkingDaysCount); SELECT SCOPE_IDENTITY();", connection))
                    {
                        command.Parameters.AddWithValue("@TemplateName", $"{TemplateNameTBX.Text} {TemplateAdditionalNameTBX.Text}");
                        command.Parameters.AddWithValue("@isFlexible", false);
                        command.Parameters.AddWithValue("@RestingDaysCount", restingDaysCount);
                        command.Parameters.AddWithValue("@WorkingDaysCount", workingDaysCount);

                        int templateId = Convert.ToInt32(command.ExecuteScalar());

                        // Создание объекта Schedule_StaticDays для каждого дня и вставка в базу данных
                        foreach (var day in StaticDays)
                        {
                            using (SqlCommand staticDaysCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_StaticDays (Day, WorkBegin, WorkEnd, LunchTimeBegin, LunchTimeEnd, Template_ID, isRestingDay) VALUES (@Day, @WorkBegin, @WorkEnd, @LunchTimeBegin, @LunchTimeEnd, @Template_ID, @isRestingDay);", connection))
                            {
                                staticDaysCommand.Parameters.AddWithValue("@Day", day.Day);
                                staticDaysCommand.Parameters.AddWithValue("@WorkBegin", day.WorkBegin ?? string.Empty);
                                staticDaysCommand.Parameters.AddWithValue("@WorkEnd", day.WorkEnd ?? string.Empty);
                                staticDaysCommand.Parameters.AddWithValue("@LunchTimeBegin", day.LunchTimeBegin ?? string.Empty);
                                staticDaysCommand.Parameters.AddWithValue("@LunchTimeEnd", day.LunchTimeEnd ?? string.Empty);
                                staticDaysCommand.Parameters.AddWithValue("@Template_ID", templateId);
                                staticDaysCommand.Parameters.AddWithValue("@isRestingDay", day.isRestingDay);

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

            MessageBox.Show("График успешно добавлен в базу данных.");
            TemplateCreated?.Invoke(this, EventArgs.Empty);
        }

        private void AddFlexibleTemplateBtn_Click(object sender, RoutedEventArgs e)
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
                int checkExisting = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT 1 FROM Zarplats.dbo.Schedule_Template WHERE TemplateName LIKE @TemplateName) SELECT 1 ELSE SELECT 0", new SqlParameter("TemplateName", $"%{TemplateNameTBX.Text}%")).SingleOrDefault();
                if (!Convert.ToBoolean(checkExisting))
                {
                    // Создание объекта Schedule_Template и вставка в базу данных
                    using (SqlCommand command = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_Template (TemplateName, isFlexible, RestingDaysCount, WorkingDaysCount) VALUES (@TemplateName, @isFlexible, @RestingDaysCount, @WorkingDaysCount); SELECT SCOPE_IDENTITY();", connection))
                    {
                        command.Parameters.AddWithValue("@TemplateName", $"{TemplateNameTBX.Text} {TemplateAdditionalNameTBX.Text}");
                        command.Parameters.AddWithValue("@isFlexible", true);
                        command.Parameters.AddWithValue("@RestingDaysCount", RestingDaysCountCMB.SelectedValue);
                        command.Parameters.AddWithValue("@WorkingDaysCount", WorkingDaysCountCMB.SelectedValue);

                        int templateId = Convert.ToInt32(command.ExecuteScalar());

                        // Создание объекта Schedule_FlexibleDays для каждого дня и вставка в базу данных
                        foreach (var day in FlexibleDays)
                        {
                            using (SqlCommand flexibleDaysCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Schedule_FlexibleDays (WorkBegin, WorkEnd, LunchTimeBegin, LunchTimeEnd, Template_ID) VALUES (@WorkBegin, @WorkEnd, @LunchTimeBegin, @LunchTimeEnd, @Template_ID);", connection))
                            {
                                flexibleDaysCommand.Parameters.AddWithValue("@WorkBegin", day.WorkBegin ?? string.Empty);
                                flexibleDaysCommand.Parameters.AddWithValue("@WorkEnd", day.WorkEnd ?? string.Empty);
                                flexibleDaysCommand.Parameters.AddWithValue("@LunchTimeBegin", day.LunchTimeBegin ?? string.Empty);
                                flexibleDaysCommand.Parameters.AddWithValue("@LunchTimeEnd", day.LunchTimeEnd ?? string.Empty);
                                flexibleDaysCommand.Parameters.AddWithValue("@Template_ID", templateId);

                                flexibleDaysCommand.ExecuteNonQuery();
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

            MessageBox.Show("График успешно добавлен в базу данных.");
            TemplateCreated?.Invoke(this, EventArgs.Empty);
        }

        private void MaskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null && string.IsNullOrEmpty(maskedTextBox.Text))
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.CaretIndex = 0; }));
            }
        }

        private void WorkingDaysCountCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduleTB.SelectedIndex == 1)
            {
                TemplateAdditionalNameTBX.Text = $"({WorkingDaysCountCMB.SelectedValue}/{RestingDaysCountCMB.SelectedValue})";
                UpdateRestingDaysComboBox();
                UpdateFlexibleDaysCollection();
            }
        }

        private void RestingDaysCountCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduleTB.SelectedIndex == 1)
            {
                TemplateAdditionalNameTBX.Text = $"({WorkingDaysCountCMB.SelectedValue}/{RestingDaysCountCMB.SelectedValue})";
                UpdateFlexibleDaysCollection();
            }
        }

        public void UpdateFlexibleDaysCollection()
        {
            int workingDaysCount = WorkingDaysCountCMB.SelectedIndex + 1;

            FlexibleDays.Clear();

            for (int i = 0; i < workingDaysCount; i++)
            {
                FlexibleDays.Add(new ScheduleTemplateModel
                {
                    Day = $"День {i + 1}"
                });
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

                    UpdateWorkingAndRestingDaysCount();
                }
            }
        }

        private void isRestingDayCB_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                ScheduleTemplateModel day = checkBox.DataContext as ScheduleTemplateModel;

                if (day != null)
                {
                    day.WorkBegin = "08:00";
                    day.WorkEnd = "17:00";
                    day.LunchTimeBegin = "12:00";
                    day.LunchTimeEnd = "13:00";
                    day.isRestingDay = false;

                    UpdateWorkingAndRestingDaysCount();
                }
            }
        }

        private void UpdateWorkingAndRestingDaysCount()
        {
            int workingDaysCount = StaticDays.Count(day => !day.isRestingDay);
            int restingDaysCount = StaticDays.Count(day => day.isRestingDay);

            TemplateAdditionalNameTBX.Text = $"({workingDaysCount}/{restingDaysCount})";
        }

        private void MTBX_KeyDown(object sender, KeyEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                string textToCopy = maskedTextBox.Text;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        Clipboard.SetText(textToCopy);
                        return;
                    }
                    catch { }
                    System.Threading.Thread.Sleep(10);
                }
            }
        }
    }
}
