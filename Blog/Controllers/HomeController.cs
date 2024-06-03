using Blog.Data;
using Blog.Data.FileManager;
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
		private IFileManager fileManager;

        public HomeController(IRepository repository, IFileManager fileManager)
        {
            this.repository = repository;
            this.fileManager = fileManager;
        }

        public IActionResult Index(string category)
		{
			ViewData["Title"] = "Blog";
			ViewData["Description"] = "Where I write my words";
			ViewData["Keywords"] = "blog, programming, games, books";
			List<Post> posts = string.IsNullOrEmpty(category) ? repository.GetAllPosts() : repository.GetAllPosts(category);
			return View(posts);
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
	}
}
