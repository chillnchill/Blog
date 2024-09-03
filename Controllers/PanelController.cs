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

		public async Task<IActionResult> Index()
		{
			IEnumerable<PostViewModel> posts = await repository.GetAllPostsAsync();

			return View(posts);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View(new PostViewModel());
			}

			Post post = await repository.GetPostAsync(id);
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
		public async Task<IActionResult> Edit(PostViewModel vm)
		{
			Post post = new Post()
			{
				Id = vm.Id,
				Title = vm.Title,
				Body = vm.Body,
				Description = vm.Description,
				Category = vm.Category,
				Tags = vm.Tags
			};

			if (vm.Image == null)
			{
				post.Image = vm.CurrentImage;
			}
			else
			{
				post.Image = await fileManager.SaveImage(vm.Image);
			}

			if (ModelState.IsValid)
			{
				try
				{
					Post existingPost = await repository.GetPostAsync(vm.Id.ToString());

					if (existingPost == null)
					{
						await repository.AddPostAsync(post);
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
			Post post = await repository.GetPostAsync(id);

			if (post == null)
			{
				return NotFound();
			}

			foreach (var comment in post.MainComments)
			{
				var subComments = await repository.GetSubCommentsByMainCommentIdAsync(comment.Id);
				repository.DeleteRange(subComments);
			}

			repository.DeleteRange(post.MainComments);
			await repository.RemovePostAsync(id);

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
