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
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Mejl")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} måste ha minst en icke bokstav, ett tal, en versal('A' - 'Z') och bestå av minst {2} bokstäver.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        //  public string CourseCode { get; set; }
        public int CourseId { get; set; }

        [Display(Name = "Kurser")]
        public List<Course> Courses { get; set; }
    }
}