using PlanningScheduleApp.Models;
using PlanningScheduleApp.Pages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlanningScheduleApp
{
    public partial class SmenZadaniaWindow : Window
    {
        private List<SmenZadaniaModel> SmenZadaniaList = new List<SmenZadaniaModel>();

        public SmenZadaniaWindow(List<SmenZadaniaModel> smenZadaniaList)
        {
            try
            {
                InitializeComponent();
                SmenZadaniaList = smenZadaniaList;
                AssignCMB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна сменных заданий: {ex.Message}");
            }
        }

        private void SearchTBX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SearchInDG();

        private void SearchInDG()
        {
            List<SmenZadaniaModel> staff = new List<SmenZadaniaModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                staff = SmenZadaniaList;

            switch (filterCMB.SelectedIndex)
            {
                case 0:
                    staff = SmenZadaniaList.Where(u => u.Product.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 1:
                    staff = SmenZadaniaList.Where(u => u.PP.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 2:
                    staff = SmenZadaniaList.Where(u => u.Detail.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                default:
                    staff = SmenZadaniaList.Where(u => u.SZFull.ToLower().Contains(txt.ToLower())).ToList();
                    break;

            };
            SmenZadaniaDG.ItemsSource = staff;
        }

        private void filterCMB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SearchTBX.Clear();

        public void AssignCMB()
        {
            filterCMB.ItemsSource = new filterCMB[]
            {
                new filterCMB { id = 0, filterName = "изделию" },
                new filterCMB { id = 1, filterName = "заказу" },
                new filterCMB { id = 2, filterName = "детали" }
            };
            filterCMB.SelectedIndex = 0;
        }

    }
}
