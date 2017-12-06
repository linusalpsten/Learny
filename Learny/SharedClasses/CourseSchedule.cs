using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learny.SharedClasses
{
    public class OneScheduleEntry
    {
       
        // Activity start date
        public DateTime StartDate { get; set; }
        
        // Activity end date
        public DateTime EndDate { get; set; }

        // Module name
        public string ModuleName { get; set; }

        // Activity name
        public string ActivityName { get; set; }
    }
}