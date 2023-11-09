using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPage : System.Windows.Controls.Page
    {
        List<StaffModel> StaffList = new List<StaffModel>();
        List<StaffModel> StaffListInPosition = new List<StaffModel>();
        List<ScheduleTemplateModel> ScheduleTemplateList = new List<ScheduleTemplateModel>();

        DepModel SelectedDep { get; set; }
        StaffModel SelectedStaffInDG { get; set; }
        StaffModel SelectedStaff { get; set; }

        public double WorkingHours, CalculatedLunchTime, LunchTime;
        DateTime combinedStartDateTime, combinedFinishDateTime;

        public SelectedDepPage(DepModel selectedDep)
        {
            InitializeComponent();
            SelectedDep = selectedDep;
            FrameApp.SetCurrentMainFrame(FrameApp.FrameMain);
            UpdateGrid();
            AssignCMB();

            StaffListInPosition = Odb.db.Database.SqlQuery<StaffModel>("select distinct b.SHORT_FIO, b.TABEL_ID, b.ID_STAFF as STAFF_ID from perco...staff_ref as a left join perco...staff as b on a.STAFF_ID = b.ID_STAFF left join perco...subdiv_ref as c on a.SUBDIV_ID = c.ID_REF where c.DISPLAY_NAME = @padrazd", new SqlParameter("padrazd", SelectedDep.Position)).OrderBy(s => s.SHORT_FIO).ToList();
            StaffLV.ItemsSource = StaffListInPosition;
            ScheduleTemplateList = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select distinct TemplateName from Zarplats.dbo.Schedule_Template").ToList();
            TemplateCB.ItemsSource = ScheduleTemplateList;
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

        private void StaffRefreshBtn_Click(object sender, RoutedEventArgs e) => UpdateGrid();

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

        private void ExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelFilterWindow exportToExcelFilterWindow = new ExportToExcelFilterWindow();
            exportToExcelFilterWindow.ShowDialog();
        }

        public void DeleteRow()
        {
            List<StaffModel> selectedItems = StaffDG.SelectedItems.Cast<StaffModel>().ToList();
            if (selectedItems.Count > 1)
            {
                var result = MessageBox.Show("Удалить записи?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (StaffModel selectedStaff in selectedItems)
                    {
                        Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", selectedStaff.ID_Schedule));
                    }
                }
                UpdateGrid();
            }
            else if (selectedItems.Count == 1)
            {
                var result = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE ID_Schedule = @idschedule", new SqlParameter("idschedule", SelectedStaffInDG.ID_Schedule));
                }
            }
            else
            {
                MessageBox.Show("Выберите записи для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StaffDG_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteRow();
            }
        }

        private void UpdateGrid()
        {
            StaffList = Odb.db.Database.SqlQuery<StaffModel>("SELECT DISTINCT a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTime, a.WorkingHours, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, d.Cause as CauseTimeOff, d.TimeBegin, d.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join Zarplats.dbo.Schedule_TimeOff as d on a.STAFF_ID = d.id_Staff and a.DTA = d.DTA left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA", new SqlParameter("podrazd", SelectedDep.Position)).ToList();
            StaffDG.ItemsSource = StaffList;
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

        private void AddScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (isRequiredFieldsNotEmpty())
            {
                ParseToDateTime();
                AddSchedules();
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!");
            }
            */

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
            scheduleManageWindow.ShowDialog();
        }

        /*
        private void ParseToDateTime()
        {
            string startTimeText = StartTimeMTBX.Text;
            string finishTimeText = FinishTimeMTBX.Text;

            DateTime startDate = ScheduleStartDP.SelectedDate ?? DateTime.Now.Date;
            DateTime finishDate = ScheduleFinishDP.SelectedDate ?? DateTime.Now.Date;

            // Преобразование в double для высчитывания рабочих часов
            TimeSpan startTimeSpan = TimeSpan.Parse(startTimeText);
            TimeSpan finishTimeSpan = TimeSpan.Parse(finishTimeText);
            double startTotalHours = startTimeSpan.TotalHours;
            double finishTotalHours = finishTimeSpan.TotalHours;

            WorkingHours = finishTotalHours - startTotalHours;
            if (double.TryParse(LunchTimeTBX.Text, out LunchTime))
            {
                WorkingHours = WorkingHours - LunchTime;
            }
            

            // Преобразование string в DateTime и объединение времени с датой
            if (DateTime.TryParseExact(startTimeText, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartTime))
            {
                combinedStartDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, parsedStartTime.Hour, parsedStartTime.Minute, 0);
            }

            if (DateTime.TryParseExact(finishTimeText, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFinishTime))
            {
                combinedFinishDateTime = new DateTime(finishDate.Year, finishDate.Month, finishDate.Day, parsedFinishTime.Hour, parsedFinishTime.Minute, 0);
            }
        }

        private void AddSchedules()
        {
            DateTime selectedStartDate = ScheduleStartDP.SelectedDate ?? DateTime.Now;
            DateTime selectedFinishDate = ScheduleFinishDP.SelectedDate ?? DateTime.Now;
            DateTime currentDay = selectedStartDate;

            #region
            int count = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where DTA between @startDate and @finishDate and STAFF_ID = @staffid",
                new SqlParameter("startDate", selectedStartDate), new SqlParameter("finishDate", selectedFinishDate), new SqlParameter("staffid", SelectedStaff.STAFF_ID)).FirstOrDefault();
            if (count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Записи на некоторые дни уже существуют. Заменить их?");
                if (result == MessageBoxResult.OK)
                {
                    // Удалить существующие записи в заданном диапазоне дат
                    Odb.db.Database.ExecuteSqlCommand("DELETE FROM Zarplats.dbo.Staff_Schedule WHERE DTA BETWEEN @startDate AND @finishDate AND STAFF_ID = @staffid",
                        new SqlParameter("startDate", selectedStartDate), new SqlParameter("finishDate", selectedFinishDate), new SqlParameter("staffid", SelectedStaff.STAFF_ID));
                }
                else
                {
                    // Отменить добавление новых записей
                    return;
                }
            }
            #endregion

            if (TemplateCB.SelectedIndex == 1)
            {
                while (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                {
                    currentDay = currentDay.AddDays(1);
                }

                while (currentDay <= selectedFinishDate)
                {
                    // Проверка, является ли текущий день рабочим днем (пн-пт)
                    if (currentDay.DayOfWeek != DayOfWeek.Saturday && currentDay.DayOfWeek != DayOfWeek.Sunday)
                    {
                        DateTime workBegin = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, combinedStartDateTime.Hour, combinedStartDateTime.Minute, 0);
                        DateTime workEnd = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, combinedFinishDateTime.Hour, combinedFinishDateTime.Minute, 0);

                        Odb.db.Database.ExecuteSqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule(WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTime, WorkingHours) VALUES (@workbegin, @workend, @dta, @staffid, @lunchtime, @workinghours)",
                            new SqlParameter("workbegin", workBegin), new SqlParameter("workend", workEnd), new SqlParameter("dta", currentDay.Date), new SqlParameter("staffid", SelectedStaff.STAFF_ID), new SqlParameter("lunchtime", LunchTime), new SqlParameter("workinghours", WorkingHours));
                    }

                    // Переход к следующему дню
                    currentDay = currentDay.AddDays(1);
                    while (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                    {
                        currentDay = currentDay.AddDays(1);
                    }
                }
            }
            else if (TemplateCB.SelectedIndex == 0)
            {
                bool addRecord = true;
                int counter = 0;

                while (currentDay <= selectedFinishDate)
                {
                    if (addRecord)
                    {
                        DateTime workBegin = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, combinedStartDateTime.Hour, combinedStartDateTime.Minute, 0);
                        DateTime workEnd = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, combinedFinishDateTime.Hour, combinedFinishDateTime.Minute, 0);

                        Odb.db.Database.ExecuteSqlCommand("INSERT INTO Zarplats.dbo.Staff_Schedule(WorkBegin, WorkEnd, DTA, STAFF_ID, LunchTime, WorkingHours) VALUES (@workbegin, @workend, @dta, @staffid, @lunchtime, @workinghours)",
                            new SqlParameter("workbegin", workBegin), new SqlParameter("workend", workEnd), new SqlParameter("dta", currentDay.Date), new SqlParameter("staffid", SelectedStaff.STAFF_ID), new SqlParameter("lunchtime", LunchTime), new SqlParameter("workinghours", WorkingHours));
                    }

                    currentDay = currentDay.AddDays(1);
                    counter++;

                    if (counter % 2 == 0)
                        addRecord = !addRecord;
                }
            }
        }

        private bool isRequiredFieldsNotEmpty()
        {
            if (StaffLV.SelectedItem != null && StartTimeMTBX.Value != null && FinishTimeMTBX.Value != null && ScheduleStartDP.SelectedDate != null && ScheduleFinishDP.SelectedDate != null && LunchTimeTBX.Text != String.Empty && TemplateCB.SelectedItem != null)
                return true;
            else
                return false;
        }
        */
    }

    public class filterCMB
    {
        public int id { get; set; }
        public string filterName { get; set; } = "";
        public override string ToString() => $"{filterName}";
    }
}
