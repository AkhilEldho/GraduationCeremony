using GraduationCeremony.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using OfficeOpenXml;
using GraduationCeremony.Models.DB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.PortableExecutable;


namespace GraduationCeremony.Controllers
{
    public class HomeController : Controller
    {
        private readonly S232_Project01_TestContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(S232_Project01_TestContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        //import excel view
        [Authorize(Roles = "Staff")]
        public IActionResult ImportExcel()
        {
            return View();
        }


        //based from: https://medium.com/c-sharp-progarmming/convert-excel-to-data-table-in-asp-net-core-using-ep-plus-b59533e162b3
        [HttpPost]
        public IActionResult Upload(FileUploadModel model)
        {
            try
            {
                if (model.File != null)
                {
                    using (var stream = model.File.OpenReadStream())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelPackage package = new ExcelPackage();
                        package.Load(stream);

                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                            if (worksheet != null && worksheet.Dimension != null)
                            {
                                var awards = new List<Award>();
                                var graduands = new List<Graduand>();

                                var errors = new HashSet<string>();

                                int noOfRow = worksheet.Dimension.End.Row;

                                var processedAwardCodes = new HashSet<string>();

                                for (int r = 2; r <= noOfRow; r++)
                                {
                                    var award = ExtractAward(worksheet, r);
                                    var graduand = ExtractGraduand(worksheet, r);

                                    if (award != null)
                                    {
                                        if (processedAwardCodes.Add(award.AwardCode))
                                        {
                                            var awardErrors = ValidateAward(award);

                                            if (awardErrors.Count == 0)
                                            {
                                                awards.Add(award);
                                                graduands.Add(graduand);
                                            }
                                            else
                                            {
                                                errors.UnionWith(awardErrors);
                                            }
                                        }
                                    }
                                    else
                                    { 
                                        errors.Add("Award not found for row " + r);
                                    }
                                }

                                if (errors.Count == 0)
                                {
                                    _context.Awards.AddRange(awards);
                                    _context.Graduands.AddRange(graduands);
                                    _context.SaveChanges();

                                    // add GraduandAward records
                                 /*   var graduandAwards = new List<GraduandAward>();
                                    for (int r = 2; r <= noOfRow; r++)
                                    {
                                        var graduandAward = ExtractGraduandAward(worksheet, r);
                                        if (graduandAward != null)
                                        {
                                            graduandAwards.Add(graduandAward);
                                        }
                                    }*/

                                  //  _context.GraduandAwards.AddRange(graduandAwards);
                                  //  _context.SaveChanges(); // Save GraduandAward records
                                }

                                else
                                {
                                    ViewBag.Errors = errors;
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "This Excel is empty.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("ImportExcel");
        }



        //retrieving the actual values by row
        private string GetValue(ExcelWorksheet worksheet, int row, int columnIndex)
        {
            return worksheet.Cells[row, columnIndex].Text;
        }
        //-----------------------Graduand
        private Graduand ExtractGraduand(ExcelWorksheet worksheet, int row)
        {
            //creating new Graduand object to store each one
            Graduand graduand = new Graduand
            {
                //numbers in last are the column numbers for each
                PersonCode = int.Parse(GetValue(worksheet, row, 1)),
                Forenames = GetValue(worksheet, row, 2),
                Surname = GetValue(worksheet, row, 3),
                Nsn = int.Parse(GetValue(worksheet, row, 4)),
                Status = GetValue(worksheet, row, 16),
                BadDebtStatus = GetValue(worksheet, row, 17),
                DateOfBirth = DateTime.Parse(GetValue(worksheet, row, 18)),
                Ethnicity1 = GetValue(worksheet, row, 19),
                Ethnicity2 = GetValue(worksheet, row, 20),
                Ethnicity3 = GetValue(worksheet, row, 21),
                Iwi1 = GetValue(worksheet, row, 22),
                Iwi2 = GetValue(worksheet, row, 23),
                Iwi3 = GetValue(worksheet, row, 24),
                AddressLine1 = GetValue(worksheet, row, 25),
                AddressLine2 = GetValue(worksheet, row, 26),
                AddressLine3 = GetValue(worksheet, row, 27),
                AddressLine4 = GetValue(worksheet, row, 28),
                Town = GetValue(worksheet, row, 29),
                Postcode = GetValue(worksheet, row, 30),
                CollegeEmail = GetValue(worksheet, row, 31),
                PersonalEmail = GetValue(worksheet, row, 32),
                Mobile = GetValue(worksheet, row, 33),
                Telephone = GetValue(worksheet, row, 34),
                Campus = GetValue(worksheet, row, 35),
                School = GetValue(worksheet, row, 36),
                Comments = GetValue(worksheet, row, 39),
                DateRecordAddedToMasterList = GetValue(worksheet, row, 40),
            };
            return graduand;
        }

        //-----------------------Award
        // Extract an Award from a row in the worksheet
        //worksheet parameter to have the extracting work
        private Award ExtractAward(ExcelWorksheet worksheet, int row)
        {
            // Create a new Award object
            Award award = new Award
            {
                AwardCode = GetValue(worksheet, row, 5), 
                QualificationCode = GetValue(worksheet, row, 6),
                AwardDescription = GetValue(worksheet, row, 7),
                Level = GetValue(worksheet, row, 8),
                Credits = int.Parse(GetValue(worksheet, row, 11)),
                School = GetValue(worksheet, row, 36)
            };

            return award;
        }


        // Validation method for Award 
        private List<string> ValidateAward(Award award)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(award.AwardCode))
            {
                errors.Add("Award Code is missing a value.");
            }

            if (string.IsNullOrEmpty(award.QualificationCode))
            {
                errors.Add("Qualification Code is missing a value.");
            }

            if (string.IsNullOrEmpty(award.AwardDescription))
            {
                errors.Add("Award Description is missing a value.");
            }

            if (string.IsNullOrEmpty(award.Level))
            {
                errors.Add("Level is missing a value.");
            }

            if (string.IsNullOrEmpty(award.Credits.ToString()))
            {
                errors.Add("Credits is missing a value.");
            }

            return errors;
        }

        private GraduandAward ExtractGraduandAward(ExcelWorksheet worksheet, int row)
        {
            string awardCode = GetValue(worksheet, row, 5); 

            // check if an Award with the same AwardCode exists in the database
            var award = _context.Awards.FirstOrDefault(a => a.AwardCode == awardCode);

            // create a new GraduandAward object
            GraduandAward graduandAward = new GraduandAward
            {
                PersonCode = int.Parse(GetValue(worksheet, row, 1)),
                Major1 = GetValue(worksheet, row, 9),
                Major2 = GetValue(worksheet, row, 10),
                Completion = DateTime.Parse(GetValue(worksheet, row, 12)),
                Awarded = DateTime.Parse(GetValue(worksheet, row, 13)),
                YearAchieved = DateTime.Parse(GetValue(worksheet, row, 14)),
                IncludeInSdr = GetValue(worksheet, row, 15),
                AcademicDressRequirements1 = GetValue(worksheet, row, 37),
                AcademicDressRequirements2 = GetValue(worksheet, row, 38)
            };

            if (award != null)
            {
                // award found
                graduandAward.AwardId = award.AwardId;
            }

            return graduandAward;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}