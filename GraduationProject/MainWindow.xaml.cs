using System.Windows;
using System.Windows.Controls;
using GraduationProject.Views;

namespace GraduationProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public TestMeasurement TestMeasurement = new TestMeasurement();
        public Map TestMeasurementMap = new Map();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridMain.Children.Clear();

            switch (((ListViewItem) ((ListView) sender).SelectedItem).Name)
            {
                case "Measurement":
                    GridMain.Children.Add(TestMeasurement);
                    break;
                case "Map":
                    GridMain.Children.Add(TestMeasurementMap);
                    break;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}