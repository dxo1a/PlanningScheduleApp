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
    public partial class AddCauseWindow : Window
    {
        public event EventHandler CauseAdded;

        public AddCauseWindow()
        {
            InitializeComponent();

            this.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                    this.Close();
            };
        }

        private void AddCauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CauseTBX.Text != null && CauseTBX.Text != string.Empty)
            {
                bool typeValue = TypeCB.IsChecked ?? false;
                Odb.db.Database.ExecuteSqlCommand("insert into Zarplats.dbo.AbsenceRef (Cause, Type) values (@cause, @type)", new SqlParameter("@cause", CauseTBX.Text), new SqlParameter("type", typeValue));
                CauseAdded?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
        }
    }
}
