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
            //getting all the relevant tables to search
            var graduants = from g in _context.Graduands select g;
            var graduantAwards = from g in _context.GraduandAwards select g;
            var awards = from g in _context.Awards select g;

            if (!String.IsNullOrEmpty(searchString))
            {
                //converting db to list for easier search
                List<Graduand> grads = await graduants.ToListAsync();
                List<GraduandAward> gradAwards = await graduantAwards.ToListAsync();
                List<Award> awardsList = await awards.ToListAsync();

                Graduand grad = new Graduand();
                GraduandAward gradAward = new GraduandAward();
                Award award = new Award();

                //searching the grad
                grad = grads.Find(g => g.CollegeEmail.Contains(searchString));

                if(grad != null)
                {
                    gradAward = gradAwards.Find(g => g.PersonCode == grad.PersonCode);
                    award = awardsList.Find(g => g.AwardCode == gradAward.AwardCode);

                    //Saving them as checkin object
                    //might be able to simplify it 
                    CheckIn studentCheckIn = new CheckIn();
                    studentCheckIn.PersonCode = grad.PersonCode;
                    studentCheckIn.Forenames = grad.Forenames;
                    studentCheckIn.Surname = grad.Surname;
                    studentCheckIn.Nsn = grad.Nsn;

                    studentCheckIn.AwardCode = award.AwardCode;
                    studentCheckIn.QualificationCode = award.QualificationCode;
                    studentCheckIn.AwardDescription = award.AwardDescription;
                    studentCheckIn.Level = award.Level;

                    studentCheckIn.DateOfBirth = grad.DateOfBirth;
                    studentCheckIn.Mobile = int.Parse(grad.Mobile);
                    studentCheckIn.CollegeEmail = grad.CollegeEmail;

                    List<CheckIn> checksIn = new List<CheckIn>();
                    checksIn.Add(studentCheckIn);

                    return View(checksIn);
                }
                else
                {
                    return View(grads);
                }
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

        //Function which adds the student to the presenters list
        public async Task<IActionResult> checkIn(int PersonCode)
        {
            //get the details and add it to presenters list
            var graduants = from g in _context.Graduands select g;
            var graduantAwards = from g in _context.GraduandAwards select g;
            var awards = from g in _context.Awards select g;

            List<Graduand> grads = await graduants.ToListAsync();
            List<GraduandAward> gradAwards = await graduantAwards.ToListAsync();
            List<Award> awardsList = await awards.ToListAsync();

            Graduand grad = grads.Find(x => x.PersonCode == PersonCode);
            GraduandAward gradAward = gradAwards.Find(x => x.PersonCode == PersonCode);
            Award award = awardsList.Find(x => x.AwardCode == gradAward.AwardCode);

            if (grad != null)
            {
                CheckIn student = new CheckIn();
                student.PersonCode = PersonCode;
                student.Forenames = grad.Forenames;
                student.Surname = grad.Surname;
                student.Nsn = grad.Nsn;

                student.AwardCode = award.AwardCode;
                student.QualificationCode = award.QualificationCode;
                student.AwardDescription = award.AwardDescription;
                student.Level = award.Level;

                student.DateOfBirth = grad.DateOfBirth;
                student.Mobile = int.Parse(grad.Mobile);
                student.CollegeEmail = grad.CollegeEmail;

                _context.CheckIns.Add(student);
                _context.SaveChanges();

            }

            return CheckedIn();
        }

        public IActionResult CheckedIn()
        {
            return View();
        }

        //The view for the presenters
        public async Task<IActionResult> CheckedInList()
        {
            var checkInFull = from g in _context.CheckIns select g;
            
            List<CheckIn> checkIn = new List<CheckIn>();
            checkIn = await checkInFull.ToListAsync();

            return View(checkIn);
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
