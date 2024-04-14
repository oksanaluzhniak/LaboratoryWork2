using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
namespace l2
{
    public class Person
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Date { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailAdrress { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone.")]
        public string MobilePhone { get; set; }
        public string Location { get; set; }
        public string Sex { get; set; }
        public string Age
        {
            get

            {
                var dateTime = DateTime.Parse(this.Date);
                int age = (DateTime.Today.Year - dateTime.Year);
                if (DateTime.Today.DayOfYear < dateTime.DayOfYear)
                {
                    age--;
                }
                return age.ToString();
            }
        }
    }
}
