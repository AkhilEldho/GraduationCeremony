using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using OfficeOpenXml.Style;
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
            //removing extra space  
            searchString = searchString?.Trim();

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
                    studentCheckIn.Mobile = grad.Mobile;
                    studentCheckIn.CollegeEmail = grad.CollegeEmail;

                    var checkedIn = from c in _context.CheckIns select c;
                    List<CheckIn> list = await checkedIn.ToListAsync();
                    CheckIn stud = list.Find(c => c.PersonCode == studentCheckIn.PersonCode);

                    if(stud != null)
                    {
                        ViewBag.Message = "Checked In";
                    }

                    return View(studentCheckIn);
                }
                else
                {
                    return View(grads);
                }
            }
            else if (String.IsNullOrEmpty(searchString))
            {
                //show error message
            }
            else
            {
                //show error message
            }

            return View();
        }

        //Function which adds the student to the presenters list
        public async void checkIn(int PersonCode)
        {
            //get the details and add it to presenters list
            var graduants = from g in _context.Graduands select g;
            var graduantAwards = from g in _context.GraduandAwards select g;
            var awards = from g in _context.Awards select g;
            var checkIn = from g in _context.CheckIns select g;

            List<Graduand> grads = await graduants.ToListAsync();
            List<GraduandAward> gradAwards = await graduantAwards.ToListAsync();
            List<Award> awardsList = await awards.ToListAsync();
            List<CheckIn> checkInList = await checkIn.ToListAsync();

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
                student.Mobile = grad.Mobile;
                student.CollegeEmail = grad.CollegeEmail;
                student.OrderInList = gradAward.GraduandAwardId;

                if(checkInList.Find(x => x.OrderInList == student.OrderInList) == null)
                {
                    _context.CheckIns.Add(student);
                    _context.CheckIns.OrderBy(item => item.OrderInList);
                    _context.SaveChanges();

                    CheckedIn(student.OrderInList);
                }
                else
                {
                    ViewBag.Message = "Already Checked In";
                }
            }
            else
            {
                ViewBag.Message = "Student Not Found";
            }
        }

        public async Task<IActionResult> CheckedIn(int order)
        {
            var checkInFull = from g in _context.CheckIns select g;
            List<CheckIn> checkInList = new List<CheckIn>();
            checkInList = await checkInFull.ToListAsync();

            CheckIn student = checkInList.FirstOrDefault(x => x.OrderInList == order);

            return View(student);
        }

        //view with next button
        public async Task<IActionResult> Presenter()
        {
            var checkInFull = from g in _context.CheckIns select g;
            checkInFull = checkInFull.OrderBy(x => x.Level).ThenBy(item => item.AwardDescription).ThenBy(item => item.Forenames);
            List<CheckIn> checkInList = new List<CheckIn>();
            checkInList = await checkInFull.ToListAsync();

            return View(checkInList);
        }

        [HttpGet]
        public IActionResult GetUpdatedPersons()
        {
            // Fetch the updated list of persons from your data source
            var updatedPersons = _context.CheckIns.ToListAsync();

            return Json(updatedPersons);
        }


        //The view for the presenters
        public async Task<IActionResult> CheckedInList()
        {
            var checkInFull = from g in _context.CheckIns select g;

            List<CheckIn> checkIn = new List<CheckIn>();
            checkIn = await checkInFull.ToListAsync();
            checkIn = checkIn.OrderBy(x => x.OrderInList).ToList();

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
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == null || _context.CheckIns == null)
            {
                return NotFound();
            }

            var checkInFull = await _context.CheckIns.FirstOrDefaultAsync(m => m.PersonCode == id);

            if (checkInFull == null)
            {
                return NotFound();
            }

            return View(checkInFull);
        }

        // POST: Graduation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CheckIns == null)
            {
                return Problem("Entity set 'S232_Project01_TestContext.CheckIn'  is null.");
            }
            var student = await _context.CheckIns.FindAsync(id);
            if (student != null)
            {
                _context.CheckIns.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
