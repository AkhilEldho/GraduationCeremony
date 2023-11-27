using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Versioning;
using OfficeOpenXml.Style;
using System.Collections;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Xml.Linq;
using X.PagedList;

namespace GraduationCeremony.Controllers
{
    public class CheckInController : Controller
    {
        private readonly S232_Project01_TestContext _context;

        public CheckInController(S232_Project01_TestContext context)
        {
            _context = context;
        }

        //CHECK IN HOME
        public IActionResult Index(string? message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return View();
            }
            else
            {
                ViewBag.Message = message;
                return View();
            }
        }

        // AUTO SUGGEST FOR CHECKIN 
        public string SearchGraduandByName(string searchString)
        {
            string sql = "SELECT Graduand.* FROM Graduand INNER JOIN Graduand_Award ON Graduand.Person_code = Graduand_Award.Person_code WHERE(Graduand.Forenames LIKE @p0 OR Graduand.Surname LIKE @p1) AND(Graduand_Award.awarded IS NULL OR Graduand_Award.awarded = '')";

            //allow to search similar to startswith for both forenames or surname
            string wrapSearchString = searchString + "%";

            List<Graduand> graduands = _context.Graduands.FromSqlRaw(sql, wrapSearchString, wrapSearchString).ToList();
            string json = JsonConvert.SerializeObject(graduands);

            return json;
        }

        // AUTO SUGGEST FOR CHECKEDIN GRADUANDS 
        public string SearchCheckedInGraduands(string searchString)
        {
            string sql = "SELECT * FROM CheckIn WHERE Forenames LIKE @p0 OR Surname LIKE @p1";

            //allow to search similar to startswith for both forenames or surname
            string wrapSearchString = searchString + "%";

            List<CheckIn> checkin = _context.CheckIns.FromSqlRaw(sql, wrapSearchString, wrapSearchString).ToList();
            string json = JsonConvert.SerializeObject(checkin);

            return json;
        }

        // SEARCHING FOR CHECKEDIN GRADUANDS
        public async Task<IActionResult> SearchCheckedIn(string searchString, int? page)
        {
            try
            {
                var pageNumber = page ?? 1;

                // Removing extra space  
                searchString = searchString?.Trim();
                string[] nameParts = searchString?.Split(' ');

                string firstName = "";
                string lastName = "";

                if (nameParts != null && nameParts.Length > 0)
                {
                    // if search string is a single character, use it for both first and last name
                    if (nameParts[0].Length == 1)
                    {
                        firstName = nameParts[0];
                        lastName = nameParts[0];
                    }
                    else
                    {
                        firstName = nameParts[0];
                        lastName = string.Join(" ", nameParts.Skip(1));
                    }
                }

                if (string.IsNullOrEmpty(searchString))
                {
                    // Handle empty or null search string
                    ViewBag.Message = "Error: Please enter a valid search string.";
                    return View("Index");
                }
                // search query using first name or last name 
                List<CheckIn> checkInRecord = await _context.CheckIns
                    .Where(x => (string.IsNullOrEmpty(firstName) || x.Forenames.ToLower().StartsWith(firstName.ToLower()) || x.Surname.ToLower().StartsWith(firstName.ToLower())) &&
                                (string.IsNullOrEmpty(lastName) || x.Forenames.ToLower().StartsWith(lastName.ToLower()) || x.Surname.ToLower().StartsWith(lastName.ToLower())))
                    .ToListAsync();

                if (checkInRecord != null && checkInRecord.Count > 0)
                {
                    //returning as ipagedlist
                    return View(checkInRecord.ToPagedList(pageNumber, 10));
                } else
                {
                    ViewBag.Message = "Student " + searchString + " not found. Please enter name correctly";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Index");
            }
            return View();
        }


        // SEARCHING SINGLE GRADUAND
        public async Task<IActionResult> SearchCheckIn(string searchString)
        {
            try
            {
                // Removing extra space  
                searchString = searchString?.Trim();
                string[] nameParts = searchString?.Split(' ');

                string firstName = "";
                string lastName = "";

                if (nameParts != null && nameParts.Length > 0)
                {
                    firstName = nameParts[0];

                    // Combine the remaining parts as the last name
                    lastName = string.Join(" ", nameParts.Skip(1));
                }

                // Getting all the relevant tables to search
                var graduants = _context.Graduands.AsQueryable();
                var graduantAwards = _context.GraduandAwards.AsQueryable();
                var awards = _context.Awards.AsQueryable();

                //CHANGED THINGS IN HERE
                // Search for the grad
                Graduand grad = await graduants
                    .Where(g => g.CollegeEmail.ToLower() == searchString.ToLower() ||
                    g.Forenames.ToLower() == firstName.ToLower() ||
                    g.Surname.ToLower() == lastName.ToLower())
                    .FirstOrDefaultAsync();

                if (grad != null)
                {
                    var gradAward = await graduantAwards
                        .Where(g => g.PersonCode == grad.PersonCode)
                        .FirstOrDefaultAsync();

                    var award = await awards
                        .Where(g => g.AwardCode == gradAward.AwardCode)
                        .FirstOrDefaultAsync();

                    if (gradAward != null && award != null)
                    {
                        var studentCheckIn = new CheckIn
                        {
                            PersonCode = grad.PersonCode,
                            Forenames = grad.Forenames,
                            Surname = grad.Surname,
                            Nsn = grad.Nsn,
                            AwardCode = award.AwardCode,
                            QualificationCode = award.QualificationCode,
                            AwardDescription = award.AwardDescription,
                            GraduandAwardId = gradAward.GraduandAwardId,
                            Level = award.Level,
                            DateOfBirth = grad.DateOfBirth,
                            Mobile = grad.Mobile,
                            CollegeEmail = grad.CollegeEmail,
                            Pronunciation = grad.Pronunciation,
                        };

                        var checkedIn = await _context.CheckIns
                            .Where(c => c.PersonCode == studentCheckIn.PersonCode)
                            .FirstOrDefaultAsync();

                        if (checkedIn != null)
                        {
                            ViewBag.Message = "Checked In";

                        }

                        return View(studentCheckIn);
                    }
                }

                // Handle the case when grad, gradAward, or award is null
                ViewBag.Message = "Student " + searchString + " not found. Please enter name correctly";
                return View("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Index");
            }
        }

        // CHECKING IN 
        public async Task<IActionResult> CheckIn(int PersonCode, string pronunciation)
        {
            try
            {
                // Get the details and add them to presenters list
                var grad = await _context.Graduands.FirstOrDefaultAsync(x => x.PersonCode == PersonCode);
                var gradAward = await _context.GraduandAwards.FirstOrDefaultAsync(x => x.PersonCode == PersonCode);
                var award = await _context.Awards.FirstOrDefaultAsync(x => x.AwardCode == gradAward.AwardCode);

                // Check if the student has an existing check-in record
                var student = await _context.CheckIns.FirstOrDefaultAsync(x => x.PersonCode == PersonCode);

                string namePronunciation = pronunciation;

                if(string.IsNullOrEmpty(namePronunciation))
                {
                    namePronunciation = grad.Pronunciation;
                }

                if (student == null)
                {
                    // Create a new check-in record if it doesn't exist
                    student = new CheckIn
                    {
                        PersonCode = PersonCode,
                        Forenames = grad.Forenames,
                        Surname = grad.Surname,
                        Nsn = grad.Nsn,
                        AwardCode = award.AwardCode,
                        QualificationCode = award.QualificationCode,
                        AwardDescription = award.AwardDescription,
                        Level = award.Level,
                        GraduandAwardId = gradAward.GraduandAwardId,
                        Major1 = gradAward.Major1,
                        Major2 = gradAward.Major2,
                        DateOfBirth = grad.DateOfBirth,
                        Mobile = grad.Mobile,
                        CollegeEmail = grad.CollegeEmail,
                        School = grad.School,
                        Pronunciation = namePronunciation,
                    };


                    // Check if a record with the same OrderInList already exists
                    if (await _context.CheckIns.AnyAsync(x => x.GraduandAwardId == student.GraduandAwardId))
                    {
                        ViewBag.Message = "Already Checked In";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pronunciation))
                            grad.Pronunciation = pronunciation;

                        // Add the new check-in record, order the list, and save changes
                        _context.CheckIns.Add(student);
                        _context.CheckIns.OrderBy(item => item.GraduandAwardId);

                        await _context.SaveChangesAsync();
                    }
                } else
                {
                    student.PersonCode = PersonCode;
                    student.Forenames = grad.Forenames;
                    student.Surname = grad.Surname;
                    student.Nsn = grad.Nsn;

                    student.AwardCode = award.AwardCode;
                    student.QualificationCode = award.QualificationCode;
                    student.AwardDescription = award.AwardDescription;
                    student.Level = award.Level;

                    student.GraduandAwardId = gradAward.GraduandAwardId;
                    student.Major1 = gradAward.Major1;
                    student.Major2 = gradAward.Major2;

                    student.DateOfBirth = grad.DateOfBirth;
                    student.Mobile = grad.Mobile;
                    student.CollegeEmail = grad.CollegeEmail;
                    student.Pronunciation = grad.Pronunciation;
                }

                return View(student);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Index");
            }
        }

        // LIST FOR THE STAFF TO SEE
        public async Task<IActionResult> CheckedInList(int? page)
        {
            // if no page was specified in the querystring, deafult to the first page
            var pageNumber = page ?? 1;

            //retrieve checkin table
            var checkInFull = from g in _context.CheckIns select g;
            if (checkInFull.Count() == 0)
            {
                ViewBag.Message = "No students have checked in yet";
            }
            List<CheckIn> checkIn = await checkInFull.ToListAsync();
            checkIn = checkIn.OrderBy(x => x.GraduandAwardId).ToList();

            return View(checkIn.ToPagedList(pageNumber, 10));
        }

        // PRESENTERS VIEW
        public async Task<IActionResult> Presenter()
        {
            var checkInFull = from g in _context.CheckIns select g;
            checkInFull = checkInFull.OrderBy(x => x.Level).ThenBy(item => item.AwardDescription).ThenBy(item => item.Forenames);

            List<CheckIn> checkInList = new List<CheckIn>();

            checkInList = await checkInFull.ToListAsync();

            return View(checkInList);
        }

        // JAVA SCRIPT UPDATE
        [HttpGet]
        public async Task<IActionResult> GetUpdatedPersons()
        {
            // the updated list of persons from your data source
            var updatedPersons = await _context.CheckIns.ToListAsync();

            return Json(updatedPersons);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePerson(string? PersonCode)
        {
            if (!string.IsNullOrEmpty(PersonCode))
            {
                int code = int.Parse(PersonCode);
                //saving awarded date to the day it's given
                List<GraduandAward> graduandAward = await _context.GraduandAwards.ToListAsync();
                GraduandAward awarded = graduandAward.Find(x => x.PersonCode == code);

                if (awarded.Awarded == null || string.IsNullOrEmpty(awarded.Awarded.ToString()))
                {
                    awarded.Awarded = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            var updatedPersons = await _context.CheckIns.ToListAsync();

            return Json(updatedPersons);
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

            var student = await _context.CheckIns.FirstOrDefaultAsync(m => m.PersonCode == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
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

            var student = await _context.CheckIns.FirstOrDefaultAsync(m => m.PersonCode == id);
            _context.CheckIns.Remove(student);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CheckedInList));
        }
    }
}
