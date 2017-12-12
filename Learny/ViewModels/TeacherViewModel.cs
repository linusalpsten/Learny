using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class TeacherViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Lärare")]
        public string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "E-post är obligatoriskt")]
        [Display(Name = "E-post")]
        public string Email { get; set; }
        
        public TeacherViewModel() { }
        
        public TeacherViewModel(Models.ApplicationUser teacher)
        {
            Id = teacher.Id;
            Name = teacher.Name;
            Email = teacher.Email;
        }

    }
}