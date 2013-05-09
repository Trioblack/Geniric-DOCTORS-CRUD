using System.Collections.Generic;

//<summary>
//The Code File has been made to get paged item results//
//Here, the code for paged item class and a data access class has been written //
//this class is conroller specific rather than being a generic class applicable to all the controllers
//</summary>


namespace AB2EDEMO.Models
{
    public class PagedItem<T>     //generic class to attach entities with other properties
    {
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public List<T> Entities { get; set; }
    }

}