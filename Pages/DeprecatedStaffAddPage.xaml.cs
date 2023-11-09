using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PlanningScheduleApp.Pages
{
    public partial class DeprecatedStaffAddPage : Page
    {
        public List<DateViewModel> DatesList { get; set; }
        List<StaffModel> StaffList = new List<StaffModel>();

        StaffModel SelectedStaff { get; set; }
        DepModel SelectedDep { get; set; }

        double resultWorkingHours;
        List<TextBox> textBoxes = new List<TextBox>();

        public DeprecatedStaffAddPage(DepModel selectedDep)
        {
            InitializeComponent();

            FrameApp.SetCurrentMainFrame(FrameApp.FrameMain);

            DatesList = new List<DateViewModel>();
            DateTime currentDate = DateTime.Now;

            SelectedDep = selectedDep;

            for (int i = 0; i <= 6; i++)
            {
                DateTime nextDate = currentDate.AddDays(i);
                DatesList.Add(new DateViewModel(nextDate));
            }

            StaffList = Odb.db.Database.SqlQuery<StaffModel>("select distinct a.FIO as FIO, LTRIM(a.Tabel) as Tabel, a.STAFF_ID from SerialNumber.dbo.StaffView as a left join SerialNumber.dbo.Staff_Schedule as b on a.STAFF_ID = b.STAFF_ID where a.VALID = 1 and a.Position = @pos ORDER BY a.FIO DESC", new SqlParameter("pos", SelectedDep.Position)).ToList();
            StaffLV.ItemsSource = StaffList;
            DataContext = this;
        }

        private void AddScheduleBTN_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show("Рабочие часы добавлены/изменены.", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public async void LoadStaffSchedule()
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

                            if (workingHoursObj == null)
                            {
                                dateViewModel.WorkingHours = 0;
                            }

                            if (workingHoursObj != null && double.TryParse(workingHoursObj.ToString(), out double workingHours))
                            {
                                dateViewModel.WorkingHours = workingHours;
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
            StaffLV.Visibility = Visibility.Visible;
        }

        private void StaffTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            StaffLV.Visibility= Visibility.Collapsed;
        }

        private void StaffLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffLV.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffTBX.Text = $"{SelectedStaff.SHORT_FIO} ({SelectedStaff.TABEL_ID})";
                for (int i = 0; i < textBoxes.Count; i++)
                {
                    textBoxes[i].IsEnabled = true;
                }
                LoadStaffSchedule();
            }
            
        }
        
        private void WorkingHoursTBX_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsEnabled = false;
                textBoxes.Add(textBox);
            }
        }
    }
}
