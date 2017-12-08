using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class ActivityType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ShortName { get; set; }
    }
}