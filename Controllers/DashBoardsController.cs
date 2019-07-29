using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsBlog.Data;
using NewsBlog.Services;
using NewsData.Models;

namespace NewsBlog.Controllers
{
    public class DashBoardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public DashBoardsController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task<IActionResult> IndexArticle(int? page)
        {

            var applicationDbContext = _context.Articles
                                            .Include(p => p.Category)
                                            .OrderByDescending(p => p.ID);

            var model = await PaginatedList<Article>
                                    .CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, 20);

            return View(model);

        }

        // GET: Articles/ AddArticle
        public IActionResult AddArticle()
        {
            PopulateCategorysDropDownList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddArticle([Bind("ID,CategoryID,Description,Tittle,ProfilePictureFile")] Article article, IFormFile ProfilePictureFile)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePictureFile != null)
                {
                    var fnm = article.Tittle;
                    var filename = fnm + DateTime.Now.ToString("MMddHHmmss") + ".jpg";

                    string uploadPath = Path.Combine(_environment.WebRootPath, "images/Article");

                    if (filename.Contains('\\'))
                    {
                        filename = filename.Split('\\').Last();
                    }

                    using (FileStream fs = new FileStream(Path.Combine(uploadPath, filename), FileMode.Append))
                    {
                        await ProfilePictureFile.CopyToAsync(fs);
                    }

                    article.ImageName = filename;
                }
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexArticle");
            }
            PopulateCategorysDropDownList();
            return View(article);
        }

        // GET: Articles/EditArticle/5
        public async Task<IActionResult> EditArticle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.SingleOrDefaultAsync(m => m.ID == id);
            if (article == null)
            {
                return NotFound();
            }
            PopulateCategorysDropDownList();
            return View(article);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArticle(int id, [Bind("ID,CategoryID,Description,Tittle,ProfilePictureFile")] Article article, IFormFile ProfilePictureFile)
        {
            if (id != article.ID)
            {
                return RedirectToAction("IndexArticle");
                //return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ProfilePictureFile != null)
                    {
                        var fnm = article.Tittle;
                        var filename = fnm + DateTime.Now.ToString("MMddHHmmss") + ".jpg";

                        string uploadPath = Path.Combine(_environment.WebRootPath, "images/Article");

                        if (filename.Contains('\\'))
                        {
                            filename = filename.Split('\\').Last();
                        }

                        using (FileStream fs = new FileStream(Path.Combine(uploadPath, filename), FileMode.Append))
                        {
                            await ProfilePictureFile.CopyToAsync(fs);
                        }

                        article.ImageName = filename;
                    }

                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ID))
                    {
                        return RedirectToAction("IndexArticle");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditArticle");
            }
            PopulateCategorysDropDownList();
            return View(article);
        }

        [HttpPost, ActionName("DeleteArticle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var Messages = await _context.Articles
                                    .Include(c => c.Category)
                                    .Include(p => p.Comments)
                                    .SingleOrDefaultAsync(m => m.ID == id);
            _context.Articles.Remove(Messages);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexArticle");
        }


        private void PopulateCategorysDropDownList(object selectedCategory = null)
        {
            var categorysQuery = from c in _context.Categories
                                 orderby c.Name
                                 select c;
            ViewBag.CategoryID = new SelectList(categorysQuery.AsNoTracking(), "CategoryID", "Name",
                selectedCategory);
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ID == id);
        }




        public async Task<IActionResult> IndexCategory(int? page)
        {

            var applicationDbContext = _context.Categories
                                            .OrderByDescending(p => p.CategoryID);

            var model = await PaginatedList<Category>
                                    .CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, 20);

            return View(model);

        }

        // GET: Categorys/AddCategory
        public IActionResult AddCategory()
        {
            return View();
        }

        // POST: Categorys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory([Bind("CategoryID,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("AddCategory");
            }
            return View(category);
        }

        // GET: Categorys/EditCategory/5
        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categorys/EditCategory/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, [Bind("CategoryID,Name")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexCategory");
            }
            return View(category);
        }


      
        
        public IActionResult DeleteCategory(int id)
        {
            var Messages = _context.Categories
                                .FirstOrDefault(m => m.CategoryID == id);
                 _context.Categories.Remove(Messages);

             _context.SaveChangesAsync();
            return RedirectToAction("IndexCategory");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }

    }
}