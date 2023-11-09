using System;
using System.Collections.Generic;
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
using Xceed.Wpf.Toolkit;

namespace PlanningScheduleApp
{
    public partial class ScheduleEditWindow : Window
    {
        public int ID_Template;

        public ScheduleEditWindow(int idtemplate)
        {
            InitializeComponent();
            ID_Template = idtemplate;
        }

        #region Проверка LunchTimeTBX
        private void LunchTimeTBX_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            // Разрешить ввод только цифр и одной точки или запятой
            if (!IsNumericInput(e.Text) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
            }
            else if (e.Text == "." || e.Text == ",")
            {
                // Если введена точка или запятая, заменить ее на запятую
                if (textBox != null)
                {
                    int caretIndex = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Insert(caretIndex, ",");
                    textBox.CaretIndex = caretIndex + 1;
                    e.Handled = true;
                }
            }
        }

        private void LunchTimeTBX_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Разрешить удаление символа
            if (e.Key == Key.Back)
                return;

            // Запретить ввод пробела
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private bool IsNumericInput(string text)
        {
            // Проверить, является ли введенный текст числом, запятой или точкой
            return double.TryParse(text, out double result) || text == "," || text == ".";
        }
        #endregion

        private void MTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            if (maskedTextBox != null)
            {
                maskedTextBox.Dispatcher.BeginInvoke((Action)(() => { maskedTextBox.Select(0, 0); }));
            }
        }
    }
}
