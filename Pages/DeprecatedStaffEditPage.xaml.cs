using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlanningScheduleApp.Pages
{
    public partial class DeprecatedStaffEditPage : Page
    {
        public List<DateViewModel> DatesList { get; set; }
        public List <StaffModel> SubdivisionsList { get; set; }

        StaffModel SelectedStaff { get; set; }

        double resultWorkingHours;

        public DeprecatedStaffEditPage(StaffModel selectedStaff)
        {
            InitializeComponent();

            Loaded += AddSchedulePage_Loaded;

            FrameApp.SetCurrentMainFrame(FrameApp.FrameMain);

            SelectedStaff = selectedStaff;
            if (SelectedStaff != null )
                StaffTB.Text = $"{SelectedStaff.SHORT_FIO} ({SelectedStaff.TABEL_ID})";

            SubdivisionsList = Odb.db.Database.SqlQuery<StaffModel>("select distinct Subdivision from SerialNumber.dbo.StaffView where STAFF_ID = @staffid", new SqlParameter("staffid", SelectedStaff.STAFF_ID)).ToList();
            StaffSubdivisionTB.Text = string.Join(", ", SubdivisionsList.Select(item => item.Subdivision));

            DatesList = new List<DateViewModel>();
            DateTime choosenDate = SelectedStaff.DTA;
            DateTime dateMinus3 = choosenDate.AddDays(-3);

            for (int i = 0; i <= 6; i++)
            {
                DateTime nextDate = dateMinus3.AddDays(i);
                DatesList.Add(new DateViewModel(nextDate));
            }

            DataContext = this;
        }

        private async void AddSchedulePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql"))
                {
                    await connection.OpenAsync();

                    foreach (var dateViewModel in DatesList)
                    {
                        string checkExistingQuery = "SELECT WorkingHours FROM SerialNumber.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date";
                        using (SqlCommand checkExistingCommand = new SqlCommand(checkExistingQuery, connection))
                        {
                            checkExistingCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                            checkExistingCommand.Parameters.AddWithValue("@date", dateViewModel.Date.ToString("dd.MM.yyyy"));

                            object workingHoursObj = await checkExistingCommand.ExecuteScalarAsync();

                            if (workingHoursObj != null && double.TryParse(workingHoursObj.ToString(), out double workingHours))
                            {
                                dateViewModel.WorkingHours = workingHours;
                                if (dateViewModel.Date == SelectedStaff.DTA)
                                {
                                    dateViewModel.IsDateMatching = true;
                                }
                                if (dateViewModel.Date == DateTime.Today)
                                {
                                    dateViewModel.IsTodayDateMatching = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void SaveScheduleBTN_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                using (SqlConnection connection = new SqlConnection("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql"))
                {
                    connection.Open();

                    foreach (var dateViewModel in DatesList)
                    {
                        if (dateViewModel.WorkingHours > 0)
                        {
                            resultWorkingHours = dateViewModel.WorkingHours;

                            string checkExistingQuery = "SELECT ID_Schedule FROM SerialNumber.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date";
                            using (SqlCommand checkExistingCommand = new SqlCommand(checkExistingQuery, connection))
                            {
                                checkExistingCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                checkExistingCommand.Parameters.AddWithValue("@date", dateViewModel.Date.ToString("dd.MM.yyyy"));

                                object existingId = checkExistingCommand.ExecuteScalar();

                                if (existingId != null)
                                {
                                    int scheduleId = (int)existingId;
                                    string updateQuery = "UPDATE SerialNumber.dbo.Staff_Schedule SET WorkingHours = @workingHours WHERE ID_Schedule = @scheduleId";
                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@scheduleId", scheduleId);
                                        updateCommand.Parameters.AddWithValue("@workingHours", resultWorkingHours);

                                        updateCommand.ExecuteNonQuery();
                                    }
                                    MessageBox.Show("Рабочие часы изменены.", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    string insertQuery = "INSERT INTO SerialNumber.dbo.Staff_Schedule (STAFF_ID, WorkingHours, DTA) VALUES (@staffId, @workingHours, @date)";
                                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@staffId", SelectedStaff.STAFF_ID);
                                        insertCommand.Parameters.AddWithValue("@workingHours", resultWorkingHours);
                                        insertCommand.Parameters.AddWithValue("@date", dateViewModel.Date.ToString("dd.MM.yyyy"));

                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            //SelectedStaffPage selectedStaffPage = new SelectedStaffPage(SelectedStaff);
            //FrameApp.NavigateToPageMain(selectedStaffPage);
        }

        private void WorkingHoursTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                DateViewModel dateViewModel = textBox.DataContext as DateViewModel;
                if (dateViewModel != null)
                {
                    if (double.TryParse(textBox.Text, out double workingHours))
                    {
                        dateViewModel.WorkingHours = workingHours;
                    }
                }
            }
        }

        private void StaffTBX_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void StaffTBX_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void StaffLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
