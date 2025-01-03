﻿using System;
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
    public class CitiesController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly ICityRepository _cityRepository;

        public CitiesController(AppDbContext context, ICityRepository cityRepository)
        {
            _context = context;
            _cityRepository = cityRepository;
        }

        // GET: Admin/Cities
        public async Task<IActionResult> Index()
        {
            ViewBag.region = new SelectList(await _context.Regions.ToListAsync(), "RegionId", "Name");
            var city = await _cityRepository.GetAllCitysAsync();
            return View(city);
        }

        // GET: Admin/Cities/Create
        public IActionResult Create()
        {
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "Name");
            return View();
        }

        // POST: Admin/Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityId,Name,RegionId")] City city)
        {
            if (ModelState.IsValid)
            {
                await _cityRepository.AddCityAsync(city);
                TempData["msg"] = "Create successful";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId", city.RegionId);
            return View(city);
        }

        // GET: Admin/Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _cityRepository.GetCityByIdAsync(id);
            if (city == null)
            {
                return NotFound();      
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "Name", city.RegionId);
            return View(city);
        }

        // POST: Admin/Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CityId,Name,RegionId")] City city)
        {
            if (id != city.CityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _cityRepository.UpdateCityAsync(city);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.CityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["msg"] = "Update City successful";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionId", city.RegionId);
            return View(city);
        }

        public async Task<IActionResult> Search(string? name, int? id)
        {
            ViewBag.region = new SelectList(await _context.Regions.ToListAsync(), "RegionId", "Name");
            ViewBag.name = name;
            ViewBag.id = id;
            var x = await _cityRepository.Search(name, id);
            if (x == null)
            {
                TempData["msg"] = "Search results do not exist";
                TempData["AlertType"] = "error";
                return RedirectToAction("Index");
            }
            return View(x);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = _context.Listings.FirstOrDefault(x => x.CityId == id);
            if (listing == null)
            {
                var city = _context.Cities.FirstOrDefault(x => x.CityId == id);
                if (city != null)
                {
                    await _cityRepository.DeleteCityAsync(id);
                    TempData["msg"] = "Delete City successful";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
            }
            TempData["msg"] = "Existing posts cannot be deleted.";
            TempData["AlertType"] = "error"; // Types: success, error, warning, info
           
            return RedirectToAction("index");
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.CityId == id);
        }
    }
}
