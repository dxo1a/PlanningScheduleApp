using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace PlanningScheduleApp
{
    public partial class DataGridTestWindow : Window
    {
        private BindingSource staffBindingSource = new BindingSource();
        private BindingList<StaffModel> StaffList = new BindingList<StaffModel>();
        private DepModel SelectedDep { get; set; }

        private StaffModel SelectedRow { get; set; }

        List<string> columnsToShow = new List<string> { "STAFF_ID", "TABEL_ID", "SHORT_FIO", "WorkTime", "LunchTime", "DTA", "WorkingHours", };

        public DataGridTestWindow(DepModel selectedDep)
        {
            InitializeComponent();

            SetDoubleBuffered(StaffDGV, true);

            SelectedDep = selectedDep;
            Task.Run(() => UpdateDGV());
            StaffDGV.SelectionChanged += StaffDGV_SelectionChanged;
        }

        private async void UpdateDGV()
        {
            try
            {
                List<StaffModel> tempList = await Task.Run(() =>
                {
                    return Odb.db.Database.SqlQuery<StaffModel>("SELECT DISTINCT a.ID_Schedule, a.STAFF_ID, LTRIM(e.TABEL_ID) as TABEL_ID, e.SHORT_FIO, a.WorkBegin, a.WorkEnd, a.DTA, a.LunchTimeBegin, a.LunchTimeEnd, a.WorkingHours, b.ID_Absence, c.Cause as CauseAbsence, b.DateBegin, b.DateEnd, b.TimeBegin, b.TimeEnd FROM [Zarplats].[dbo].[Staff_Schedule] as a left join Zarplats.dbo.Schedule_Absence as b on a.STAFF_ID = b.id_Staff and a.DTA between b.DateBegin and b.DateEnd left join Zarplats.dbo.AbsenceRef as c on b.AbsenceRef_ID = c.ID_AbsenceRef left join perco...staff as e on a.STAFF_ID = e.ID_STAFF left join Zarplats.dbo.StaffView as f on a.STAFF_ID = f.STAFF_ID where f.Position = @podrazd order by a.DTA",
                        new SqlParameter("podrazd", SelectedDep.Position)).ToList();
                });

                var groupedData = tempList.GroupBy(x => x.STAFF_ID)
                                  .Select(g => g.First())
                                  .ToList();

                Dispatcher.Invoke(() =>
                {
                    StaffList.Clear();
                    foreach (var staff in groupedData)
                    {
                        StaffList.Add(staff);
                    }

                    staffBindingSource.DataSource = StaffList;

                    DateTime currentDate = DateTime.Now;
                    int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                    StaffDGV.Columns.Clear();

                    DataGridViewTextBoxColumn staffIdColumn = new DataGridViewTextBoxColumn();
                    staffIdColumn.HeaderText = "ID";
                    staffIdColumn.DataPropertyName = "STAFF_ID";
                    StaffDGV.Columns.Add(staffIdColumn);

                    DataGridViewTextBoxColumn staffFIOColumn = new DataGridViewTextBoxColumn();
                    staffFIOColumn.HeaderText = "ФИО";
                    staffFIOColumn.DataPropertyName = "SHORT_FIO";
                    StaffDGV.Columns.Add(staffFIOColumn);

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DataGridViewTextBoxColumn dateColumn = new DataGridViewTextBoxColumn();
                        dateColumn.HeaderText = day.ToString();
                        dateColumn.DataPropertyName = $"Day_{day}";
                        StaffDGV.Columns.Add(dateColumn);
                    }

                    StaffDGV.DataSource = staffBindingSource;

                    StaffDGV.RowPrePaint += (s, e) =>
                    {
                        if (e.RowIndex >= 0 && e.RowIndex < StaffDGV.RowCount)
                        {
                            DataGridViewRow row = StaffDGV.Rows[e.RowIndex];
                            StaffModel staff = (StaffModel)row.DataBoundItem;

                            if (staff != null)
                            {
                                for (int day = 1; day <= daysInMonth; day++)
                                {
                                    int columnIndex = day + 1;
                                    DateTime currentDateForCell = new DateTime(currentDate.Year, currentDate.Month, day);
                                    bool isRecordExists = CheckRecordExists(staff.STAFF_ID, currentDateForCell);

                                    row.Cells[columnIndex].Value = isRecordExists ? "Р" : "Н";
                                }
                            }
                        }
                    };
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
            }
        }

        private bool CheckRecordExists(int staffId, DateTime date)
        {
            int checkExisting = Odb.db.Database.SqlQuery<int>("select count(*) from Zarplats.dbo.Staff_Schedule where STAFF_ID = @staffId and DTA = @dta", new SqlParameter("staffId", staffId), new SqlParameter("dta", date)).FirstOrDefault();
            if (checkExisting > 0)
                return true;
            else
                return false;
        }

        private void SetDoubleBuffered(System.Windows.Forms.Control c, bool value)
        {
            var property = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            property.SetValue(c, value, null);
        }

        private void StaffDGV_SelectionChanged(object sender, EventArgs e)
        {
            if (StaffDGV.CurrentRow != null)
            {
                SelectedRow = (StaffModel)staffBindingSource.Current;
                Console.WriteLine($"SelectedRow: {SelectedRow}");
            }
        }

        int doubleClickCounter = 0;
        private void StaffDGV_DoubleClick(object sender, EventArgs e)
        {
            if (SelectedRow != null)
            {
                doubleClickCounter++;
                Console.WriteLine($"StaffDGV DoubleClicked {doubleClickCounter} times!");
                MessageBox.Show($"SelectedRow Info:\n{SelectedRow.STAFF_ID}, {SelectedRow.DTA}");
            }
        }
    }
}
