using Blog.Data.FileManager;
using Blog.Data.Services;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
	public class HomeController : Controller
	{
		private IRepository repository;
		private IFileManager fileManager;
		HtmlSanitizer sanitizer = new HtmlSanitizer();

		public HomeController(IRepository repository, IFileManager fileManager)
		{
			this.repository = repository;
			this.fileManager = fileManager;
		}

		public IActionResult Index(string category, int pageNumber = 1)
		{
			ViewData["Title"] = "Blog";
			ViewData["Description"] = "Where I write my words";
			ViewData["Keywords"] = "blog, programming, games, books";

			if (pageNumber < 1)
			{
				return RedirectToAction("Index", new { pageNumber = 1, category });
			}

			IndexViewModel vm = repository.GetAllPostsForPagination(pageNumber, category);

			return View(vm);
		}

		[HttpGet]
		public IActionResult Post(string id)
		{

			Post post = repository.GetPost(id);
			ViewData["Title"] = post.Title;
			ViewData["Description"] = post.Description;
			ViewData["Keywords"] = post.Tags;
			return View(post);
		}

		[HttpGet("/Image/{image}")]
		public IActionResult Image(string image)
		{
			string mime = image.Substring(image.LastIndexOf('.') + 1);
			return new FileStreamResult(fileManager.ImageStream(image), $"image/{mime}");
		}

		[HttpPost]
		public async Task<IActionResult> Comment(CommentViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Post", new { id = vm.PostId });
			}
	
			var sanitizedMessage = sanitizer.Sanitize(vm.Message);

			Post post = repository.GetPost(vm.PostId);

			if (post == null)
			{
				ModelState.AddModelError("", "Post not found.");
				return View("Post", vm);
			}

			if (vm.MainCommentId == 0)
			{
				if (post.MainComments == null)
				{
					post.MainComments = new List<MainComment>();
				}

				post.MainComments.Add(new MainComment
				{
					Message = sanitizedMessage,
					CreatedOn = DateTime.Now,
				});

				repository.UpdatePost(post);
			}
			else
			{
				SubComment comment = new SubComment
				{
					MainCommentId = vm.MainCommentId,
					Message = sanitizedMessage,
					CreatedOn = DateTime.Now,
				};

				repository.AddSubComment(comment);
			}

			await repository.SaveChangesAsync();

			return RedirectToAction("Post", new { id = vm.PostId });
		}

	}
}
