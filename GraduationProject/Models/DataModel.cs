namespace GraduationProject.Models
{
    public class DataModel
    {
        public int Id { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double? HorizontalDistance { get; set; }

        public double? SlopeDistance { get; set; }

        public double? Height { get; set; }

        public double? VerticalDistance { get; set; }

        public double? Azimuth { get; set; }

        public double? Bias { get; set; }

        public double? DiameterOne { get; set; }

        public double? DiameterTwo { get; set; }

        public string Species { get; set; }

        public override string ToString()
        {
            return
                $"{Id},{X},{Y},{HorizontalDistance},{VerticalDistance},{Azimuth},{Bias},{DiameterOne},{DiameterTwo},{Species},{SlopeDistance},{Height}";
        }
    }
}