using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class SubClassController : BaseController
    {
        private readonly ISubclassRepository subclassRepository;

        public SubClassController(ISubclassRepository subclassRepository)
        {
            this.subclassRepository = subclassRepository;
        }

        public IActionResult Index()
        {
            var model = subclassRepository.GetSubClassList();
            ViewData["Title"] = "Klassen";
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.ReturnUrl = "/SubClass";
            if (string.IsNullOrEmpty(id))
            {
                return View(new SubClass());
            }
            else
            {
                var subclass = subclassRepository.GetSubClass(id);
                return View(subclass);
            }
        }

        [HttpPost]
        public JsonResult Save([FromBody] SubClass subClass)
        {
            subclassRepository.SaveSubClass(subClass);
            return new JsonResult("");
        }
    }
}