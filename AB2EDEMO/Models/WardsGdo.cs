using System.ComponentModel.DataAnnotations;

namespace AB2EDEMO.Models
{
    public class WardsGdo
    {
        [Display(Name = "Hospital Code")]
        public string Hospital_Code { get; set; }
        [Display(Name = "Ward Code")]
        public string Ward_Code { get; set; }
        [Display(Name = "Ward Name")]
        public string Ward_Name { get; set; }
        [Display(Name = "Patients Per Ward")]
        public int Patient_Count { get; set; }
    }
}