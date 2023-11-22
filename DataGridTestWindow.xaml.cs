using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
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

namespace PlanningScheduleApp
{
    public partial class DataGridTestWindow : Window
    {
        public string connectionString = "Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql";
        List<StaffModel> StaffList = new List<StaffModel>();
        DepModel SelectedDep = new DepModel();
        StaffModel SelectedStaff { get; set; }

        public DataGridTestWindow(DepModel selectedDep)
        {
            InitializeComponent();
            SelectedDep = selectedDep;

            StaffList = Odb.db.Database.SqlQuery<StaffModel>("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA", new SqlParameter("podrazd", SelectedDep.Position)).ToList();
            GenerateColumns();
            FillDTAStatus();
            StaffDGTest.ItemsSource = StaffList;
        }

        DateTime selectedMonth = new DateTime(2023, 11, 1);

        private void GenerateColumns()
        {
            List<DateTime> datesInMonth = GetDatesForMonth(selectedMonth);

            for (int i = 0; i < datesInMonth.Count; i++)
            {
                DataGridTemplateColumn column = new DataGridTemplateColumn
                {
                    Header = datesInMonth[i].Day.ToString(),
                    Width = DataGridLength.Auto,
                    CellTemplate = FindResource("DayColumnTemplate") as DataTemplate,
                };

                StaffDGTest.Columns.Add(column);
            }
        }

        private List<DateTime> GetDatesForMonth(DateTime month)
        {
            List<DateTime> datesInMonth = new List<DateTime>();

            DateTime firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                datesInMonth.Add(date);
            }

            return datesInMonth;
        }

        public void FillDTAStatus()
        {
            var datesInMonth = GetDatesForMonth(selectedMonth);

            foreach (var staff in StaffList)
            {
                staff.DTAStatusList = new List<StatusInfo>();

                for (int i = 0; i < datesInMonth.Count; i++)
                {
                    bool hasRecord = CheckRecordInDatabase(staff.STAFF_ID, datesInMonth[i]);
                    Console.WriteLine($"---\nhasRecord for {staff.STAFF_ID} {datesInMonth[i].Date}: {hasRecord}");

                    StatusInfo status = new StatusInfo { Date = datesInMonth[i], Status = hasRecord ? "Р" : "Н" };

                    staff.DTAStatusList.Add(status);
                }
            }
        }

        private bool CheckRecordInDatabase(int staffId, DateTime date)
        {
            string query = "SELECT COUNT(*) FROM Zarplats.dbo.Staff_Schedule WHERE STAFF_ID = @staffId AND DTA = @date";

            int recordCount = Odb.db.Database.SqlQuery<int>(query,
                new SqlParameter("staffId", staffId),
                new SqlParameter("date", date)).FirstOrDefault();

            return recordCount > 0;
        }

        private void StaffDGTest_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffDGTest.SelectedItem;
            MessageBox.Show($"SelectedStaff Info: \nFIO: {SelectedStaff.SHORT_FIO}\nDTAWork: {SelectedStaff.DTA}\nWorkStatus: {SelectedStaff.WorkStatus}");
        }
    }
}
