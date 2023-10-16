using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPage : System.Windows.Controls.Page
    {
        List<StaffModel> StaffList = new List<StaffModel>();

        DepModel SelectedDep { get; set; }
        StaffModel SelectedStaff { get; set; }

        string mode;

        public SelectedDepPage(DepModel selectedDep)
        {
            InitializeComponent();

            SelectedDep = selectedDep;

            FrameApp.SetCurrentMainFrame(FrameApp.FrameMain);

            UpdateGrid();
            AssignCMB();
        }

        private void StaffRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteRow();
        }

        private void StaffDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffDG.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffRemoveBtn.IsEnabled = true;
                StaffEditBtn.IsEnabled = true;
            }
            else
            {
                StaffRemoveBtn.IsEnabled = false;
                StaffEditBtn.IsEnabled = false;
            }
        }

        private void StaffRefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            StaffList = Odb.db.Database.SqlQuery<StaffModel>("select distinct b.FIO as FIO, LTRIM(b.Tabel) as Tabel, a.WorkingHours, a.DTA, a.STAFF_ID, ID_Schedule from SerialNumber.dbo.Staff_Schedule as a left join SerialNumber.dbo.StaffView as b on a.STAFF_ID = b.STAFF_ID where b.VALID = 1 and b.Position = @pos ORDER BY a.DTA DESC", new SqlParameter("pos", SelectedDep.Position)).ToList();
            if (StaffList != null)
                StaffDG.ItemsSource = StaffList;

            StaffDG.SelectedItem = null;
        }

        #region Search Functionality
        private void SearchTBX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            List<StaffModel> staff = new List<StaffModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                staff = StaffList;

            switch (filterCMB.SelectedIndex)
            {
                case 0:
                    staff = StaffList.Where(u => u.FIO.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 1:
                    staff = StaffList.Where(u => u.Tabel.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 2:
                    staff = StaffList.Where(u => u.DTA.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 3:
                    staff = StaffList.Where(u => u.WorkingHours.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                default:
                    staff = StaffList.Where(u => u.StaffFull.ToLower().Contains(txt.ToLower())).ToList();
                    break;

            };
            StaffDG.ItemsSource = staff;
        }

        private void filterCMB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SearchTBX.Clear();
        }

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

        private void ExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelFilterWindow exportToExcelFilterWindow = new ExportToExcelFilterWindow();
            exportToExcelFilterWindow.ShowDialog();
        }

        private void StaffEditBtn_Click(object sender, RoutedEventArgs e)
        {
            mode = "Edit";
            StaffAddOrEditWindow staffAddOrEditWindow = new StaffAddOrEditWindow(SelectedStaff, mode, SelectedDep);
            staffAddOrEditWindow.ShowDialog();
        }

        private void StaffAddBtn_Click(object sender, RoutedEventArgs e)
        {
            mode = "Add";
            StaffAddOrEditWindow staffAddOrEditWindow = new StaffAddOrEditWindow(SelectedStaff, mode, SelectedDep);
            staffAddOrEditWindow.ShowDialog();
        }

        public void DeleteRow()
        {
            List<StaffModel> selectedItems = StaffDG.SelectedItems.Cast<StaffModel>().ToList();
            if (selectedItems.Count > 0)
            {
                var result = MessageBox.Show("Удалить записи?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (StaffModel selectedStaff in selectedItems)
                    {
                        Odb.db.Database.ExecuteSqlCommand("DELETE FROM SerialNumber.dbo.Staff_Schedule WHERE DTA = @dta and WorkingHours = @wh", new SqlParameter("dta", selectedStaff.DTA), new SqlParameter("wh", selectedStaff.WorkingHours));
                    }
                }
                UpdateGrid();
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
    }

    public class filterCMB
    {
        public int id { get; set; }
        public string filterName { get; set; } = "";
        public override string ToString() => $"{filterName}";
    }
}
