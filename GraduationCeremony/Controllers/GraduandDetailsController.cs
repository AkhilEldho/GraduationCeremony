using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Xml.Linq;

namespace GraduationCeremony.Controllers
{
    public class GraduandDetailsController : Controller
    {
        private readonly S232_Project01_TestContext _context;

        public GraduandDetailsController(S232_Project01_TestContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
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

            return View(result);
        }
        //COULDNT MAKE THIS WORK WITH THE NAVIGATION FOR HTTPOST SO COMMENTED AND EDITED :(
        /* [HttpGet]
         public ActionResult Edit(int personCode)
         {
             int code = personCode;

             var result = (from g in _context.Graduands
                           join ga in _context.GraduandAwards on g.PersonCode equals ga.PersonCode
                           join a in _context.Awards on ga.AwardCode equals a.AwardCode
                           where g.PersonCode == personCode
                           select new GraduandDetails
                           {
                               graduands = g,
                               awards = a,
                               graduandAwards = ga
                           }).FirstOrDefault();

             GraduandDetails graduandDetails = result;

             if (ModelState.IsValid)
             {
                 var graduand = _context.Graduands.FirstOrDefault(g => g.PersonCode == graduandDetails.graduands.PersonCode);
                 var award = _context.Awards.FirstOrDefault(a => a.AwardCode == graduandDetails.awards.AwardCode);
                 var graduandAward = _context.GraduandAwards.FirstOrDefault(ga => ga.PersonCode == graduandDetails.graduands.PersonCode);

                 if (graduand == null)
                 {
                     return NotFound();
                 }
                 if (graduand != null)
                 {
                     graduand.Forenames = graduandDetails.graduands.Forenames;
                 }

                 _context.SaveChanges();

                 return View(graduandDetails);
             }
             return View(graduandDetails);
         }*/

        [HttpGet]
        public IActionResult Edit(int? personCode)
        {
            if (personCode == null)
            {
                return NotFound();
            }

            //creating new GraduandDetails ViewModel
            var graduandDetailsVM = new GraduandDetails();

            var gradAwards = _context.GraduandAwards
                .Include(g => g.PersonCodeNavigation) //accessing graduands table
                .Include(a => a.AwardCodeNavigation) //accessing awards table
                .SingleOrDefault(ga => ga.PersonCode == personCode);

            if (gradAwards == null)
            {
                //NEED TO HAVE BETTER VALIDATION
                return NotFound();
            }

            graduandDetailsVM.graduandAwards = gradAwards;

            graduandDetailsVM.graduands = gradAwards.PersonCodeNavigation; //populating graduand attributes

            graduandDetailsVM.awards = gradAwards.AwardCodeNavigation; //populating award attributes

            return View(graduandDetailsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GraduandDetails graduandDetailsVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var grad = _context.Graduands.SingleOrDefault(g => g.PersonCode == graduandDetailsVM.graduandAwards.PersonCode);
                    var gradAwards = _context.GraduandAwards.SingleOrDefault(ga => ga.PersonCode == graduandDetailsVM.graduandAwards.PersonCode);
                    var award = _context.Awards.SingleOrDefault(a => a.AwardCode == graduandDetailsVM.awards.AwardCode);

                    if (grad != null && award != null && gradAwards != null)
                    {
                        //updating graduands
                        grad.Forenames = graduandDetailsVM.graduands.Forenames;
                        grad.Surname = graduandDetailsVM.graduands.Surname;
                        grad.DateOfBirth = graduandDetailsVM.graduands.DateOfBirth;
                        //updating awards
                        award.AwardDescription = graduandDetailsVM.awards.AwardDescription;
                        award.QualificationCode = graduandDetailsVM.awards.QualificationCode;
                        award.Level = graduandDetailsVM.awards.Level;
                        //updating graduandawards
                        gradAwards.Major1 = graduandDetailsVM.graduandAwards.Major1;
                        gradAwards.Major2 = graduandDetailsVM.graduandAwards.Major2;

                        // if user changed award code 
                        if (gradAwards.AwardCode != graduandDetailsVM.graduandAwards.AwardCode)
                        {
                            //to find if there is a matching award code in the database that the user entered
                            var newAward = _context.Awards.SingleOrDefault(a => a.AwardCode == graduandDetailsVM.graduandAwards.AwardCode);

                            if (newAward != null)
                            {
                                // update graduandAward award code with new awardcode
                                gradAwards.AwardCode = graduandDetailsVM.graduandAwards.AwardCode;

                                // update award attributes to get the new award descriptions etc.
                                gradAwards.AwardCodeNavigation = newAward;
                            }
                            else
                            {
                                //NEED VALIDATION LIKE VIEWBAG ETC TO VALIDATE AWARD CODE ETC
                                return View(graduandDetailsVM);
                            }
                        }
                    }
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }

                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            return View(graduandDetailsVM);
        }

    }
}
