using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace GraduationProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            foreach (string arg in e.Args)
            {
                string searchString = ",";
                int indexOfStart = arg.IndexOf(searchString);
                var x = arg.Substring(0, indexOfStart);
                var y = arg.Substring(indexOfStart+1);
                CurrentContext.StartupX = Convert.ToDouble(x);
                CurrentContext.StartupY = Convert.ToDouble(y);
            }
        }
    }
}