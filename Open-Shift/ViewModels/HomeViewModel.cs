
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Open_Shift.Models;

namespace Open_Shift.ViewModels
{
    public class HomeViewModel
    {
        public User User { get; set; }
        public Shift NextShift { get; set; } = null;
        public Shift CurrentShift { get; set; } = null;
        public List<Store> Stores { get; set; } = null;
    }
}