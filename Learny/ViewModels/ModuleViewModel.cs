using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

    }
}