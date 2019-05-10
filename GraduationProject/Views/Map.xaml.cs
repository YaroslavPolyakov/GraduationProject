using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace GraduationProject.Views
{
    public partial class Map
    {
        public Map()
        {
            InitializeComponent();
        }

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // choose your provider here
            GMapControl.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            GMapControl.MinZoom = 2;
            GMapControl.MaxZoom = 100;
            // whole world zoom
            GMapControl.Zoom = 2;
            // lets the map use the mousewheel to zoom
            GMapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            GMapControl.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            GMapControl.DragButton = MouseButton.Left;
            foreach (var measure in CurrentContext.DataList)
            {
                var marker = new GMapMarker(new PointLatLng(measure.Y, measure.X));
                marker.Shape = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1.5
                };
                GMapControl.Markers.Add(marker);
            }
        }
    }
}