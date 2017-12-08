using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class CourseCreateViewModel : CourseViewModel
    {
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        public new DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        public new DateTime EndDate { get; set; }

    }
}