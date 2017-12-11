using Learny.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Learny.ViewModels
{
    public class CourseDetailsViewModel : CourseViewModel
    {
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        public bool HaveDocuments { get; set; }

        public CourseDetailsViewModel() { }

        public CourseDetailsViewModel(Course course)
        {
            Id = course.Id;
            Name = course.Name;
            CourseCode = course.CourseCode;
            FullCourseName = course.FullCourseName;
            StartDate = course.StartDate;
            EndDate = course.EndDate;
            Modules = course.Modules.OrderBy(m => m.StartDate).ToList();
            HaveDocuments = course.Documents.Count() > 0;
        }
    }
}