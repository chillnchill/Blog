using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Data.FileManager;
using Blog.Data.Services;
using Blog.Extensions;
using Blog.Models;
using Blog.Models.Comments;
using Blog.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace Blog.Controllers
{
	public class HomeController : Controller
	{
		private IRepository repository;
		private IFileManager fileManager;
		HtmlSanitizer sanitizer = new HtmlSanitizer();
		private readonly ILogger<HomeController> logger;
		private readonly INotyfService notyf;

		public HomeController(IRepository repository, IFileManager fileManager,
			ILogger<HomeController> logger, INotyfService notyf)
		{
			this.repository = repository;
			this.fileManager = fileManager;
			this.logger = logger;
			this.notyf = notyf;
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

                PostViewModel vm = new PostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    Description = post.Description,
                    Tags = post.Tags,
                    Category = post.Category,
                    CurrentImage = post.Image,
                    MainComments = post.MainComments,
                };


                ViewData["Title"] = vm.Title;
                ViewData["Description"] = vm.Description;
                ViewData["Keywords"] = vm.Tags;

                return View(vm);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving post with ID {id}.");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the post."});
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
			string userId = User.GetId()!;

			if (string.IsNullOrEmpty(userId))
			{
				notyf.Error("You need to be logged in to write a comment!", 5);
				return RedirectToAction("Post", new { id = vm.PostId });
			}

			vm.UserId = userId;

			if (!ModelState.IsValid)
			{
				return RedirectToAction("Post", new { id = vm.PostId });
            }
	
			string sanitizedMessage = sanitizer.Sanitize(vm.Message);     

			Post post = await repository.GetPostAsync(vm.PostId);

			if (post == null)
			{
				logger.LogWarning($"Attempt to comment on a non-existent post with ID {vm.PostId}");
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
					UserId = vm.UserId,
					Message = sanitizedMessage,
					CreatedOn = DateTime.Now,
				});

				repository.UpdatePost(post);
			}
			else
			{
				SubComment comment = new SubComment
				{
					UserId = vm.UserId,
					MainCommentId = vm.MainCommentId,
					Message = sanitizedMessage,
					CreatedOn = DateTime.Now,
				};

				await repository.AddSubCommentAsync(comment);
			}

			await repository.SaveChangesAsync();

			return RedirectToAction("Post", new { id = vm.PostId });
		}

		[HttpGet]
		//i have to check if the postId is being successfully grabbed tomorrow 
		public async Task<IActionResult> EditComment(int commentId, string postId)
		{
			//check if this is not null or some dork shit
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			Comment comment = await repository.FetchCommentForEditAsync(commentId, userId);

			if (comment == null)
			{				
				return NotFound("Comment not found or you do not have permission to edit this comment.");
			}

			CommentViewModel vm = new CommentViewModel
			{
				PostId = postId,
				UserId = comment.UserId,
				MainCommentId = comment.Id,
				Message = comment.Message,
				CreatedOn = comment.CreatedOn
			};
			return View(vm);  // Return the view with the populated view model
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditComment(CommentViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			Comment comment = await repository.FetchCommentForEditAsync(vm.MainCommentId, vm.UserId);

			if (comment == null)
			{
				return NotFound("Comment not found or you do not have permission to edit this comment.");
			}

			// Update the entity model with new data from the view model
			comment.Message = vm.Message;

			// Save changes to the database
			bool success = await repository.SaveChangesAsync();

			if (!success)
			{
				ModelState.AddModelError(string.Empty, "Failed to update the comment.");
				return View(vm);
			}

			// Redirect back to the post or another page
			return RedirectToAction("Post", "Home", new { id = vm.PostId });

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
