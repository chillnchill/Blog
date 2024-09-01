using Blog.Data.FileManager;
using Blog.Data.Services;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Blog.Controllers
{
	public class HomeController : Controller
	{
		private IRepository repository;
		private IFileManager fileManager;
		HtmlSanitizer sanitizer = new HtmlSanitizer();
		private readonly ILogger<HomeController> logger;

		public HomeController(IRepository repository, IFileManager fileManager, ILogger<HomeController> logger)
		{
			this.repository = repository;
			this.fileManager = fileManager;
			this.logger = logger;
		}

		public async Task<IActionResult> Index(string category, int pageNumber = 1)
		{
			ViewData["Title"] = "Blog";
			ViewData["Description"] = "Where I write my words";
			ViewData["Keywords"] = "blog, programming, games, books";

            if (pageNumber < 1)
            {
                logger.LogWarning("Invalid page number {PageNumber}, redirecting to page 1.", pageNumber);
                return RedirectToAction("Index", new { pageNumber = 1, category });
            }

            try
            {
                IndexViewModel vm = await repository.GetAllPostsForPaginationAsync(pageNumber, category);

                if (vm == null)
                {
                    logger.LogWarning($"No posts found for category {category} on page {pageNumber}.");
                    return RedirectToAction("Error", "Home", new { message = "No posts found." });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving posts for category {category} on page {pageNumber}.");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving posts." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Post(string id)
        {
            try
            {
                Post post = await repository.GetPostAsync(id);
                if (post == null)
                {
                    logger.LogWarning($"Post with ID {id} not found.");
                    return RedirectToAction("Error", "Home", new { message = "Post not found." });
                }

                ViewData["Title"] = post.Title;
                ViewData["Description"] = post.Description;
                ViewData["Keywords"] = post.Tags;

                return View(post);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving post with ID {id}.");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the post." });
            }
        }

        [HttpGet("/Image/{image}")]
		public IActionResult Image(string image)
		{
			string mime = image.Substring(image.LastIndexOf('.') + 1);
			return new FileStreamResult(fileManager.ImageStream(image), $"image/{mime}");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Comment(CommentViewModel vm)
		{
			if (!ModelState.IsValid)
			{
                return RedirectToAction("Post", new { id = vm.PostId });
            }
	
			string sanitizedMessage = sanitizer.Sanitize(vm.Message);     

			Post post = await repository.GetPostAsync(vm.PostId);

			if (post == null)
			{
				logger.LogWarning($"Attempt to comment on a non-existent post with ID {vm.PostId}", vm.PostId);
				return RedirectToAction("Error", "Home", new { message = "Post not found." });
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

				await repository.AddSubCommentAsync(comment);
			}

			await repository.SaveChangesAsync();

			return RedirectToAction("Post", new { id = vm.PostId });
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            ViewBag.ErrorMessage = message;

            return View(errorViewModel);  
        }
    }
}
