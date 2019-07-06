using System.ComponentModel.DataAnnotations;

namespace Schaad.Accounting.Models
{
    public class SubClass
    {
        public string Id { get; set; }

        [Display(Name = "Nummer")]
        public int Number { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Makes a copy
        /// </summary>
        public void Copy(SubClass target)
        {
            target.Id = Id;
            target.Name = Name;
            target.Number = Number;
        }
    }
}