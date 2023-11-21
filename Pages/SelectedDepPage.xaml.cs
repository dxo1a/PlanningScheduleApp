using MathCore.WPF.Converters;
using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPage : System.Windows.Controls.Page
    {
        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";

        List<StaffModel> StaffList = new List<StaffModel>();
        List<StaffModel> StaffListInPosition = new List<StaffModel>();

        List<ScheduleTemplateModel> ScheduleTemplateList = new List<ScheduleTemplateModel>();

        List<AbsenceModel> CauseList = new List<AbsenceModel>();

        List<string> FullDayCauseList = new List<string> { "Больничный", "Отгул", "Прогул", "Отпуск" };

        DepModel SelectedDep { get; set; }
        StaffModel SelectedStaffInDG { get; set; }
        StaffModel SelectedStaff { get; set; }
        ScheduleTemplateModel SelectedTemplate { get; set; }
        AbsenceModel SelectedCause { get; set; }

        public double WorkingHours, CalculatedLunchTime, LunchTime;

        public SelectedDepPage(DepModel selectedDep)
        {
            InitializeComponent();
            SelectedDep = selectedDep;
            FrameApp.SetCurrentMainFrame(FrameApp.FrameMain);
            InitializeAsync();
            AssignCMB();
            UpdateTemplatesList();

            StaffListInPosition = Odb.db.Database.SqlQuery<StaffModel>("select distinct b.SHORT_FIO, b.TABEL_ID, b.ID_STAFF as STAFF_ID from perco...staff_ref as a left join perco...staff as b on a.STAFF_ID = b.ID_STAFF left join perco...subdiv_ref as c on a.SUBDIV_ID = c.ID_REF where c.DISPLAY_NAME = @padrazd", new SqlParameter("padrazd", SelectedDep.Position)).OrderBy(s => s.SHORT_FIO).ToList();
            StaffLV.ItemsSource = StaffListInPosition;
            CauseList = Odb.db.Database.SqlQuery<AbsenceModel>("select distinct * from Zarplats.dbo.AbsenceRef").ToList();
            CauseLV.ItemsSource = CauseList;

            AbsenceStartDP.SelectedDate = DateTime.Now;
            AbsenceFinishDP.SelectedDate = DateTime.Now;
        }

        private async void InitializeAsync()
        {
            await UpdateGridAsync();
        }

        public void UpdateTemplatesList()
        {
            ScheduleTemplateList = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct ID_Template, TemplateName, isFlexible, RestingDaysCount, WorkingDaysCount from Zarplats.dbo.Schedule_Template as a").OrderBy(u => u.TemplateName).ToList();
            if (ScheduleTemplateList.Count <= 0)
            {
                TemplateCB.SelectedItem = null;
                TemplateCB.IsEnabled = false;
            }
            else
            {
                TemplateCB.SelectedIndex = 0;
                TemplateCB.IsEnabled = true;
                TemplateCB.ItemsSource = ScheduleTemplateList;
            }
        }

        int refreshCount = 0;
        public async Task UpdateGridAsync()
        {
            SearchTBX.Clear();
            StaffList.Clear();
            StaffDG.ItemsSource = null;

            string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA", connection))
                {
                    command.Parameters.AddWithValue("@podrazd", SelectedDep.Position);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        /* Проверка типа переменной в бд и в сущности
                         * Console.WriteLine($"Type of column 9: {reader.GetFieldType(9)}");
                        Console.WriteLine($"Type of WorkingHours property: {typeof(StaffModel).GetProperty("WorkingHours").PropertyType}");*/
                        List<StaffModel> staffList = new List<StaffModel>();
                        while (await reader.ReadAsync())
                        {
                            StaffModel staff = new StaffModel
                            {
                                ID_Schedule = reader.GetInt32(0),
                                STAFF_ID = reader.GetInt32(1),
                                TABEL_ID = reader.GetString(2),
                                SHORT_FIO = reader.GetString(3),
                                WorkBegin = reader.GetString(4),
                                WorkEnd = reader.GetString(5),
                                DTA = reader.GetDateTime(6),
                                LunchTimeBegin = reader.GetString(7),
                                LunchTimeEnd = reader.GetString(8),
                                WorkingHours = reader.GetDouble(9),
                                ID_Absence = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                CauseAbsence = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                DateBegin = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                                DateEnd = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                                TimeBegin = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                TimeEnd = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                            };
                            staffList.Add(staff);
                        }

                        StaffList = staffList;
                        StaffDG.ItemsSource = StaffList;
                    }
                }
            }
            refreshCount++;
            Console.WriteLine($"Таблица StaffDG обновлена {refreshCount} раз.");
        }

        private void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckWhatToAdd() == "1100")
            {
                if (SelectedTemplate.isFlexible)
                    FillFlexibleSchedule();
                else if (!SelectedTemplate.isFlexible)
                    FillStaticSchedule();
            }
            else if (CheckWhatToAdd() == "1011")
            {
                int checkExistingSchedule = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where DTA between @DateBegin and @DateEnd", new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate)).SingleOrDefault();
                if (checkExistingSchedule > 0)
                    AddAbsence();
                else
                    MessageBox.Show("Для указанного диапазона не найдено рабочих дней!");
            }
            else if (CheckWhatToAdd() == "1111")
            {
                int checkExistingSchedule = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where DTA between @DateBegin and @DateEnd", new SqlParameter("DateBegin", ScheduleStartDP.SelectedDate), new SqlParameter("DateEnd", ScheduleEndDP.SelectedDate)).SingleOrDefault();
                if (checkExistingSchedule <= 0)
                {
                    if (SelectedTemplate.isFlexible)
                    {
                        FillFlexibleSchedule();
                        AddAbsence();
                    }
                    else if (!SelectedTemplate.isFlexible)
                    {
                        FillStaticSchedule();
                        AddAbsence();
                    }
                }
                else
                {
                    MessageBox.Show("График уже существует, добавляем только отсутствие.");
                    AddAbsence();
                }
            }
            else if (CheckWhatToAdd() == "##0#")
            {
                MessageBox.Show("Укажите причину отсутствия.");
                CauseLV.Focus();
            }
            else if (CheckWhatToAdd() == "1608")
                MessageBox.Show("Ошибка 1608. Обратитесь к разработчику.");
            else if (CheckWhatToAdd() == "0###")
            {
                MessageBox.Show("Выберите сотрудника.");
                StaffLV.Focus();
            }
            else if (CheckWhatToAdd() == "#0##")
                MessageBox.Show("Не все поля графика заполнены.");
            else if (CheckWhatToAdd() == "###0")
                MessageBox.Show($"Не все поля отсутствия заполнены.\nTimeStart: {AbsenceTimeBeginMTBX.Text}, TimeEnd: {AbsenceTimeEndMTBX.Text}");
        }

        public async void DeleteRow()
        {
            List<StaffModel> selectedItems = StaffDG.SelectedItems.Cast<StaffModel>().ToList();
            if (selectedItems.Count > 1)
            {
                var result = MessageBox.Show("Удалить записи?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var result2 = MessageBox.Show("Удалить отсутствия?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result2 == MessageBoxResult.Yes)
                        DeleteAbsence();

                    foreach (StaffModel selectedRow in selectedItems)
                    {
                        Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", selectedRow.ID_Schedule));
                    }
                }
                await UpdateGridAsync();
            }
            else if (selectedItems.Count == 1)
            {
                var result = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", SelectedStaffInDG.ID_Schedule));
                }
                await UpdateGridAsync();
            }
            else
            {
                MessageBox.Show("Выберите записи для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteAbsenceMI_Click(object sender, RoutedEventArgs e)
        {
            DeleteAbsence();
            await UpdateGridAsync();
        }

        #region UI
        private void ExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelFilterWindow exportToExcelFilterWindow = new ExportToExcelFilterWindow();
            exportToExcelFilterWindow.ShowDialog();
        }
        private void StaffDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteRow();
            }
        }

        private void StaffTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Visible;
        }

        private void StaffTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility = Visibility.Collapsed;
        }

        private void StaffTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchStaff();
        }

        private void StaffLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffLV.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffTBX.Text = $"{SelectedStaff.SHORT_FIO} ({SelectedStaff.TABEL_ID.Trim()})";
            }
        }

        private void TemplateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTemplate = (ScheduleTemplateModel)TemplateCB.SelectedItem;
        }

        private void TemplateCB_DropDownOpened(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            StaffTBX.Clear();
            StaffLV.SelectedItem = null;
            SelectedStaff = null;
        }

        private void ManageScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            ScheduleManageWindow scheduleManageWindow = new ScheduleManageWindow();
            scheduleManageWindow.TemplateCreated += ScheduleManageWindow_TemplateCreated;
            scheduleManageWindow.TemplateDeleted += ScheduleManageWindow_TemplateDeleted;
            scheduleManageWindow.ShowDialog();
        }

        private void ScheduleManageWindow_TemplateCreated(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void ScheduleManageWindow_TemplateDeleted(object sender, EventArgs e)
        {
            UpdateTemplatesList();
        }

        private void StaffRemoveBtn_Click(object sender, RoutedEventArgs e) => DeleteRow();

        private void StaffDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaffInDG = (StaffModel)StaffDG.SelectedItem;
            if (SelectedStaffInDG != null)
            {
                StaffRemoveBtn.IsEnabled = true;
            }
            else
            {
                StaffRemoveBtn.IsEnabled = false;
            }
        }

        private async void StaffRefreshBtn_Click(object sender, RoutedEventArgs e) => await UpdateGridAsync();

        #region Search Functionality
        #region Search In DataGrid
        private void SearchTBX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SearchInDG();

        private void SearchInDG()
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                staff = StaffList;

            switch (filterCMB.SelectedIndex)
            {
                case 0:
                    staff = StaffList.Where(u => u.SHORT_FIO.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 1:
                    staff = StaffList.Where(u => u.TABEL_ID.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 2:
                    staff = StaffList.Where(u => u.DTA.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 3:
                    staff = StaffList.Where(u => u.WorkingHours.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                default:
                    staff = StaffList.Where(u => u.StaffForSearch.ToLower().Contains(txt.ToLower())).ToList();
                    break;

            };
            StaffDG.ItemsSource = staff;
        }

        private void filterCMB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SearchTBX.Clear();

        public void AssignCMB()
        {
            filterCMB.ItemsSource = new filterCMB[]
            {
                new filterCMB { id = 0, filterName = "ФИО" },
                new filterCMB { id = 1, filterName = "табельному номеру" },
                new filterCMB { id = 2, filterName = "дате" },
                new filterCMB { id = 3, filterName = "рабочим часам" }
            };
            filterCMB.SelectedIndex = 0;
        }
        #endregion

        #region Search Staff
        private void SearchStaff()
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = StaffTBX.Text;
            if (txt.Length == 0)
                staff = StaffList;
            staff = StaffListInPosition.Where(u => u.StaffForSearch.ToLower().Contains(txt.ToLower())).ToList();
            StaffLV.ItemsSource = staff;
        }
        #endregion
        #endregion

        private void CauseTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            CauseLV.Visibility = Visibility.Visible;
        }

        private void CauseTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            CauseLV.Visibility = Visibility.Collapsed;
        }

        private void CauseLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCause = (AbsenceModel)CauseLV.SelectedItem;
            if (SelectedCause != null)
                CauseTBX.Text = SelectedCause.Cause;
        }

        private void ClearAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            CauseLV.SelectedItem = null;
            CauseTBX.Clear();
            AbsenceStartDP.SelectedDate = null;
            AbsenceFinishDP.SelectedDate = null;
            AbsenceTimeBeginMTBX.Clear();
            AbsenceTimeEndMTBX.Clear();
        }

        private string CheckWhatToAdd() // # - сотрудник, # - поля графика, # - отсутствие, # - поля отсутствия
        {
            if (StaffLV.SelectedItem != null)
            {
                if (FieldIsFilled("staff") && CauseLV.SelectedItem == null)   // если поля сотрудника заполнены, но отсутствие не выбрано, то добавляем график
                    return "1100";
                else if (FieldIsFilled("staff") == false && CauseLV.SelectedItem != null && FieldIsFilled("absence"))  // если выбран сотрудник, но его поля графика не заполнены и поля отсутствия заполнены, то добавляем только отсутствие
                    return "1011";
                else if (FieldIsFilled("staff") && CauseLV.SelectedItem != null && FieldIsFilled("absence"))  // если поля сотрудника и отсутствия заполнены
                    return "1111";
                else if (CauseLV.SelectedItem == null)
                    return "##0#";   // причина отсутствия не выбрана
                else if (!FieldIsFilled("staff"))
                    return "#0##";
                else if (!FieldIsFilled("absence"))
                    return "###0";
                else
                    return "1608";     // сотрудник выбран, но неизвестная ошибка - отладить код
            }
            else return "0###";  // сотрудник не выбран
        }

        private bool FieldIsFilled(string whichFields)
        {
            if (whichFields == "staff")
            {
                if (TemplateCB.SelectedItem == null || ScheduleStartDP.SelectedDate == null || ScheduleEndDP.SelectedDate == null)
                    return false;
                return true;
            }
            else if (whichFields == "absence")
            {
                if (AbsenceStartDP.SelectedDate == null || AbsenceFinishDP.SelectedDate == null)
                    return false;
                return true;
            }
            else return false;
        }

        private void DP_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                if (DateTime.TryParse(datePicker.Text, out DateTime selectedDate))
                    datePicker.SelectedDate = selectedDate;
            }
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
        
        private void StaffDG_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // получение элемента под указателем мыши
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row)
            {
                StaffDG.SelectedItem = row;
                StaffModel selectedStaff = (StaffModel)row.Item;

                if (!HasAbsence(selectedStaff.STAFF_ID, selectedStaff.DTA))
                {
                    e.Handled = true;
                    DGCM.IsOpen = false;
                    DGCM.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DGCM.IsOpen = true;
                    DGCM.Visibility = Visibility.Visible;
                }
            }
        }

        private bool HasAbsence(int staffId, DateTime date)
        {
            StaffModel absence = Odb.db.Database.SqlQuery<StaffModel>("select * from Zarplats.dbo.Schedule_Absence where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date", new SqlParameter("date", date), new SqlParameter("idstaff", staffId)).FirstOrDefault();
            if (absence != null)
                return true;
            else
                return false;
        }

        private void AbsenceStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AbsenceFinishDP.IsEnabled = true;
            AbsenceFinishDP.DisplayDateStart = AbsenceStartDP.SelectedDate;
        }

        private void ScheduleStartDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ScheduleEndDP.IsEnabled = true;
            ScheduleEndDP.DisplayDateStart = ScheduleStartDP.SelectedDate;
        }
        #endregion

        #region Заполнение графиков
        private async void FillFlexibleSchedule()
        {
            List<ScheduleTemplateModel> flexibleDays = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct * from Zarplats.dbo.Schedule_FlexibleDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template)).ToList();

            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleEndDP.SelectedDate ?? DateTime.Now;
            DateTime current = selectedStartDate;

            int flexibleDaysIndex = 0;

            while (current <= selectedFinishDate)
            {
                if (flexibleDaysIndex >= flexibleDays.Count)
                {
                    flexibleDaysIndex = 0;
                    current = current.AddDays(SelectedTemplate.RestingDaysCount);
                }

                var flexibleDay = flexibleDays[flexibleDaysIndex];

                DateTime workBegin = ConvertToDateTime(current, flexibleDay.WorkBegin);
                DateTime workEnd = ConvertToDateTime(current, flexibleDay.WorkEnd);
                DateTime lunchTimeBegin = ConvertToDateTime(current, flexibleDay.LunchTimeBegin);
                DateTime lunchTimeEnd = ConvertToDateTime(current, flexibleDay.LunchTimeEnd);

                double totalAbsenceTime = await CalculateAbsenceHoursForEachDay(SelectedStaff.STAFF_ID, current);
                double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - totalAbsenceTime;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Проверка существования записи в графике
                    using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                        checkCommand.Parameters.AddWithValue("@date", current.Date);

                        int existingCount = (int)checkCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            // Запись существует, обновление
                            using (SqlCommand updateCommand = new SqlCommand("UPDATE Zarplats.dbo.Staff_Schedule SET WorkBegin = @workBegin, WorkEnd = @workEnd, LunchTimeBegin = @lunchTimeBegin, LunchTimeEnd = @lunchTimeEnd, WorkingHours = @workingHours WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                            {
                                updateCommand.Parameters.AddWithValue("@workBegin", flexibleDay.WorkBegin);
                                updateCommand.Parameters.AddWithValue("@workEnd", flexibleDay.WorkEnd);
                                updateCommand.Parameters.AddWithValue("@lunchTimeBegin", flexibleDay.LunchTimeBegin);
                                updateCommand.Parameters.AddWithValue("@lunchTimeEnd", flexibleDay.LunchTimeEnd);
                                updateCommand.Parameters.AddWithValue("@workingHours", workingHours);
                                updateCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                updateCommand.Parameters.AddWithValue("@date", current.Date);

                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Записи нет, добавление новой записи
                            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule (WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTimeBegin, LunchTimeEnd, WorkingHours) VALUES (@workBegin, @workEnd, @date, @staffId, @lunchTimeBegin, @lunchTimeEnd, @workingHours)", connection))
                            {
                                insertCommand.Parameters.AddWithValue("@workBegin", flexibleDay.WorkBegin);
                                insertCommand.Parameters.AddWithValue("@workEnd", flexibleDay.WorkEnd);
                                insertCommand.Parameters.AddWithValue("@date", current.Date);
                                insertCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                insertCommand.Parameters.AddWithValue("@lunchTimeBegin", flexibleDay.LunchTimeBegin);
                                insertCommand.Parameters.AddWithValue("@lunchTimeEnd", flexibleDay.LunchTimeEnd);
                                insertCommand.Parameters.AddWithValue("@workingHours", workingHours);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                flexibleDaysIndex++;
                current = current.AddDays(1);
            }
            MessageBox.Show($"График заполнен!");
            await UpdateGridAsync();
        }

        private async void FillStaticSchedule()
        {
            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleEndDP.SelectedDate ?? DateTime.Now;
            DateTime current = selectedStartDate;

            List<ScheduleTemplateModel> Days = GetDaysInfo(SelectedTemplate.ID_Template);

            DayOfWeek startDayOfWeek = selectedStartDate.DayOfWeek;
            var currentDay = Days.FirstOrDefault(d => d.Day == startDayOfWeek.ToString());

            while (current <= selectedFinishDate.AddDays(1))
            {
                if (currentDay != null && !currentDay.isRestingDay)
                {
                    DateTime workBegin = ConvertToDateTime(current, currentDay.WorkBegin);
                    DateTime workEnd = ConvertToDateTime(current, currentDay.WorkEnd);
                    DateTime lunchTimeBegin = ConvertToDateTime(current, currentDay.LunchTimeBegin);
                    DateTime lunchTimeEnd = ConvertToDateTime(current, currentDay.LunchTimeEnd);

                    double totalAbsenceTime = await CalculateAbsenceHoursForEachDay(SelectedStaff.STAFF_ID, current);
                    double workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, current) - totalAbsenceTime;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Проверка существования записи в графике
                        using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                        {
                            checkCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                            checkCommand.Parameters.AddWithValue("@date", current.Date);

                            int existingCount = (int)checkCommand.ExecuteScalar();

                            if (existingCount > 0)
                            {
                                // Запись существует, обновление
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Zarplats.dbo.Staff_Schedule SET WorkBegin = @workBegin, WorkEnd = @workEnd, LunchTimeBegin = @lunchTimeBegin, LunchTimeEnd = @lunchTimeEnd, WorkingHours = @workingHours WHERE STAFF_ID = @staffId AND DTA = @date", connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@workBegin", currentDay.WorkBegin);
                                    updateCommand.Parameters.AddWithValue("@workEnd", currentDay.WorkEnd);
                                    updateCommand.Parameters.AddWithValue("@lunchTimeBegin", currentDay.LunchTimeBegin);
                                    updateCommand.Parameters.AddWithValue("@lunchTimeEnd", currentDay.LunchTimeEnd);
                                    updateCommand.Parameters.AddWithValue("@workingHours", workingHours);
                                    updateCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                    updateCommand.Parameters.AddWithValue("@date", current.Date);

                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Записи нет, добавление новой записи
                                using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule (WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTimeBegin, LunchTimeEnd, WorkingHours) VALUES (@workBegin, @workEnd, @date, @staffId, @lunchTimeBegin, @lunchTimeEnd, @workingHours)", connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@workBegin", currentDay.WorkBegin);
                                    insertCommand.Parameters.AddWithValue("@workEnd", currentDay.WorkEnd);
                                    insertCommand.Parameters.AddWithValue("@date", current.Date);
                                    insertCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                    insertCommand.Parameters.AddWithValue("@lunchTimeBegin", currentDay.LunchTimeBegin);
                                    insertCommand.Parameters.AddWithValue("@lunchTimeEnd", currentDay.LunchTimeEnd);
                                    insertCommand.Parameters.AddWithValue("@workingHours", workingHours);

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                current = current.AddDays(1);
                currentDay = Days.FirstOrDefault(d => d.Day == current.DayOfWeek.ToString());   // переход к следующему дню недели в записях из базы данных
            }
            MessageBox.Show($"График заполнен!");
            await UpdateGridAsync();
        }

        public List<ScheduleTemplateModel> GetDaysInfo(int templateid) // информация о каждом дне в статик таблице
        {
            List<ScheduleTemplateModel> staticDaysList = new List<ScheduleTemplateModel>();
            staticDaysList = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct * from Zarplats.dbo.Schedule_StaticDays where Template_ID = @templateid", new SqlParameter("templateid", templateid)).ToList();
            return staticDaysList;
        }

        public DateTime ConvertToDateTime(DateTime date, string time)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return new DateTime(date.Year, date.Month, date.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            }
            return DateTime.MinValue;
        }
        #endregion

        #region Работа с отсутствиями
        private async void AddAbsence()
        {
            string timeBeginValue = AbsenceTimeBeginMTBX.Text.Any(char.IsDigit) ? AbsenceTimeBeginMTBX.Text : string.Empty;
            string timeEndValue = AbsenceTimeEndMTBX.Text.Any(char.IsDigit) ? AbsenceTimeEndMTBX.Text : string.Empty;

            int checkExistingAbsence = Odb.db.Database.SqlQuery<int>("IF EXISTS (SELECT * FROM Zarplats.dbo.Schedule_Absence WHERE DateBegin <= @DateEnd AND DateEnd >= @DateBegin AND id_Staff = @staffid) SELECT 1 ELSE SELECT 0", new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate), new SqlParameter("staffid", SelectedStaff.STAFF_ID)).SingleOrDefault();
            if (!Convert.ToBoolean(checkExistingAbsence))
            {
                Odb.db.Database.ExecuteSqlCommand("INSERT INTO Zarplats.dbo.Schedule_Absence (AbsenceRef_ID, id_Staff, DateBegin, DateEnd, TimeBegin, TimeEnd) VALUES (@AbsenceRef_ID, @staffid, @DateBegin, @DateEnd, @TimeBegin, @TimeEnd)",
                    new SqlParameter("AbsenceRef_ID", SelectedCause.ID_AbsenceRef), new SqlParameter("staffid", SelectedStaff.STAFF_ID), new SqlParameter("DateBegin", AbsenceStartDP.SelectedDate), new SqlParameter("DateEnd", AbsenceFinishDP.SelectedDate), new SqlParameter("TimeBegin", timeBeginValue), new SqlParameter("TimeEnd", timeEndValue));

                UpdateWorkingHours();

                await UpdateGridAsync();
                MessageBox.Show("Отсутствие добавлено!");
            }
            else
            {
                MessageBox.Show("В указанном периоде уже существует отсутствие!");
            }
        }

        private void DeleteAbsence()
        {
            List<StaffModel> selectedItems = new List<StaffModel>();
            selectedItems.AddRange(StaffDG.SelectedItems.Cast<StaffModel>());

            foreach (var selectedStaff in selectedItems)
                Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE ID_Absence = @ID_Absence AND DateBegin <= @currentDate AND DateEnd >= @currentDate", new SqlParameter("ID_Absence", selectedStaff.ID_Absence), new SqlParameter("currentDate", selectedStaff.DTA));
            UpdateWorkingHours();
        }
        #endregion

        #region Вычисления рабочих часов
        private async Task UpdateWorkingHours()
        {
            DateTime startDate = AbsenceStartDP.SelectedDate ?? DateTime.Now;
            DateTime endDate = AbsenceFinishDP.SelectedDate ?? DateTime.Now;

            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                List<StaffModel> affectedRows = await GetAffectedRowsAsync(SelectedDep.Position, currentDate);
                foreach (StaffModel row in affectedRows)
                {
                    if (FullDayCauseList.Contains(row.CauseAbsence))
                    {
                        await Odb.db.Database.ExecuteSqlCommandAsync("update Zarplats.dbo.Staff_Schedule set WorkingHours = 0 where ID_Schedule = @id", new SqlParameter("id", row.ID_Schedule));
                    }
                    else
                    {
                        DateTime workBegin = ConvertToDateTime(currentDate, row.WorkBegin);
                        DateTime workEnd = ConvertToDateTime(currentDate, row.WorkEnd);
                        DateTime lunchTimeBegin = ConvertToDateTime(currentDate, row.LunchTimeBegin);
                        DateTime lunchTimeEnd = ConvertToDateTime(currentDate, row.LunchTimeEnd);

                        double totalAbsenceTime = await CalculateAbsenceHoursForEachDay(row.STAFF_ID, currentDate);
                        double? workingHours = CalculateWorkingHours(workBegin, workEnd, lunchTimeBegin, lunchTimeEnd, row.DTA) - totalAbsenceTime;

                        await Odb.db.Database.ExecuteSqlCommandAsync("update Zarplats.dbo.Staff_Schedule set WorkingHours = @workingHours where ID_Schedule = @id", new SqlParameter("workingHours", workingHours), new SqlParameter("id", row.ID_Schedule));
                    }
                }
            }
        }

        private async Task<List<StaffModel>> GetAffectedRowsAsync(string position, DateTime currentDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = $"SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd and a.DTA = @date order by a.DTA";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@podrazd", SelectedDep.Position);
                    command.Parameters.AddWithValue("@date", currentDate);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var affectedRows = new List<StaffModel>();

                        while (await reader.ReadAsync())
                        {
                            StaffModel staff = new StaffModel
                            {
                                ID_Schedule = reader.GetInt32(0),
                                STAFF_ID = reader.GetInt32(1),
                                TABEL_ID = reader.GetString(2),
                                SHORT_FIO = reader.GetString(3),
                                WorkBegin = reader.GetString(4),
                                WorkEnd = reader.GetString(5),
                                DTA = reader.GetDateTime(6),
                                LunchTimeBegin = reader.GetString(7),
                                LunchTimeEnd = reader.GetString(8),
                                WorkingHours = reader.GetDouble(9),
                                ID_Absence = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                CauseAbsence = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                DateBegin = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                                DateEnd = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                                TimeBegin = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                TimeEnd = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                            };
                        }

                        return affectedRows;
                    }
                }
            }
        }

        private double CalculateWorkingHours(DateTime workBegin, DateTime workEnd, DateTime lunchTimeBegin, DateTime lunchTimeEnd, DateTime date)
        {
            double totalWorkingHours = (workEnd - workBegin).TotalHours;
            double lunchTime = (lunchTimeEnd - lunchTimeBegin).TotalHours;
            //double absenceHours = CalculateAbsenceHours(SelectedStaff.STAFF_ID, date, date);

            return totalWorkingHours - lunchTime;
        }

        private async Task<double> CalculateAbsenceHoursForEachDay(int staffId, DateTime currentDate)
        {
            double totalAbsenceTime = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT TimeBegin, TimeEnd FROM Zarplats.dbo.Schedule_Absence WHERE id_Staff = @staffId AND DateBegin <= @currentDate AND DateEnd >= @currentDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@staffId", staffId);
                    command.Parameters.AddWithValue("@currentDate", currentDate);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime absenceTimeBegin = ConvertToDateTime(currentDate, reader["TimeBegin"].ToString());
                            DateTime absenceTimeEnd = ConvertToDateTime(currentDate, reader["TimeEnd"].ToString());

                            totalAbsenceTime += (absenceTimeEnd - absenceTimeBegin).TotalHours;
                        }
                    }
                }
            }

            return totalAbsenceTime;
        }
        #endregion
    }

    public class filterCMB
    {
        public int id { get; set; }
        public string filterName { get; set; } = "";
        public override string ToString() => $"{filterName}";
    }
}
