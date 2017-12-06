using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learny.Shared_classes
{
    public class CourseSchedule
    {
        public int CourseId { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }
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