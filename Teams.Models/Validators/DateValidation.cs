using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Models.Models;

namespace Teams.Models.Validators
{
    public class DateValidation: ValidationAttribute
    {
        public bool isEndate { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool customValidate = validationContext.ObjectType == typeof(TaskModel) && ((TaskModel)validationContext.ObjectInstance).StartDate <= (Convert.ToDateTime(value));
            if ((Convert.ToDateTime(value))> new DateTime(2020,7,1))
            {
                if (isEndate == false || customValidate)
                {
                    return null;
                }
                else
                {
                    Regex r = new Regex("([A-Z]+[a-z]+)");
                    string pascalCase = r.Replace(validationContext.MemberName, m => (m.Value.Length > 3 ? m.Value : m.Value) + " ");
                    return new ValidationResult($"{pascalCase} must be later than Start Date", new[] { validationContext.MemberName });
                }
               
            }
            else
            {
                Regex r = new Regex("([A-Z]+[a-z]+)");
                string pascalCase = r.Replace(validationContext.MemberName, m => (m.Value.Length > 3 ? m.Value : m.Value) + " ");
                return new ValidationResult($"{pascalCase} must be later than 07/01/2020", new[] { validationContext.MemberName });
            }
        }
    }
}
