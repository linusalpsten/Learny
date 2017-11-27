using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace Learny.DataAnnotations
{
    public sealed class DateTimeToSqlDateTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Convert.ToDateTime(value) < (DateTime)SqlDateTime.MinValue)
            {
                return new ValidationResult("Startdatum får inte vara mindre än " + SqlDateTime.MinValue.ToString());
            }
            else if (Convert.ToDateTime(value) > (DateTime)SqlDateTime.MaxValue)
            {
                return new ValidationResult("Startdatum får inte vara större än" + SqlDateTime.MaxValue.ToString());
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}