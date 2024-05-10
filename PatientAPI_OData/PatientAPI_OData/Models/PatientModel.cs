using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAPI_OData.Models
{
    public class PatientModel
    {
        public int id { get; set; } = 0;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Active { get; set; } = "No";
    }
}
