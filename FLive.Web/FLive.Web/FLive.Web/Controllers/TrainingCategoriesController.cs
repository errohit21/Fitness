using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Data;
using FLive.Web.Models;
using FLive.Web.Repositories;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Controllers
{
    public class TrainingCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadRepository _fileUploadRepository;

        public TrainingCategoriesController(ApplicationDbContext context, IFileUploadRepository fileUploadRepository)
        {
            _context = context;
            _fileUploadRepository = fileUploadRepository;
        }

        // GET: TrainingCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrainingCategories.ToListAsync());
        }

        // GET: TrainingCategories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingCategory = await _context.TrainingCategories.SingleOrDefaultAsync(m => m.Id == id);
            if (trainingCategory == null)
            {
                return NotFound();
            }

            return View(trainingCategory);
        }

        // GET: TrainingCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Description,ImageUrl,Name,TextColor")] TrainingCategory trainingCategory, IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(trainingCategory);
        }

        // GET: TrainingCategories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingCategory = await _context.TrainingCategories.SingleOrDefaultAsync(m => m.Id == id);
            if (trainingCategory == null)
            {
                return NotFound();
            }
            return View(trainingCategory);
        }

        // POST: TrainingCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,
            [Bind("Id,Description,DescriptionColor,ImageUrl,Name,NameColor,")] TrainingCategory trainingCategory,
            IEnumerable<IFormFile> files)
        {
            if (id != trainingCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var file = files.FirstOrDefault();
                    var imageUrl = trainingCategory.ImageUrl;
                    if (file.IsNotNull())
                    {
                        var stream = file.OpenReadStream();
                        imageUrl =  await _fileUploadRepository.UploadFileAsBlob(stream, file.FileName, "trainingcategories");
                    }

                    trainingCategory.ImageUrl = imageUrl;
                    _context.Update(trainingCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingCategoryExists(trainingCategory.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(trainingCategory);
        }

        // GET: TrainingCategories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingCategory = await _context.TrainingCategories.SingleOrDefaultAsync(m => m.Id == id);
            if (trainingCategory == null)
            {
                return NotFound();
            }

            return View(trainingCategory);
        }

        // POST: TrainingCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var trainingCategory = await _context.TrainingCategories.SingleOrDefaultAsync(m => m.Id == id);
            _context.TrainingCategories.Remove(trainingCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TrainingCategoryExists(long id)
        {
            return _context.TrainingCategories.Any(e => e.Id == id);
        }
    }
}