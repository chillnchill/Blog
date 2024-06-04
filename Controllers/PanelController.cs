using Blog.Data.FileManager;
using Blog.Data.Services;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        
        private IRepository repository;
        private IFileManager fileManager;
        public PanelController(IRepository repository, IFileManager fileManager)
        {
            this.repository = repository;
            this.fileManager = fileManager;
        }

        public IActionResult Index()
        {
            List<Post> posts = repository.GetAllPosts();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new PostViewModel());
            }

            Post post = repository.GetPost(id);
            return View(new PostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                CurrentImage = post.Image,
                Description = post.Description,
                Category = post.Category,
                Tags = post.Tags
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {
            Post post = new Post()
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Body = viewModel.Body,
                Description = viewModel.Description,
                Category = viewModel.Category,
                Tags = viewModel.Tags
            };

			if (viewModel.Image == null)
			{
                post.Image = viewModel.CurrentImage;
			}
            else
            {
				post.Image = await fileManager.SaveImage(viewModel.Image);
			}
            
			if (ModelState.IsValid)
            {
                try
                {
                    Post existingPost = repository.GetPost(viewModel.Id.ToString());

                    if (existingPost == null)
                    {
                        repository.AddPost(post);
                    }
                    else
                    {
                        repository.UpdatePost(post);
                    }

                    if (await repository.SaveChangesAsync())
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(post);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflict
                    ModelState.AddModelError(string.Empty, "You bumbled");
                    return View(post);
                }
            }
            else
            {
                return View(post);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Remove(string id)
        {

            repository.RemovePost(id);
            await repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
