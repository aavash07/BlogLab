using System;
using System.Collections.Generic;
using System.Text;

namespace BlogLab.Models.Photo
{
    public class Photo:PhotoCreate
    {
        public int PhotoID { get; set; }
        public int ApplicationUserID { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
