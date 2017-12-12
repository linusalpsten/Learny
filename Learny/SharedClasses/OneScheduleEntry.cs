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
        public DateTime CurrentDate { get; set; }

        public string CurrentDay => CurrentDate.DayOfWeek.ToString();

        public List<ModuleActivity> Activities { get; set; }
        
    }
}