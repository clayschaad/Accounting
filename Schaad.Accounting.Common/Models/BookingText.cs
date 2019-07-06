using System.ComponentModel.DataAnnotations;

namespace Schaad.Accounting.Models
{
    public class BookingText
    {
        public string Id { get; set; }

        [Display(Name = "Buchungstext")]
        public string Text { get; set; }

        /// <summary>
        /// Makes a copy
        /// </summary>
        public void Copy(BookingText target)
        {
            target.Id = Id;
            target.Text = Text;
        }
    }
}