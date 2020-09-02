using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Models.Models;

namespace Teams.Models.Validators
{
    public class UserValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Convert.ToInt32(value) > 0)
            {
                return null;
            }
            else
            {
                return new ValidationResult("Please Assign User to Task", new[] { validationContext.MemberName });
            }
        }
    }
}
