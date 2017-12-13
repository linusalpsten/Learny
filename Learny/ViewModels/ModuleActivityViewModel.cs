using Learny.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Learny.ViewModels
{
    public class ModuleActivityViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Aktivitet")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Modul")]
        public string ModuleName { get; set; }

        public int CourseModuleId { get; set; }

        [Display(Name = "Kurs")]
        public string FullCourseName { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "Typ")]
        public string ActivityTypeName { get; set; }

        public bool Edit { get; set; }

        public bool HaveDocuments { get; set; }

        public ModuleActivityViewModel() { }

        public ModuleActivityViewModel(ModuleActivity activity)
        {
            Id = activity.Id;
            Name = activity.Name;
            Description = activity.Description;
            StartDate = activity.StartDate;
            EndDate = activity.EndDate;
            ActivityTypeName = activity.ActivityType.Name;
            CourseId = activity.Module.CourseId;
            FullCourseName = activity.Module.Course.FullCourseName;
            HaveDocuments = activity.Documents.Count() > 0;
        }

    }
}