using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlanningScheduleApp
{
    public class SearchInDatagrid
    {
        public delegate string SearchPropertySelector<T>(T item);

        public static void SearchToListView<T>(List<T> items, SearchPropertySelector<T> propertySelector, TextBox textboxFrom, ListView listViewTo)
        {
            List<T> filteredItems = new List<T>();
            string txt = textboxFrom.Text;
            if (txt.Length == 0)
            {
                filteredItems = items;
            }
            else
            {
                filteredItems = items.Where(u => propertySelector(u).ToLower().Contains(txt.ToLower())).ToList();
            }
            listViewTo.ItemsSource = filteredItems;
        }
    }
}
