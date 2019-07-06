using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class BookingRuleController : BaseController
    {
        private readonly IBookingRuleRepository bookingRuleRepository;
        private readonly IViewService viewService;

        public BookingRuleController(IViewService viewService, IBookingRuleRepository bookingRuleRepository)
        {
            this.viewService = viewService;
            this.bookingRuleRepository = bookingRuleRepository;
        }

        public IActionResult Index()
        {
            var model = viewService.GetBookingRuleViewList();
            ViewData["Title"] = "Buchungsregeln";
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.ReturnUrl = "/BookingRule";
            if (string.IsNullOrEmpty(id))
            {
                return View(new BookingRule());
            }
            else
            {
                var bookingRule = bookingRuleRepository.GetBookingRule(id);
                return View(bookingRule);
            }
        }

        [HttpPost]
        public JsonResult Save([FromBody] BookingRule bookingRule)
        {
            bookingRuleRepository.SaveBookingRule(bookingRule);
            return new JsonResult("");
        }
    }
}