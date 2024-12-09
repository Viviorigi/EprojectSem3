﻿using DataAccessLayer_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Repositories
{
	public interface IListingRepository
	{
		Task<IEnumerable<Listing>> GetAllListingAsync();
		Task<Listing?> GetListingByIdAsync(int listingId);
		Task AddListingAsync(Listing listing);
		Task UpdateListingAsync(Listing listing);
		Task DeleteListingAsync(int listingId);
	}
}
