using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace EprojectSem3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegionsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IRegionRepository _regionRepository;

        public RegionsController(AppDbContext context, IRegionRepository regionRepository)
        {
            _context = context;
            _regionRepository = regionRepository;
        }

        // GET: Admin/Regions
        public async Task<IActionResult> Index()
        {
            var regions = await _regionRepository.GetAllRegionsAsync();
            return View(regions);
        }

        // GET: Admin/Regions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Regions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegionId,Name")] Region region)
        {
            if (ModelState.IsValid)
            {               
                await _regionRepository.AddRegionAsync(region);
                TempData["msg"] = "Create successful";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }
           
            return View(region);
        }

        // GET: Admin/Regions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _regionRepository.GetRegionByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Admin/Regions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegionId,Name")] Region region)
        {
            if (id != region.RegionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _regionRepository.UpdateRegionAsync(region);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegionExists(region.RegionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["msg"] = "Update Region successful";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            return View(region);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cities = _context.Cities.FirstOrDefault(x => x.RegionId == id);
            if (cities != null)
            {
                var regions = _context.Regions.FirstOrDefault(x => x.RegionId == id);
                if (regions != null)
                {
                    await _regionRepository.DeleteRegionAsync(id);
                    ViewBag.message = "Delete region successful";
                    TempData["msg"] = "Delete region successful";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.message = "Existing posts cannot be deleted.";
            return View("index");
        }

        private bool RegionExists(int id)
        {
            return _context.Regions.Any(e => e.RegionId == id);
        }
    }
}
