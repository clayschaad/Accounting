using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class BookingTextController : BaseController
    {
        private readonly IBookingTextRepository bookingTextRepository;

        public BookingTextController(IBookingTextRepository bookingTextRepository)
        {
            this.bookingTextRepository = bookingTextRepository;
        }

        public IActionResult Index()
        {
            var model = bookingTextRepository.GetBookingTextList();
            ViewData["Title"] = "Buchungstexte";
            return View(model);
        }

        public IEnumerable<string> GetBookingTextList()
        {
            var model = bookingTextRepository.GetBookingTextList();
            return model.Select(a => a.Text);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.ReturnUrl = "/BookingText";
            if (string.IsNullOrEmpty(id))
            {
                return View(new BookingText());
            }
            else
            {
                var bookingText = bookingTextRepository.GetBookingText(id);
                return View(bookingText);
            }
        }

        [HttpPost]
        public JsonResult Save([FromBody] BookingText bookingText)
        {
            bookingTextRepository.SaveBookingText(bookingText);
            return new JsonResult("");
        }
    }
}