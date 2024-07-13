﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClinicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClinicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clinics
        [Authorize(Roles = "ViewClinic")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clinic.ToListAsync());
        }

        // GET: Clinics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinic
                .FirstOrDefaultAsync(m => m.ClinicID == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        // GET: Clinics/Create
        [Authorize(Roles = "CreateClinic")]
        public IActionResult Create()
        {
            return View();
        }
        private Clinic ExtractFormValuesIntoModel(IFormCollection form)
        {


            var clinics = new Clinic()
            {
                ClinicID = int.Parse(form["ClinicID"]),
                ClinicName = form["ClinicName"],
                ClinicLocation = form["ClinicLocation"],
                IsActive = bool.Parse(form["IsActive"])

            };


            return clinics;
        }
        // POST: Clinics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClinicID,ClinicName,ClinicLocation,IsActive")] Clinic clinic)
        {

            //var clinics = ExtractFormValuesIntoModel(form);
            if (ModelState.IsValid)
            {
                _context.Add(clinic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clinic);
        }

        // GET: Clinics/Edit/5
        [Authorize(Roles = "CreateClinic")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinic.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            return View(clinic);
        }

        // POST: Clinics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClinicID,ClinicName,ClinicLocation,IsActive")] Clinic clinic)
        {
            if (id != clinic.ClinicID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clinic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClinicExists(clinic.ClinicID))
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
            return View(clinic);
        }

        // GET: Clinics/Delete/5
        [Authorize(Roles = "DeleteClinic")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinic = await _context.Clinic
                .FirstOrDefaultAsync(m => m.ClinicID == id);
            if (clinic == null)
            {
                return NotFound();
            }

            return View(clinic);
        }

        // POST: Clinics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clinic = await _context.Clinic.FindAsync(id);
            if (clinic != null)
            {
                _context.Clinic.Remove(clinic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinic.Any(e => e.ClinicID == id);
        }
    }
}
