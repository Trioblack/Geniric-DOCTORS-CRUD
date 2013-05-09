﻿using System.ComponentModel.DataAnnotations;

namespace AB2EDEMO.Models
{
    public class HospitalGdo
    {
        private string _hospital_code = "";
        private string _hopital_name = "";
        private string _address_street = "";
        private string _address_town = "";
        private string _address_province = "";
        private string _address_post_zip = "";
        private decimal _telephone_number;
        private decimal _fax_number;
        private string _country = "";
        
        [Display(Name = "Hospital Code")]
        public string Hospital_Code
        {
            get { return _hospital_code; }
            set { if (value != null) _hospital_code = value; }
        }
        [Display(Name = "Hospital Name")]
        public string Hospital_Name
        {
            get { return _hopital_name; }
            set { if (value != null) _hopital_name = value; }
        }
        [Display(Name = "Street Address")]
        public string Address_Street
        {
            get { return _address_street; }
            set { if (value != null) _address_street = value; }
        }
        [Display(Name = "Towm")]
        public string Address_Town
        {
            get { return _address_town; }
            set { if (value != null) _address_town = value; }
        }
        [Display(Name = "Province Address")]
        public string Address_Province
        {
            get { return _address_province; }
            set { if (value != null) _address_province = value; }
        }
        [Display(Name = "Zip Code")]
        public string Address_Post_Zip
        {
            get { return _address_post_zip; }
            set { if (value != null) _address_post_zip = value; }
        }
        [Display(Name = "Telephone No.")]
        public decimal? Telephone_Number
        {
            get { return _telephone_number; }
            set { if (value != null) _telephone_number = (decimal)value; }
        }
        [Display(Name = "Fax No.")]
        public decimal? Fax_Number
        {
            get { return _fax_number; }
            set { if (value != null) _fax_number = (decimal)value; }
        }
        [Display(Name = "Country")]
        public string Country
        {
            get { return _country; }
            set { if (value != null) _country = value; }
        }
        [Display(Name = "Total Wards")]
        public string Total_Wards { get; set; }
        [Display(Name = "Total Patients")]
        public string Total_Patient { get; set; }
    }
}