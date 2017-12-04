using Learny.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class StudentListViewModel
    {

        [Display(Name = "Elever")]
        public ICollection<ApplicationUser> Students { get; set; }
    }
}