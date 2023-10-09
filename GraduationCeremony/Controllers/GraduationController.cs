using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraduationCeremony.Models.DB;
using Microsoft.AspNetCore.Authorization;

namespace GraduationCeremony.Controllers
{
    public class GraduationController : Controller
    {
        private readonly GraduationContext _context;

        public GraduationController(GraduationContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Graduation
        public async Task<IActionResult> Index()
        {
              return _context.Graduations != null ? 
                          View(await _context.Graduations.ToListAsync()) :
                          Problem("Entity set 'S232_Project01_TestContext.Graduations'  is null.");
        }

        // GET: Graduation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Graduations == null)
            {
                return NotFound();
            }

            var graduation = await _context.Graduations
                .FirstOrDefaultAsync(m => m.PersonCode == id);
            if (graduation == null)
            {
                return NotFound();
            }

            return View(graduation);
        }

        // GET: Graduation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Graduation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonCode,Forenames,Surname,Nsn,AwardCode,QualificationCode,AwardDescription,Level,Major1,Major2,Credits,Completion,Awarded,YearArchieved,IncludeInSdr,Status,BadDebtStatus,DateOfBirth,Ethnicity1,Ethnicity2,Ethnicity3,Iwi1,Iwi2,Iwi3,AddressLine1,AddressLine2,AddressLine3,AddressLine4,Town,PostCode,CollegeEmail,PersonalEmail,Mobile,Telephone,Campus,School,AcademicDressRequirements1,AcademicDressRequirements2,Comments,DateRecordAddedToMasterList")] Graduation graduation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(graduation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(graduation);
        }

        // GET: Graduation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Graduations == null)
            {
                return NotFound();
            }

            var graduation = await _context.Graduations.FindAsync(id);
            if (graduation == null)
            {
                return NotFound();
            }
            return View(graduation);
        }

        // POST: Graduation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonCode,Forenames,Surname,Nsn,AwardCode,QualificationCode,AwardDescription,Level,Major1,Major2,Credits,Completion,Awarded,YearArchieved,IncludeInSdr,Status,BadDebtStatus,DateOfBirth,Ethnicity1,Ethnicity2,Ethnicity3,Iwi1,Iwi2,Iwi3,AddressLine1,AddressLine2,AddressLine3,AddressLine4,Town,PostCode,CollegeEmail,PersonalEmail,Mobile,Telephone,Campus,School,AcademicDressRequirements1,AcademicDressRequirements2,Comments,DateRecordAddedToMasterList")] Graduation graduation)
        {
            if (id != graduation.PersonCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(graduation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GraduationExists(graduation.PersonCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(graduation);
        }

        // GET: Graduation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Graduations == null)
            {
                return NotFound();
            }

            var graduation = await _context.Graduations
                .FirstOrDefaultAsync(m => m.PersonCode == id);
            if (graduation == null)
            {
                return NotFound();
            }

            return View(graduation);
        }

        // POST: Graduation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Graduations == null)
            {
                return Problem("Entity set 'S232_Project01_TestContext.Graduations'  is null.");
            }
            var graduation = await _context.Graduations.FindAsync(id);
            if (graduation != null)
            {
                _context.Graduations.Remove(graduation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GraduationExists(int id)
        {
          return (_context.Graduations?.Any(e => e.PersonCode == id)).GetValueOrDefault();
        }
    }
}
