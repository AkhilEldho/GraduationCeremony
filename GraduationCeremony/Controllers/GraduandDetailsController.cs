using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;
using X.PagedList;

namespace GraduationCeremony.Controllers
{
    public class GraduandDetailsController : Controller
    {
        private readonly S232_Project01_TestContext _context;
        private readonly ILogger<GraduandDetailsController> _logger;
        private string search = "";

        public GraduandDetailsController(S232_Project01_TestContext context, ILogger<GraduandDetailsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;

            var result = (from g in _context.Graduands
                          join ga in _context.GraduandAwards on g.PersonCode equals ga.PersonCode
                          join a in _context.Awards on ga.AwardCode equals a.AwardCode
                          select new GraduandDetails
                          {
                              graduands = g,
                              awards = a,
                              graduandAwards = ga
                          })
                         .OrderBy(item => item.awards.Level)
                        .ThenBy(item => item.awards.AwardDescription)
                        .ThenBy(item => item.graduands.Forenames).ToList();

            if (result.Count() == 0)
            {
                ViewBag.Message = "No excel file imported yet";
            }

            var yearList = result
                .Where(grad => grad.graduandAwards.Awarded != null)
                .Select(grad => grad.graduandAwards.Awarded.Value.Year)
                .Distinct() // To get unique years
                .ToList();

            ViewBag.YearList = new SelectList(yearList);

            return View(result.ToPagedList(pageNumber, 15));
        }

        // AUTO SUGGEST FOR GRADUANDDETAILS 
        public string SearchGraduandDetailsByName(string searchString)
        {
            string sql = "SELECT * FROM Graduand WHERE Forenames LIKE @p0 OR Surname LIKE @p1";

            string wrapSearchString = searchString + "%";

            var graduandDetailsList = _context.Graduands
            .FromSqlRaw(sql, wrapSearchString, wrapSearchString)
            .Select(g => new GraduandDetails
            {
                PersonCode = g.PersonCode,
                graduands = g
            })
            .ToList();

            string json = JsonConvert.SerializeObject(graduandDetailsList);

            return json;
        }

        public async Task<IActionResult> Search(string searchString, int? page, string? selectedYear)
        {
            try
            {
                var pageNumber = page ?? 1;
                ViewData["SearchString"] = searchString;
                ViewData["SelectedYear"] = selectedYear;

                if (!string.IsNullOrEmpty(searchString))
                {
                    search = searchString;
                }
                else
                {
                    searchString = search;
                }

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

                List<GraduandDetails> result = result = await (from g in _context.Graduands
                                                               join ga in _context.GraduandAwards on g.PersonCode equals ga.PersonCode
                                                               join a in _context.Awards on ga.AwardCode equals a.AwardCode
                                                               select new GraduandDetails
                                                               {
                                                                   graduands = g,
                                                                   awards = a,
                                                                   graduandAwards = ga
                                                               })
                      .OrderBy(item => item.awards.Level)
                      .ThenBy(item => item.awards.AwardDescription)
                      .ThenBy(item => item.graduands.Forenames)
                      .ToListAsync();

                List<GraduandDetails> searchList = result;

                if (string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(selectedYear))
                {
                    searchList = result
                        .Where(item => item.graduandAwards.Awarded != null && item.graduandAwards.Awarded.Value.Year == int.Parse(selectedYear))
                        .ToList();
                }
                else if(!string.IsNullOrEmpty(searchString)) 
                {
                    //searching by first name / last name
                    searchList = await (from g in _context.Graduands
                                    join ga in _context.GraduandAwards on g.PersonCode equals ga.PersonCode
                                    join a in _context.Awards on ga.AwardCode equals a.AwardCode
                                    where (string.IsNullOrEmpty(firstName) || g.Forenames.ToLower().StartsWith(firstName.ToLower()) || g.Surname.ToLower().StartsWith(firstName.ToLower())) &&
                                          (string.IsNullOrEmpty(lastName) || g.Forenames.ToLower().StartsWith(lastName.ToLower()) || g.Surname.ToLower().StartsWith(lastName.ToLower()))
                                    select new GraduandDetails
                                    {
                                        graduands = g,
                                        awards = a,
                                        graduandAwards = ga
                                    })
                          .OrderBy(item => item.awards.Level)
                          .ThenBy(item => item.awards.AwardDescription)
                          .ThenBy(item => item.graduands.Forenames)
                          .ToListAsync();
                }
                else if (string.IsNullOrEmpty(searchString))
                {
                    // Handle empty or null search string
                    ViewBag.Message = "Error: Please enter a valid search string.";
                    return View("Index");
                }

                if (result.Count == 0)
                {
                    // No results found
                    ViewBag.Message = "Student " + searchString + " not found. Please enter name correctly";
                }

                //getting the years of graduaded students
                var yearList = result
                    .Where(grad => grad.graduandAwards.Awarded != null)
                    .Select(grad => grad.graduandAwards.Awarded.Value.Year)
                    .Distinct() // To get unique years
                    .ToList();

                ViewBag.YearList = new SelectList(yearList);

                return View(searchList.ToPagedList(pageNumber, 15));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                ViewBag.Message = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Edit(int? personCode)
        {
            try
            {
                if (personCode == null)
                {
                    return NotFound();
                }

                // Creating a new GraduandDetails ViewModel
                var graduandDetailsVM = new GraduandDetails();

                // Check if the personCode exists in the GraduandAwards table
                var gradAwards = _context.GraduandAwards
                    .Include(g => g.PersonCodeNavigation) // accessing graduands table
                    .Include(a => a.AwardCodeNavigation)  // accessing awards table
                    .SingleOrDefault(ga => ga.PersonCode == personCode);

                if (gradAwards == null)
                {
                    //COPY PASTED THIS FROM ONLINE
                    // Log the occurrence for debugging purposes
                    _logger.LogError($"Edit: GraduandAwards not found for PersonCode {personCode}");

                    // Return a user-friendly error page or redirect to an error page
                    return View("Index");
                }

                graduandDetailsVM.graduandAwards = gradAwards;
                graduandDetailsVM.graduands = gradAwards.PersonCodeNavigation; // populating graduand attributes
                graduandDetailsVM.awards = gradAwards.AwardCodeNavigation;     // populating award attributes

                return View(graduandDetailsVM);
            }
            catch (Exception ex)
            {
                //COPY PASTED THIS FROM ONLINE
                // Log the exception for further investigation
                _logger.LogError($"Edit: An error occurred - {ex.Message}");

                // Return a user-friendly error page or redirect to an error page
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GraduandDetails graduandDetailsVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Remove spaces in award code if user added any
                    graduandDetailsVM.graduandAwards.AwardCode = graduandDetailsVM.graduandAwards.AwardCode?.Trim();

                    // Fetch existing entities from the database
                    var grad = _context.Graduands.SingleOrDefault(g => g.PersonCode == graduandDetailsVM.graduandAwards.PersonCode);
                    var gradAwards = _context.GraduandAwards.SingleOrDefault(ga => ga.PersonCode == graduandDetailsVM.graduandAwards.PersonCode);
                    var award = _context.Awards.SingleOrDefault(a => a.AwardCode == graduandDetailsVM.awards.AwardCode);

                    // Check if entities exist in the database
                    if (grad != null && award != null && gradAwards != null)
                    {
                        // Update graduand properties
                        grad.Forenames = graduandDetailsVM.graduands.Forenames;
                        grad.Surname = graduandDetailsVM.graduands.Surname;
                        grad.DateOfBirth = graduandDetailsVM.graduands.DateOfBirth;
                        grad.Pronunciation = graduandDetailsVM.graduands.Pronunciation;

                        // Update award properties
                        award.AwardDescription = graduandDetailsVM.awards.AwardDescription;
                        award.QualificationCode = graduandDetailsVM.awards.QualificationCode;
                        award.Level = graduandDetailsVM.awards.Level;

                        // Update graduand awards properties
                        gradAwards.Major1 = graduandDetailsVM.graduandAwards.Major1;
                        gradAwards.Major2 = graduandDetailsVM.graduandAwards.Major2;

                        // Check if the user changed the award code 
                        if (gradAwards.AwardCode != graduandDetailsVM.graduandAwards.AwardCode)
                        {
                            // Find if there is a matching award code in the database that the user entered
                            var newAward = _context.Awards.SingleOrDefault(a => a.AwardCode == graduandDetailsVM.graduandAwards.AwardCode);

                            if (newAward != null)
                            {
                                // Update graduandAward award code with the new award code
                                gradAwards.AwardCode = graduandDetailsVM.graduandAwards.AwardCode;

                                // Update award attributes to get the new award descriptions, etc.
                                gradAwards.AwardCodeNavigation = newAward;
                            }
                            else
                            {
                                // Add a model state error for invalid award code
                                ModelState.AddModelError("graduandAwards.AwardCode", "Invalid Award Code entered.");
                                return View(graduandDetailsVM);
                            }
                        }

                        // Save changes to the database
                        _context.SaveChanges();

                        // Redirect to the Index action after successful update
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Return a not found view if entities are not found in the database
                        return NotFound();
                    }
                }
                // ModelState is not valid, return the current view with validation errors
                return View(graduandDetailsVM);
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                _logger.LogError($"An error occurred while processing the Edit request - {ex.Message}");

                // Return a different view with an error message or redirect to an error page
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }


    }
}
