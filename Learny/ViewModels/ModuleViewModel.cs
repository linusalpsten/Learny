using Learny.DataAnnotations;
using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Foolproof;

namespace Learny.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt")]
        [Display(Name = "Startdatum")]
        [DateTimeToSqlDateTime]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt")]
        [Display(Name = "Slutdatum")]
        [DateTimeToSqlDateTime]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

    }
}