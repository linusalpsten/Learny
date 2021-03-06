﻿using Foolproof;
using Learny.DataAnnotations;
using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Learny.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Modul")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        [DateTimeToSqlDateTime]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        [DateTimeToSqlDateTime]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "Kurs")]
        public string FullCourseName { get; set; }

        [Display(Name = "Aktivitet")]
        public virtual ICollection<ModuleActivity> Activities { get; set; }

        public bool EditMode { get; set; }
        public bool ShowModuleList { get; set; }

        public bool HaveDocuments { get; set; }

        public ModuleViewModel() { }

        public ModuleViewModel(CourseModule module)
        {
            Id = module.Id;
            Name = module.Name;
            Description = module.Description;
            StartDate = module.StartDate;
            EndDate = module.EndDate;
            CourseId = module.CourseId;
            FullCourseName = module.Course.FullCourseName;
            Activities = module.Activities.OrderBy(a => a.StartDate).ToList();
            HaveDocuments = module.Documents.Count() > 0;
        }
    }
}