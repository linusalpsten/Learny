using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Learny.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Learny.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        [DateTimeToSqlDateTime]
        public DateTime TimeStamp { get; set; }

        [Required]
        public string FileName { get; set; }

        public int? CourseId { get; set; }

        public int? CourseModuleId { get; set; }

        public int? ModuleActivityId { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual Course Course { get; set; }

        public virtual CourseModule Module { get; set; }

        public virtual ModuleActivity Activity { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        //Display name returns FileName if Name is null, else it returns Name
        public string Extension { get { return new Regex(@"\.[^.]+$").Match(FileName).Value; } } // takes the last characters after the last dot (including the dot)
        public string FileNameWithoutExtension { get { return new Regex(@"(^.+)(?:\.[^.]+$)").Match(FileName).Captures[0].Value; } } // takes the first characters before the last dot (excluding the last dot)
        public string DisplayName { get { return Name ?? FileNameWithoutExtension; } }
    }
}