using GraduationCeremony.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using OfficeOpenXml;
using GraduationCeremony.Models.DB;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Globalization;

namespace GraduationCeremony.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly S232_Project01_TestContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(S232_Project01_TestContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportExcel()
        {
            return View();
        }


        //based from: https://medium.com/c-sharp-progarmming/convert-excel-to-data-table-in-asp-net-core-using-ep-plus-b59533e162b3
        [HttpPost]
        public async Task<IActionResult> UploadAsync(FileUploadModel model)
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
                                DeleteEmptyRows(worksheet);
                                //getting existing data from db
                                var awardsFull = from g in _context.Awards select g;
                                var graduandsFull = from g in _context.Graduands select g;
                                var gradAwardsFull = from g in _context.GraduandAwards select g;

                                //converting db to list
                                List<Award> awardsFullList = await awardsFull.ToListAsync();
                                List<Graduand> graduandsFullList = await graduandsFull.ToListAsync();
                                List<GraduandAward> graduandAwardsFullList = await gradAwardsFull.ToListAsync();

                                //new list to save the excel data
                                List<Award> awards = new List<Award>();
                                List<Graduand> graduands = new List<Graduand>();
                                List<GraduandAward> graduandAwards = new List<GraduandAward>();

                                var errors = new HashSet<string>();

                                int noOfRow = worksheet.Dimension.End.Row;

                                var processedAwardCodes = new HashSet<string>();
                                var processedGraduandCodes = new HashSet<string>();
                                var processedGraduandAwardCodes = new HashSet<string>();

                                //starts at 2 because row 1 is headings
                                for (int r = 2; r <= noOfRow; r++)
                                {
                                    Award award = ExtractAward(worksheet, r);
                                    Graduand graduand = ExtractGraduand(worksheet, r);
                                    GraduandAward graduandAward = ExtractGraduandAward(worksheet, r);

                                    //validating award
                                    if (award != null)
                                    {
                                        if (processedAwardCodes.Add(award.AwardCode))
                                        {
                                            var awardErrors = ValidateAward(award);

                                            if (awardErrors.Count == 0)
                                            {
                                                //checking if data exists already
                                                if (awardsFullList.Find(x => x.AwardCode == award.AwardCode) == null)
                                                {
                                                    awards.Add(award);
                                                }

                                            }
                                            else
                                            {
                                                //https://stackoverflow.com/questions/47752/remove-duplicates-from-a-listt-in-c-sharp
                                                errors.UnionWith(awardErrors);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errors.Add("Award not found for row " + r);
                                    }

                                    //validating graduand
                                    if (graduand != null)
                                    {
                                        //checking if data exists already
                                        if (processedGraduandCodes.Add(graduand.PersonCode.ToString()))
                                        {
                                            if (graduandsFullList.Find(x => x.PersonCode == graduand.PersonCode) == null)
                                            {
                                                graduands.Add(graduand);
                                            }
                                        }
                                    }

                                    //validating graduand award
                                    if (graduandAward != null)
                                    {
                                        //checking if data exists already
                                        if (processedGraduandAwardCodes.Add(graduandAward.PersonCode.ToString()))
                                        {
                                            var gradAwardErrors = ValidateGraduandAward(graduandAward);
                                            if (gradAwardErrors.Count == 0)
                                            {
                                                //ensuring that the data gets added if it doesn't exist in bridging table
                                                if (graduandAwardsFullList.Find(x => x.AwardCode == graduandAward.AwardCode && x.PersonCode == graduandAward.PersonCode) == null)
                                                {
                                                    graduandAwards.Add(graduandAward);
                                                }
                                            }
                                            else
                                            {
                                                errors.UnionWith(gradAwardErrors);
                                            }
                                        }

                                    }
                                    ViewBag.Errors = errors;
                                }


                                //only saving those with changes
                                if (awards.Count != 0 && awards.Count != awardsFullList.Count())
                                {
                                    //for add range: https://stackoverflow.com/questions/38887434/cannot-convert-from-string-to-system-collections-generic-list-string
                                    //adding to DB
                                    _context.Awards.AddRange(awards);
                                    _context.SaveChanges();
                                }
                                else
                                    ViewBag.ErrorMessage = "No New Awards Added" + "\n";


                                //only saving those with changes
                                if (graduands.Count != 0 && graduands.Count != graduandsFullList.Count())
                                {
                                    _context.Graduands.AddRange(graduands);
                                    _context.SaveChanges();
                                }
                                else
                                    ViewBag.ErrorMessage = "No New Graduands Added" + "\n";

                                //only saving those with changes

                                if (graduandAwards.Count != 0 && graduandAwards.Count != graduandAwardsFullList.Count())
                                {
                                    //ordering the items
                                    graduandAwards = graduandAwards
                                     .OrderBy(item => item.AwardCodeNavigation?.Level)
                                     .ThenBy(item => item.AwardCodeNavigation?.AwardDescription)
                                     .ThenBy(item => item.PersonCodeNavigation?.Forenames)
                                     .ToList();

                                    _context.GraduandAwards.AddRange(graduandAwards);
                                    _context.SaveChanges();
                                    ViewBag.SuccessMessage = "Excel successfully uploaded";
                                }
                                else
                                    ViewBag.ErrorMessage = "No New Grad Awards Added" + "\n";
                            }

                            else
                            {
                                ViewBag.ErrorMessage = "This Excel is empty.";
                            }
                        }
                    }
                }
                else
                    ViewBag.ErrorMessage = "No File Was Selected";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = "Incorrect file. Please try again";
            }

            return View("ImportExcel");
        }

        //https://stackoverflow.com/questions/73690637/how-to-find-empty-rows-from-excel-sheet-given-by-user-and-delete-them-in-asp-net
        private void DeleteEmptyRows(ExcelWorksheet worksheet)
        {
            var rowCount = worksheet.Dimension.Rows;
            var maxColumns = worksheet.Dimension.Columns;

            for (int row = rowCount; row > 0; row--)
            {
                bool isRowEmpty = true;
                for (int column = 1; column <= maxColumns; column++)
                {
                    var cellEntry = Convert.ToString(worksheet.Cells[row, column].Value);
                    if (!string.IsNullOrEmpty(cellEntry))
                    {
                        isRowEmpty = false;
                        break;
                    }
                }

                if (!isRowEmpty)
                    continue;
                else
                    worksheet.DeleteRow(row);
            }
        }

        //for awarded column
        private DateTime? ParseCustomDateFormat(object value)
        {
            //convert value to string
            var convertedvalue = value.ToString();
            //check if it is not empty
            if (!string.IsNullOrWhiteSpace(convertedvalue))
            {
                //https://stackoverflow.com/questions/4718960/datetime-tryparse-issue-with-dates-of-yyyy-dd-mm-format
                //convert string date to datetime
                if (DateTime.TryParse(convertedvalue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine($"Failed to convert to datetime: {convertedvalue}");
            }
            return null;
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
                Campus = GetValue(worksheet, row, 35),
                School = GetValue(worksheet, row, 36),
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
                AwardCode = GetValue(worksheet, row, 5),
                PersonCode = int.Parse(GetValue(worksheet, row, 1)),
                Major1 = GetValue(worksheet, row, 9),
                Major2 = GetValue(worksheet, row, 10),
                Completion = DateTime.Parse(GetValue(worksheet, row, 12)),
                Awarded = ParseCustomDateFormat(GetValue(worksheet, row, 13)),
                YearAchieved = DateTime.Parse(GetValue(worksheet, row, 14)),
            };

            if (award != null)
            {
                // award found
                graduandAward.AwardCode = award.AwardCode;
            }

            return graduandAward;
        }

        // Validation method for Award 
        private List<string> ValidateGraduandAward(GraduandAward graduandAward)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(graduandAward.Completion.ToString()))
            {
                errors.Add("Completion date is missing a value.");
            }

            if (string.IsNullOrEmpty(graduandAward.YearAchieved.ToString()))
            {
                errors.Add("Year achieved date is missing a value.");
            }
            return errors;
        }


        public async Task<ViewResult> DeleteAsync(string text)
        {
            if (text == "Checked-in List")
            {
                var checkIn = from g in _context.CheckIns select g;

                _context.CheckIns.RemoveRange(checkIn);
                _context.SaveChanges();

                ViewBag.ErrorMessage = "Checked in List Deleted";
                return View("ImportExcel");
            }
            else if (text == "Graduand")
            {
                var awardsFull = from g in _context.Awards select g;
                var graduandsFull = from g in _context.Graduands select g;
                var gradAwardsFull = from g in _context.GraduandAwards select g;

                _context.RemoveRange(awardsFull);
                _context.RemoveRange(graduandsFull);
                _context.RemoveRange(gradAwardsFull);

                _context.SaveChanges();

                // reset identity numbering for GraduandAwards
                ResetIdForGraduandAwards();

                ViewBag.ErrorMessage = "Graduands Deleted";
                return View("ImportExcel");
            }
            else
            {
                //getting existing data from db
                var awardsFull = from g in _context.Awards select g;
                var graduandsFull = from g in _context.Graduands select g;
                var gradAwardsFull = from g in _context.GraduandAwards select g;
                var checkIn = from g in _context.CheckIns select g;

                _context.RemoveRange(awardsFull);
                _context.RemoveRange(graduandsFull);
                _context.RemoveRange(gradAwardsFull);
                _context.RemoveRange(checkIn);

                _context.SaveChanges();

                // reset identity numbering for GraduandAwards
                ResetIdForGraduandAwards();

                ViewBag.ErrorMessage = "Database is empty";
                return View("ImportExcel");
            }
        }

        // based off: https://stackoverflow.com/questions/52937700/reset-id-column-of-database-records
        public void ResetIdForGraduandAwards()
        {
            try
            {
                using (S232_Project01_TestContext context = new S232_Project01_TestContext())
                {
                    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Graduand_Award', RESEED, 0)");
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Error: Records could not be reset - " + exp.Message.ToString(), exp);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}