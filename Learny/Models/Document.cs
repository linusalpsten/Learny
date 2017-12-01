using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Learny.DataAnnotations;
using System.ComponentModel.DataAnnotations;

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

        public string Name { get; set; }
        public string Description { get; set; }
        //Display name returns FileName if Name is null, else it returns Name
        public string DisplayName { get { return Name ?? FileName; } }
    }
}