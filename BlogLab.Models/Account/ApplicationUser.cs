using System;
using System.Collections.Generic;
using System.Text;

namespace BlogLab.Models
{
    public class ApplicationUser
    {
        public int ApplicationUserID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
