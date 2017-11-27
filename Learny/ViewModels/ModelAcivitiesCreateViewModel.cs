﻿using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Learny.ViewModels
{
    public class ModelAcivitiesCreateViewModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name="Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        
        public int CourseModuleId { get; set; }

        public int ActivityTypeId { get; set; }

        public virtual ActivityType ActivityType { get; set; }

        [Display(Name = "Aktivitetstyp")]
        public List<ActivityType> ActivityTypes { get; set; }

    }
}