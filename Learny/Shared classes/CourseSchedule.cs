using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learny.Shared_classes
{
    public class CourseSchedule
    {
        // Module start date
        public DateTime StartDate { get; set; }
        
        // Module end date
        public DateTime EndDate { get; set; }

        // Module name
        public string ModuleName { get; set; }

        // Activity name
        public string ActivityName { get; set; }
    }
}