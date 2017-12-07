using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.SharedClasses
{
    // one entry matches one single day!
    public class OneScheduleEntry
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Display(Name = "Time booked")]
        public DateTime CurrentDate { get; set; }

        public string CurrentDay => CurrentDate.DayOfWeek.ToString();

        // Module name
        public string ModuleName { get; set; }

        public List<string> ActivityNamesList { get; set; }

        //// Activity start date
        //public DateTime StartDate { get; set; }

        //// Activity end date
        //public DateTime EndDate { get; set; }


        // Activity name
        //public string ActivityName { get; set; }
    }
}