using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Blog;
using NinicoFinalTask.ViewModel.Product;

namespace NinicoFinalTask.Controllers
{
    public class BlogController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Blog()
        {
            var blogs = await _context.Blogs
                .Select(x => new BlogItemVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    Subtitle = x.SubTitle,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    CreatedTime = x.CreatedTime
                })
                .ToListAsync();

            return View(blogs);
        }
    
        public async Task<IActionResult> BlogDetails(int? id)
        {
            if (id == null)
                return NotFound();

            var data = await _context.Blogs
                .Include(x => x.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (data == null)
                return NotFound();

            var otherBlogs = await _context.Blogs
                .Where(x => x.Id != id)
                .Take(4)
                .Select(x => new BlogItemVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    Subtitle = x.SubTitle,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    CreatedTime = x.CreatedTime
                })
                .ToListAsync();

            var blogImages = await _context.BlogImages
                .Where(x => x.BlogId == id)
                .Select(x => x.ImageUrl)
                .ToListAsync();

            var model = new BlogDetailVm
            {
                Id = data.Id,
                Title = data.Title,
                Subtitle = data.SubTitle,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                CreatedTime = data.CreatedTime,
                OtherImagesUrl = blogImages,
                OtherBlogs = otherBlogs
            };

            return View(model);
        }

    }
}
