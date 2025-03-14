using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.DataAcces;
using NinicoFinalTask.Extensions;
using NinicoFinalTask.Helpers;
using NinicoFinalTask.Models;
using NinicoFinalTask.ViewModel.Blog;
using NinicoFinalTask.ViewModel.Common;

namespace NinicoFinalTask.Areas.Admin.Controllers
{
    
        [Area("Admin")]
    [Authorize(Roles = RoleConstant.Blog)]
    public class BlogController(NinicoDbContext _context, IWebHostEnvironment _env) : Controller
        {
            public async Task<IActionResult> Index()
            {
            var datas = await _context.Blogs.Where(x => x.isDeleted == false).ToListAsync();

                return View(datas);
            }
            public async Task<IActionResult> Create()
            {

                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Create(BlogCreateVM vm)
            {
                if (vm.OtherImages != null && vm.OtherImages.Any())
                {
                    if (!vm.OtherImages.All(x => x.IsValidType("image")))
                    {
                        var fileNames = vm.OtherImages.Where(x => !x.IsValidType("image")).Select(x => x.FileName);
                        ModelState.AddModelError("OtherImages", string.Join(",", fileNames) + " an(is) not image");
                    }
                    if (!vm.OtherImages.All(x => x.IsValidSize(3 * 1024)))
                    {
                        var fileNames = vm.OtherImages.Where(x => !x.IsValidSize(3 * 1024)).Select(x => x.FileName);
                        ModelState.AddModelError("OtherImages", string.Join(",", fileNames) + "must be less than 2 mb");

                    }
                }
                if (vm.CoverFile != null)
                {
                    if (!vm.CoverFile.IsValidType("image"))
                        ModelState.AddModelError("CoverFile", "must be an image");
                    if (!vm.CoverFile.IsValidSize(2 * 1024))
                        ModelState.AddModelError("CoverFile", "must be less than 2 mb");
                }
                if (!ModelState.IsValid) return View();
                Blog blog = new Blog
                {
                    Title = vm.Title,
                    SubTitle = vm.SubTitle,
                    Description = vm.Description,
                    ImageUrl = await vm.CoverFile!.UploadAsync(_env.WebRootPath, "imgs", "Blogs"),
                    Images = vm.OtherImages!.Select(x => new BlogImage
                    {
                        ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "Blogs").Result
                    }).ToList()
                };
                await _context.Blogs.AddAsync(blog);
                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            [HttpGet]
            public async Task<IActionResult> Update(int? id)
            {
                if (!id.HasValue) return NotFound();
                var data = await _context.Blogs.Where(x => x.isDeleted == false).Select(x => new BlogUpdateVM
                {
                    Title = x.Title,
                    SubTitle = x.SubTitle,
                    Description = x.Description,
                    CoverFileUrl = x.ImageUrl,
                    OtherImagesUrl = x.Images!.Select(z => new ImageUrlAndId
                    {
                        Url = z.ImageUrl,
                        Id = z.Id
                    })
                }).FirstOrDefaultAsync();
                return View(data);
            }
            [HttpPost]
            public async Task<IActionResult> Update(int? id, BlogUpdateVM vm)
            {
                var blog = await _context.Blogs
                  .Include(p => p.Images)
                  .FirstOrDefaultAsync(p => p.Id == id);

                if (blog == null) return NotFound();

                if (vm.OtherImages != null && vm.OtherImages.Any())
                {
                    var invalidFiles = vm.OtherImages.Where(x => !x.IsValidType("image")).Select(x => x.FileName).ToList();
                    if (invalidFiles.Any())
                        ModelState.AddModelError("OtherImages", string.Join(", ", invalidFiles) + " are not images");

                    var largeFiles = vm.OtherImages.Where(x => !x.IsValidSize(2 * 1024)).Select(x => x.FileName).ToList();
                    if (largeFiles.Any())
                        ModelState.AddModelError("OtherImages", string.Join(", ", largeFiles) + " must be less than 2 MB");
                }

                if (vm.CoverFile != null)
                {
                    if (!vm.CoverFile.IsValidType("image"))
                        ModelState.AddModelError("CoverFile", "must be an image");

                    if (!vm.CoverFile.IsValidSize(2 * 1024))
                        ModelState.AddModelError("CoverFile", "must be less than 2 MB");
                }




                if (vm.CoverFile != null)
                {
                    if (!string.IsNullOrEmpty(blog.ImageUrl))
                    {
                        var oldCoverPath = Path.Combine(_env.WebRootPath, "imgs", "Blogs", blog.ImageUrl);
                        if (System.IO.File.Exists(oldCoverPath))
                            System.IO.File.Delete(oldCoverPath);
                    }

                    blog.ImageUrl = await vm.CoverFile.UploadAsync(_env.WebRootPath, "imgs", "Blogs");
                }

                if (vm.OtherImagesUrl != null)
                {
                    var deletedImageIds = blog.Images!
                        .Where(img => !vm.OtherImagesUrl.Any(x => x.Id == img.Id))
                        .Select(img => img.Id)
                        .ToList();

                    foreach (var img in blog.Images!.Where(x => deletedImageIds.Contains(x.Id)).ToList())
                    {
                        var imgPath = Path.Combine(_env.WebRootPath, "imgs", "Blogs", img.ImageUrl);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);

                        _context.BlogImages.Remove(img);
                    }
                }

                if (vm.OtherImages != null && vm.OtherImages.Any())
                {
                    var newImages = await Task.WhenAll(vm.OtherImages.Select(async x => new BlogImage
                    {
                        ImageUrl = await x.UploadAsync(_env.WebRootPath, "imgs", "Blogs")
                    }));

                    blog.Images = blog.Images!.ToList();
                    blog.Images.AddRange(newImages);
                }

                blog.Title = vm.Title;
                blog.Description = vm.Description;
                blog.SubTitle = vm.SubTitle;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            public async Task<IActionResult> Delete(int? id)
            {
                if (!id.HasValue) return View();
                var data = await _context.Blogs.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
                if (data is null) return BadRequest();
                _context.Blogs.Remove(data);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }



            public async Task<IActionResult> DeleteImage(int? id)
            {
                if (!id.HasValue) return BadRequest();
                var img = await _context.BlogImages.FindAsync(id.Value);
                if (img == null) return NotFound();
                _context.BlogImages.Remove(img);
                await _context.SaveChangesAsync();
                string path = Path.Combine(_env.WebRootPath, "imgs", "Blogs", img.ImageUrl);

                if (Path.Exists(path))
                    System.IO.File.Delete(path);

                return View();
            }

        }
    }
