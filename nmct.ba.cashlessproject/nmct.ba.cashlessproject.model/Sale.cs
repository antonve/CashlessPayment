using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.ba.cashlessproject.model
{
    public class Sale : IDataErrorInfo
    {
        public int ID { get; set; }

        public int RegisterID { get; set; }

        public string RegisterName { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        [Required]
        public int Amount { get; set; }

        public double SinglePrice { get; set; }

        public double TotalPrice { get; set; }

        public DateTime Timestamp { get; set; }

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

        public override string ToString()
        {
            return ProductName;
        }
    }
}
