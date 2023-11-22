using PlanningScheduleApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
    public partial class SmenZadaniaWindow : Window
    {
        private List<SmenZadaniaModel> SmenZadaniaList = new List<SmenZadaniaModel>();

        private StaffModel SelectedStaff { get; set; }

        public SmenZadaniaWindow(StaffModel selectedStaff)
        {
            InitializeComponent();
            Task.Run(async () => await LoadSmenZadaniaForStuffAsync());

            SelectedStaff = selectedStaff;
            
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
                    command.Parameters.AddWithValue("staffid", SelectedStaff.STAFF_ID);
                    command.Parameters.AddWithValue("dta", SelectedStaff.DTA);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        SmenZadaniaList.Clear(); // Очищаем список перед добавлением новых данных

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
                                Count = Convert.ToInt32(reader["Count"]),
                                WorkingHours = Convert.ToDouble(reader["WorkingHours"]),
                                DTA = Convert.ToDateTime(reader["DTA"]),
                                TotalHours = Convert.ToDouble(reader["TotalHours"])
                            };

                            SmenZadaniaList.Add(smenZadania);
                        }
                    }
                }
            }

            SmenZadaniaDG.ItemsSource = SmenZadaniaList;
        }
    }
}
