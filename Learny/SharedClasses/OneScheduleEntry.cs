using Learny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learny.SharedClasses
{
    public class OneScheduleEntry
    {
        public DateTime CurrentDate { get; set; }

        public string CurrentDay => CurrentDate.DayOfWeek.ToString();

        // Module name
        public string ModuleName { get; set; }

        public List<ModuleActivity> ActivityList { get; set; }

        //// Activity start date
        //public DateTime StartDate { get; set; }

        //// Activity end date
        //public DateTime EndDate { get; set; }


        // Activity name
        //public string ActivityName { get; set; }
    }
}