using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class StudentViewModel
    {

        public string Id { get; set; }

        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "E-post är obligatoriskt")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Kurs id")]
        [Required(ErrorMessage = "Välj en kurs från kurslistan.")]
        public int CourseId { get; set; }

        [Display(Name = "Kurs")]
        public string CourseName { get; set; }

        [Display(Name = "Kurser")]
        public List<Course> Courses { get; set; }

    }
}