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
        private List<SmenZadaniaModel> SmenZadaniaListNonAsync = new List<SmenZadaniaModel>();

        private int StaffID;
        private DateTime Date;

        public SmenZadaniaWindow(int staffid, DateTime date)
        {
            try
            {
                InitializeComponent();
                StaffID = staffid;
                Date = date;
                AssignCMB();
                Task.Run(async () => await LoadSmenZadaniaForStuffAsync());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна сменных заданий: {ex.Message}");
            }
        }

        private async Task LoadSmenZadaniaForStuffAsync()
        {
            using (SqlConnection connection = new SqlConnection("Persist Security Info=False;User ID=sa; Password=server_esa;Initial Catalog=dsl_sp;Server=sql"))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(@"
                    SELECT DISTINCT
                        c.SHORT_FIO,
                        sz.Product,
                        sz.Detail as DetailNum,
                        dv.НазваниеД as DetailName,
                        sz.NUM,
                        dv.Договор as PP,
                        CAST(ROUND(sz.Cost, 2) as FLOAT) as Cost,
                        sz.Count,
                        a.WorkingHours,
                        a.DTA,
                        CAST(ROUND((SELECT SUM(sz.Cost) FROM Zarplats.dbo.SmenZadView sz WHERE LTRIM(c.TABEL_ID) = sz.id_Tabel AND a.DTA = sz.DTE), 2) AS FLOAT) as TotalHours
                    FROM [Zarplats].[dbo].[Staff_Schedule] as a
                    LEFT JOIN PERCO...staff_ref as b on a.STAFF_ID = b.STAFF_ID
                    LEFT JOIN PERCO...staff AS c ON b.STAFF_ID = c.ID_STAFF
                    LEFT JOIN PERCO...subdiv_ref AS d ON b.SUBDIV_ID = d.ID_REF
                    LEFT JOIN PERCO...appoint_ref AS e ON b.APPOINT_ID = e.ID_REF
                    LEFT JOIN Zarplats.dbo.SmenZadView as sz on LTRIM(c.TABEL_ID) = sz.id_Tabel and a.DTA = sz.DTE
                    LEFT JOIN Cooperation.dbo.DetailsView as dv on sz.NUM = dv.ПрП and sz.Detail = dv.НомерД
                    WHERE b.STAFF_ID = @staffid and a.DTA = @dta
                ", connection))
                {
                    command.Parameters.AddWithValue("staffid", StaffID);
                    command.Parameters.AddWithValue("dta", Date);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        SmenZadaniaList.Clear();

                        while (await reader.ReadAsync())
                        {
                            SmenZadaniaModel smenZadania = new SmenZadaniaModel
                            {
                                SHORT_FIO = reader["SHORT_FIO"].ToString(),
                                Product = reader["Product"].ToString(),
                                DetailNum = reader["DetailNum"].ToString(),
                                DetailName = reader["DetailName"].ToString(),
                                NUM = reader["NUM"].ToString(),
                                PP = reader["PP"].ToString(),
                                Cost = Convert.ToDouble(reader["Cost"]),
                                Count = Convert.ToDouble(reader["Count"]),
                                WorkingHours = Convert.ToDouble(reader["WorkingHours"]),
                                DTA = Convert.ToDateTime(reader["DTA"]),
                                TotalHours = Convert.ToDouble(reader["TotalHours"])
                            };

                            SmenZadaniaList.Add(smenZadania);
                        }
                    }
                }
            }

            Dispatcher.Invoke(() =>
            {
                LoadingSZTB.Visibility = Visibility.Collapsed;
                SmenZadaniaDG.ItemsSource = SmenZadaniaList;
                SmenZadaniaDG.Visibility = Visibility.Visible;
            });
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
                    staff = SmenZadaniaList.Where(u => u.SHORT_FIO.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 1:
                    staff = SmenZadaniaList.Where(u => u.Product.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 2:
                    staff = SmenZadaniaList.Where(u => u.PP.ToString().ToLower().Contains(txt.ToLower())).ToList();
                    break;
                case 3:
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
                new filterCMB { id = 0, filterName = "ФИО" },
                new filterCMB { id = 1, filterName = "изделию" },
                new filterCMB { id = 2, filterName = "договору" },
                new filterCMB { id = 3, filterName = "детали" }
            };
            filterCMB.SelectedIndex = 0;
        }

    }
}
