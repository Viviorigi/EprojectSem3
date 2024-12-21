using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public byte Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
