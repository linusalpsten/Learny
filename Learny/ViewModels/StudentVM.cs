using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class StudentVM
    {
        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "E-post är obligatoriskt")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "{0} måste ha minst minst {2} och max {1} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} måste ha minst en icke bokstav, ett tal, en versal('A' - 'Z') och bestå av minst 6 tecken.")]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kurs id")]
        [Required(ErrorMessage ="Välj en kurs från kurslistan.")]
        public int CourseId { get; set; }

        [Display(Name = "Kurs")]
        public string AttendingCourse { get; set; }

        public Boolean CourseSelected { get; set; }

        [Display(Name = "Kurser")]
        public List<Course> Courses { get; set; }

    }
}