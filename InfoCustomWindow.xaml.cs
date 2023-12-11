using PlanningScheduleApp.Models;
using PlanningScheduleApp.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlanningScheduleApp
{
    public partial class InfoCustomWindow : Window
    {
        StaffModel SelectedRow { get;set; }
        DateTime Date;

        List<StaffModel> AbsenceList = new List<StaffModel>();
        BindingList<StaffModel> StaffList = new BindingList<StaffModel>();
        StaffModel ScheduleInfo { get; set; }

        private SelectedDepPageVer2 _SelectedDepPageVer2 { get; set; }

#pragma warning disable CS0067
        public event EventHandler AbsenceRemoved;
#pragma warning restore CS0067

        public InfoCustomWindow(StaffModel selectedRow, DateTime date, BindingList<StaffModel> staffList, SelectedDepPageVer2 selectedDepPageVer2)
        {
            try
            {
                InitializeComponent();

                _SelectedDepPageVer2 = selectedDepPageVer2;
                SelectedRow = selectedRow;
                Date = date;
                StaffList = staffList;

                UpdateScheduleInfo();
                AssignData();

                this.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.Escape)
                        this.Close();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна с информацией\n{ex.Message}");
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InfoTC_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (InfoTC.SelectedIndex == 0)
            {
                this.Height = 245;
            }
                
            else if (InfoTC.SelectedIndex == 1)
            {
                this.Height = 320;
            }  
        }

        private void AssignData()
        {
            AbsenceList = Odb.db.Database.SqlQuery<StaffModel>("select a.*, b.Cause as CauseAbsence from Zarplats.dbo.Schedule_Absence as a left join Zarplats.dbo.AbsenceRef as b on a.AbsenceRef_ID = b.ID_AbsenceRef where id_Staff = @idstaff and DateBegin <= @date and DateEnd >= @date", new SqlParameter("idstaff", SelectedRow.STAFF_ID), new SqlParameter("date", Date)).ToList();
           

            if (AbsenceList.Count <= 0)
                AbsenceTI.Visibility = Visibility.Collapsed;
            else
                AbsenceLV.ItemsSource = AbsenceList;
        }

        private async void AbsenceRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => App.DisableAllWindows());
            Button button = sender as Button;
            StaffModel selectedAbsence = button.DataContext as StaffModel;

            DateTime dateBegin = selectedAbsence.DateBegin ?? DateTime.Now;
            DateTime dateEnd = selectedAbsence.DateEnd ?? DateTime.Now;

            MessageBoxResult result = MessageBox.Show($"Удалить отсутствие?\nСотрудник: {SelectedRow.SHORT_FIO} ({SelectedRow.TABEL_ID})\nПричина: {selectedAbsence.CauseAbsence}\nВремя: {selectedAbsence.AbsenceTime}\nДата: {selectedAbsence.AbsenceDate}", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // удаление отсутствия и обновление ячейки
                DateTime startDateTime = selectedAbsence.DateBegin ?? DateTime.Now;
                DateTime endDateTime = selectedAbsence.DateEnd ?? DateTime.Now;

                List<StaffModel> affectedRows = await _SelectedDepPageVer2.GetAffectedRowsAsync(startDateTime, endDateTime);

                using (SqlConnection connection = new SqlConnection("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql"))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("DELETE FROM Zarplats.dbo.Schedule_Absence WHERE ID_Absence = @idabsence", connection))
                    {
                        command.Parameters.Add(new SqlParameter("idabsence", selectedAbsence.ID_Absence));
                        command.ExecuteNonQuery();
                    }
                }

                int rowIndex = StaffList.ToList().FindIndex(staff => staff.STAFF_ID == SelectedRow.STAFF_ID);
                await _SelectedDepPageVer2.UpdateAffectedCellsAsync(startDateTime, endDateTime, rowIndex);

                await _SelectedDepPageVer2.UpdateWorkingHoursForAffectedRows(new List<StaffModel> { ScheduleInfo }, affectedRows);

                AbsenceList.Remove(selectedAbsence);
                MessageBox.Show("Отсутствие удалено!");
                AbsenceLV.ItemsSource = AbsenceList.ToList();
                UpdateScheduleInfo();
            }
            Dispatcher.Invoke(() => App.EnableAllWindows());
        }

        private void UpdateScheduleInfo()
        {
            ScheduleInfo = Odb.db.Database.SqlQuery<StaffModel>("select distinct * from Zarplats.dbo.Staff_Schedule where STAFF_ID = @idstaff and DTA = @date", new SqlParameter("idstaff", SelectedRow.STAFF_ID), new SqlParameter("date", Date)).FirstOrDefault();

            string scheduleDynamicStroke;

            scheduleDynamicStroke = $"Сотрудник: {SelectedRow.SHORT_FIO}" + Environment.NewLine +
                                $"Дата: {ScheduleInfo.DTA.ToShortDateString()}" + Environment.NewLine +
                                $"Время работы: {ScheduleInfo.WorkTime}" + Environment.NewLine +
                                $"Обед: {ScheduleInfo.LunchTime}" + Environment.NewLine +
                                $"Рабочие часы: {ScheduleInfo.WorkingHours}";
            WorkingDayInfoTBX.Text = scheduleDynamicStroke;
        }
    }
}
