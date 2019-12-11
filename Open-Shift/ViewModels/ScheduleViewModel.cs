using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Open_Shift.Models;

namespace Open_Shift.ViewModels
{
    public class ScheduleViewModel
    {
        public User User { get; set; }
        public List<Availability> Availabilities { get; set; }
        public List<Shift> Shifts { get; set; }
    }
}