using Blog.Data;
using Blog.Data.Services;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace Blog.Controllers
{
	public class HomeController : Controller
	{
		private IRepository repository;
		public HomeController(IRepository repository)
		{
			this.repository = repository;
		}

		public IActionResult Index()
		{
			List<Post> posts = repository.GetAllPosts();
			return View(posts);
		}

		[HttpGet]
		public IActionResult Post(string id)
		{
			Post post = repository.GetPost(id);
			return View(post);
		}


		[HttpGet]
		public IActionResult Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View(new Post());
			}

			Post post = repository.GetPost(id);
			return View(post);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Post post)
		{

			if (ModelState.IsValid)
			{
				try
				{
					Post existingPost = repository.GetPost(post.Id.ToString());

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
