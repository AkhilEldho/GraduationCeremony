using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System.Collections;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Principal;

namespace GraduationCeremony.Controllers
{
    public class CheckInController : Controller
    {
        private readonly S232_Project01_TestContext _context;

        public CheckInController(S232_Project01_TestContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchCheckIn(string searchString)
        {
            var graduants = from g in _context.Graduations select g;
            if (!String.IsNullOrEmpty(searchString))
            {
                var grads = await graduants.ToListAsync();

                graduants = graduants.Where(s => s.CollegeEmail.Contains(searchString));

                grads = grads
                        .OrderBy(item => item.Level)
                        .ThenBy(item => item.AwardDescription)
                        .ThenBy(item => item.Forenames).ToList();

                return View(grads);
            }
            else
            {
                var graduations = await _context.Graduations.ToListAsync();

                if (graduations != null)
                {
                    graduations = graduations
                        .OrderBy(item => item.Level)
                        .ThenBy(item => item.AwardDescription)
                        .ThenBy(item => item.Forenames).ToList();
                }

                return View(graduations);
            }

            return View();
        }

        public async void checkIn(int personCode)
        {
            var graduants = from g in _context.Graduations select g;
            var grads = await graduants.ToListAsync();

            graduants = graduants.Where(s => s.PersonCode == personCode);

            Graduation student = (Graduation)graduants.Where(s => s.PersonCode == personCode);

            CheckedIn();

        }

        public IActionResult CheckedIn()
        {
            return View();
        }

        //TEST URL: localhost:7204/CheckIn/CheckedInList
        public IActionResult CheckedInList()
        {

            return View();
        }


        // GET: CheckInController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        // GET: CheckInController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CheckInController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CheckInController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CheckInController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CheckInController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CheckInController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
