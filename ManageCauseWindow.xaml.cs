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
    public partial class ManageCauseWindow : Window
    {
        List<AbsenceModel> CauseList = new List<AbsenceModel>();

        public event EventHandler CauseAdded;
        public event EventHandler CauseRemoved;

        public ManageCauseWindow()
        {
            InitializeComponent();
            UpdateCauseList();

            this.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                    this.Close();
            };
        }

        private void AddAbsenceBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCauseWindow addCauseWindow = new AddCauseWindow();
            addCauseWindow.CauseAdded += AddCauseWindow_CauseAdded;
            addCauseWindow.ShowDialog();
        }

        private void CauseRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AbsenceModel selectedCause = button.DataContext as AbsenceModel;

            MessageBoxResult result = MessageBox.Show($"Удалить причину {selectedCause.Cause}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Odb.db.Database.ExecuteSqlCommand("delete from Zarplats.dbo.AbsenceRef where ID_AbsenceRef = @id", new SqlParameter("id", selectedCause.ID_AbsenceRef));
                CauseList.Remove(selectedCause);
                MessageBox.Show("Причина удалена!");
                CauseLV.ItemsSource = CauseList.ToList();
                CauseRemoved?.Invoke(this, EventArgs.Empty);
            }
        }

        private void AddCauseWindow_CauseAdded(object sender, EventArgs e)
        {
            CauseLV.ItemsSource = null;
            UpdateCauseList();
            CauseAdded?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateCauseList()
        {
            CauseList = Odb.db.Database.SqlQuery<AbsenceModel>("select distinct * from Zarplats.dbo.AbsenceRef").ToList();
            CauseLV.ItemsSource = CauseList;
        }
    }
}
