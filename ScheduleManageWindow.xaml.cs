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
    public partial class ScheduleManageWindow : Window
    {
        public event EventHandler TemplateCreated;
        public event EventHandler TemplateDeleted;

        List<ScheduleTemplateModel> scheduleTemplates = new List<ScheduleTemplateModel>();
        ScheduleTemplateModel SelectedTemplate { get; set; }

        public ScheduleManageWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private void TemplateAdd_Click(object sender, RoutedEventArgs e)
        {
            ScheduleAddWindow scheduleAddWindow = new ScheduleAddWindow();
            scheduleAddWindow.TemplateCreated += OnTemplateCreated;
            scheduleAddWindow.ShowDialog();
        }

        private void TemplatesDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ScheduleEditWindow scheduleEditWindow = new ScheduleEditWindow(SelectedTemplate);
            scheduleEditWindow.TemplateCreated += OnTemplateCreated;
            scheduleEditWindow.ShowDialog();
        }

        private void TemplatesDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTemplate = (ScheduleTemplateModel)TemplatesDG.SelectedItem;
        }

        private void TemplateDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Удалить шаблон?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Odb.db.Database.ExecuteSqlCommand("delete from Zarplats.dbo.Schedule_Template where ID_Template = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template));
                if (SelectedTemplate.isFlexible)
                {
                    Odb.db.Database.ExecuteSqlCommand("delete from Zarplats.dbo.Schedule_FlexibleDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template));
                }
                else
                {
                    Odb.db.Database.ExecuteSqlCommand("delete from Zarplats.dbo.Schedule_StaticDays where Template_ID = @templateid", new SqlParameter("templateid", SelectedTemplate.ID_Template));
                }
                MessageBox.Show($"Шаблон {SelectedTemplate.TemplateName} удалён!");
                TemplateDeleted?.Invoke(this, EventArgs.Empty);
                UpdateGrid();
            }
        }

        private void OnTemplateCreated(object sender, EventArgs e)
        {
            UpdateGrid();
            TemplateCreated?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateGrid()
        {
            scheduleTemplates = Odb.db.Database.SqlQuery<ScheduleTemplateModel>("select ID_Template, TemplateName, isFlexible, WorkingDaysCount, RestingDaysCount from Zarplats.dbo.Schedule_Template").ToList();
            TemplatesDG.ItemsSource = scheduleTemplates;
        }
    }
}
