using Microsoft.Office.Interop.Excel;
using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Net;
using RestSharp;
using System.Threading;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using System.Security.Policy;

namespace PlanningScheduleApp.Pages
{
    public partial class SelectedDepPage : System.Windows.Controls.Page
    {
        //List<ScheduleModel> ScheduleList = new List<ScheduleModel>();
        List<StaffModel> StaffList = new List<StaffModel>();


        DepModel SelectedDep { get; set; }
        //ScheduleModel SelectedSchedule { get; set; }

        StaffModel SelectedStaff { get; set; }

        DateTime SelectedDate;
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
            SelectedStaff = (StaffModel)StaffDG.SelectedItem;
            var result = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Odb.db.Database.ExecuteSqlCommand("DELETE FROM SerialNumber.dbo.Staff_Schedule WHERE DTA = @dta and WorkingHours = @wh", new SqlParameter("dta", SelectedStaff.DTA), new SqlParameter("wh", SelectedStaff.WorkingHours));
                UpdateGrid();
            }

        }

        private void StaffDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStaff = (StaffModel)StaffDG.SelectedItem;
            if (SelectedStaff != null)
            {
                StaffRemoveBtn.IsEnabled = true;
                StaffEditBtn.IsEnabled = true;
            }
        }

        private void StaffRefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
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

        private void UpdateGrid()
        {
            //ScheduleList = Odb.db.Database.SqlQuery<ScheduleModel>("select distinct a.DTA, a.WorkingHours AS WorkingHours from SerialNumber.dbo.Staff_Schedule as a left join perco...staff_ref as b on a.STAFF_ID = b.STAFF_ID where a.STAFF_ID = @staffid group by a.DTA, a.WorkingHours", new SqlParameter("staffid", SelectedStaff.STAFF_ID)).ToList();
            StaffList = Odb.db.Database.SqlQuery<StaffModel>("select distinct b.FIO as FIO, LTRIM(b.Tabel) as Tabel, a.WorkingHours, a.DTA, a.STAFF_ID, ID_Schedule from SerialNumber.dbo.Staff_Schedule as a left join SerialNumber.dbo.StaffView as b on a.STAFF_ID = b.STAFF_ID where b.VALID = 1 and b.Position = @pos ORDER BY a.DTA DESC", new SqlParameter("pos", SelectedDep.Position)).ToList();
            if (StaffList != null)
                StaffDG.ItemsSource = StaffList;
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

        private void ExportToBitrix24Btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class filterCMB
    {
        public int id { get; set; }
        public string filterName { get; set; } = "";
        public override string ToString() => $"{filterName}";
    }
}
