using System;
using System.Linq;
using System.Windows;

namespace GraduationProject.Models
{
    public class DataModel
    {
        public int Id { get; set; }

        private double _x;

        public double X
        {
            get { return _x;}
            set { _x = value; }
        }

        private double _y;

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double? HorizontalDistance { get; set; }

        public double? SlopeDistance { get; set; }

        public double? Height { get; set; }

        public double? VerticalDistance { get; set; }

        public double? Azimuth { get; set; }

        public double? Bias { get; set; }

        public double? DiameterOne { get; set; }

        public double? DiameterTwo { get; set; }

        public string Species { get; set; }

        private int? _treeNumber;

        public int? TreeNumber
        {
            get { return _treeNumber; }

            set
            {
                foreach(var elem in CurrentContext.DataList)
                {
                    if (elem.TreeNumber == value)
                    {
                        _treeNumber = null;
                        MessageBox.Show("Дерево с таким номером уже существует");

                        return;
                    }
                }

                _treeNumber = value;
            }
        }

        public override string ToString()
        {
            return
                $"{Id},{X},{Y},{HorizontalDistance},{VerticalDistance},{SlopeDistance},{Azimuth},{Bias},{DiameterOne},{DiameterTwo},{Species}";
        }
    }
}