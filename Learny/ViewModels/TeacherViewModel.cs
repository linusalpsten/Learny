using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class TeacherViewModel
    {
        public string Id { get; set; }

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

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Det bekräftade lösenordet och lösenordet matchar inte.")]
        public string ConfirmPassword { get; set; }

        public bool Edit { get; set; }

        public TeacherViewModel() { }

        public TeacherViewModel(Models.ApplicationUser teacher)
        {
            Id = teacher.Id;
            Name = teacher.Name;
            Email = teacher.Email;
        }

    }
}