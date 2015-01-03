using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Employee : IDataErrorInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "The Employee name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The Employee name must be between 1 and 50 characters long.")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Address field is required.")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 255 characters long.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email field is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone field is required.")]
        public string Phone { get; set; }

        public bool isNew { get; set; }

        public string Error
        {
            get { return null; }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null), null, true);
        }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = columnName });
                }
                catch (ValidationException ex)
                {

                    return ex.Message;
                }
                return String.Empty;
            }
        }
    }
}
