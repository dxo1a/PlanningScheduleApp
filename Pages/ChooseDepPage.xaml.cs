using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PlanningScheduleApp.Pages
{
    public partial class ChooseDepPage : Page
    {
        List<DepModel> DepList = new List<DepModel>();
        List<StaffModel> StaffPositionsList = new List<StaffModel>();

        DepModel SelectedDep { get; set; }

        public ChooseDepPage()
        {
            InitializeComponent();

            DepList = Odb.db.Database.SqlQuery<DepModel>("SELECT DISTINCT Position FROM SerialNumber.dbo.StaffView").ToList();
            DepLV.ItemsSource = DepList.OrderBy(u => u.Position);
        }

        private void SearchDepTBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            search();
        }

        private void search()
        {
            List<DepModel> deps = new List<DepModel>();
            string txt = SearchDepTBX.Text;
            if (txt.Length == 0)
                deps = DepList;
            deps = DepList.Where(u => u.Position.ToLower().Contains(txt.ToLower())).ToList();
            DepLV.ItemsSource = deps;
        }

        private void SearchDepTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            DepLV.Visibility = Visibility.Collapsed;
        }

        private void SearchDepTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            DepLV.Visibility = Visibility.Visible;
        }

        private void DepLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDep = (DepModel)DepLV.SelectedItem;
            if (SelectedDep != null)
            {
                SearchDepTBX.Text = $"{SelectedDep.Position}";
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new SelectedDepPage(SelectedDep));
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchDepTBX.Clear();
            DepLV.SelectedItem = null;
        }
    }
}
