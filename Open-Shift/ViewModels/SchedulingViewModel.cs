using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Open_Shift.Models;

namespace Open_Shift.ViewModels
{
    public class SchedulingViewModel
    {
        public User User { get; set; }
        public SchedulingCalendar Calendar { get; set; }
    }
}