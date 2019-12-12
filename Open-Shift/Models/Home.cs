using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Open_Shift.Models
{
    public class Home
    {
        public User ViewUser = new User();
        public User User = new User();
        public SystemLists Lists = new SystemLists();
        public string Status = "";
    }
}