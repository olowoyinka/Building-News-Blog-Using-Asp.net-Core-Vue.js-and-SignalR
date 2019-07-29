using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Data;
using NewsBlog.Services;
using NewsData.Models;

namespace NewsBlog.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Article Article { get; set; }

        public Comment Comment { get; set; }

        public PaginatedList<Category> CategoryPages { get; set; }

        public  async Task<IActionResult> OnGetAsync(string Tittle, int? page)
        {
            if (Tittle == null)
            {
                return NotFound();
            }

            Article = await _context.Articles
                            .Include(a => a.Category)
                            .Include(b => b.Comments)
                            .FirstOrDefaultAsync(m => m.Tittle == Tittle);
            
            int pageSize = 5;

            var applicationDbContext = _context.Categories.AsNoTracking()
                                    .OrderByDescending(i => i.CategoryID);
            
            CategoryPages =  await PaginatedList<Category>
                                    .CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, pageSize);
        
            return Page();
        }
        
    }
}