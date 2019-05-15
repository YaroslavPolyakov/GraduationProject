using System.Collections.Generic;

namespace GraduationProject.Models
{
    public class MeasureValueModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<TemplateColumn> TemplateColumns { get; set; }

        public bool CheckStatus { get; set; }
    }
}