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
                if (arg.ToLower().Contains("x"))
                {
                    //CurrentContext.StartupX = Convert.ToInt32(Regex.Replace(arg, @"[^\d]+", ""));
                    CurrentContext.StartupX = Convert.ToDouble(arg.Substring(2));
                    MessageBox.Show("Координата X получена");
                }
                if (arg.ToLower().Contains("y"))
                {
                    CurrentContext.StartupY = Convert.ToDouble(arg.Substring(2));
                    //CurrentContext.StartupY = Convert.ToDouble(Regex.Replace(arg, @"[^\d]+", ""));
                    MessageBox.Show("Координата Y получена");
                }     
            }
        }
    }
}