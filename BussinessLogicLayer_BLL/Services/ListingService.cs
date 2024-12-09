﻿using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer_BLL.Services
{
	public class ListingService : IListingRepository
	{
		private readonly AppDbContext _context;
		public ListingService(AppDbContext context) {
			_context = context;
		}
		public async Task AddListingAsync(Listing listing)
		{
			_context.Listings.Add(listing);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteListingAsync(int listingId)
		{
			var listing = _context.Listings.FirstOrDefault(x => x.ListingId == listingId);
			if (listing != null) {
				var imageOld = _context.Images.Where(x => x.ListingId == listingId).ToList();
				foreach (var i in imageOld)
				{
					_context.Images.Remove(i);
				}
				_context.Listings.Remove(listing);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Listing>> GetAllListingAsync()
		{
			
			return await _context.Listings.Include(x => x.Category).Include(x => x.User).Include(x => x.City).ToListAsync(); 
		}

		public async Task<Listing?> GetListingByIdAsync(int listingId)
		{
			var listing = await _context.Listings.FirstOrDefaultAsync(x => x.ListingId ==listingId);
			if (listing != null)
			{
				return listing;
			}
			throw new NotImplementedException();
		}

		public async Task UpdateListingAsync(Listing listing)
		{
			_context.Listings.Update(listing);
			await _context.SaveChangesAsync();
		}
	}
}
