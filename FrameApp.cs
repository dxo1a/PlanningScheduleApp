using System;
using System.Windows.Controls;

namespace PlanningScheduleApp
{
    internal class FrameApp
    {
        public static Frame FrameMain { get; set; }
        public static Frame FrameTop { get; set; }
        public static void SetCurrentMainFrame(Frame frame) => FrameMain = frame;
        public static void SetCurrentTopFrame(Frame frame) => FrameTop = frame;

        public static void NavigateToPageMain(Page page)
        {
            if (FrameMain != null)
                FrameMain.Navigate(page);
            //FrameObjSec.Content = page;
            else
                throw new InvalidOperationException("Текущий Frame не был установлен. Установите его с помощью SetCurrentFrame.");
        }

        public static void NavigateToPageTop(Page page)
        {
            if (FrameTop != null)
                FrameTop.Navigate(page);
            //FrameObjSec.Content = page;
            else
                throw new InvalidOperationException("Текущий Frame не был установлен. Установите его с помощью SetCurrentFrame.");
        }
    }

    public class Odb
    {
        public static System.Data.Entity.DbContext db;
    }
}
