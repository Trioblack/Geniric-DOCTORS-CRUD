namespace AB2EDEMO.Models
{
    public class WardsPdo
    {
        public PagedItem<WardsGdo> Ward { get; set; }
        public string Country { get; set; }
        public string Hospital_Name { get; set; }
        public decimal Telephone_Number { get; set; }
    }
}