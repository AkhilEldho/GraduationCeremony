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

        [HttpGet]
        //NOT WORKING :((
        public ActionResult Edit(GraduandDetails graduandDetails)
        {
            if (ModelState.IsValid)
            {
                var graduand = _context.Graduands.FirstOrDefault(g => g.PersonCode == graduandDetails.PersonCode);
                var award = _context.Awards.FirstOrDefault(a => a.AwardCode == graduandDetails.awards.AwardCode);
                var graduandAward = _context.GraduandAwards.FirstOrDefault(ga => ga.PersonCode == graduandDetails.PersonCode);
                if (graduand == null)
                {
                    return NotFound();
                }
                if (graduand != null)
                {
                    graduand.Forenames = graduandDetails.graduands.Forenames;
                }
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(graduandDetails);
        }
     
    }
}
