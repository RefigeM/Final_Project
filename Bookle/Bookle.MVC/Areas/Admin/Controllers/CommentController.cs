using Bookle.BL.Services.Interfaces;
using Bookle.BL.ViewModels.CommentVMs;
using Bookle.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookle.MVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class CommentController: Controller	
	{
		private readonly ICommentService _service;
		private readonly IBookService _bookService;
		private readonly IUserService _userService;

		public CommentController(ICommentService service, IBookService bookService, IUserService userService)
		{
			_service = service;	
			_bookService = bookService;	
			_userService = userService;	

		}

		public async Task<IActionResult> Index(int? page = 1, int? take = 4)
		{
			if (!page.HasValue) page = 1;
			if (!take.HasValue) take = 4;

			var query = _service.GetAllCommentsWithDetails();

			decimal bookCount = await query.CountAsync();

			var data = await query
				.Skip(take.Value * (page.Value - 1))
				.Take(take.Value)
				.ToListAsync();

			decimal pageCount = Math.Ceiling(bookCount / (decimal)take.Value);
			ViewBag.PageCount = pageCount;
			ViewBag.Take = take;
			ViewBag.AktivePage = page;

			return View(data);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return BadRequest();
			var commet = await _service.GetCommentByIdAsync(id.Value);
			await _service.DeleteCommentAsync(id.Value);

			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Update(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}

			var comment = await _service.GetCommentByIdAsync(id);

			var books = await _bookService.GetAllBooksAsync();
			var users = await _userService.GetAllUsersAsync();

			if (books == null || users == null)
			{
				return View("Error");  
			}

			ViewBag.Books = new SelectList(books, "Id", "Title", comment.BookId);  // `BookId`-ni seçilmiş olaraq göstəririk
			ViewBag.Authors = new SelectList(users, "Id", "UserName", comment.UserId); // `UserId`-ni seçilmiş olaraq göstəririk

			if (comment == null)
			{
				return NotFound();
			}

			var model = new CommentUpdateVM
			{
				BookId = comment.BookId,
				UserId = comment.UserId,
				Content = comment.Content
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Update(int? id, CommentUpdateVM vm)
		{
			if (id == null) return BadRequest();

			var comment = await _service.GetCommentByIdAsync(id.Value);
			if (comment == null || comment.IsDeleted)
			{
				ModelState.AddModelError("Comment", "The selected author is either deleted or does not exist.");
				return View(vm);
			}

			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			await _service.UpdateCommentAsync(id.Value, vm);

			return RedirectToAction(nameof(Index));

		}
		public async Task<IActionResult> Info(int? id)
		{
			if (id == null) return BadRequest();
			var comment = await _service.GetCommentIdtWithDetailsAsync(id.Value);
			return View(comment);
		}

		public async Task<IActionResult> Toggle(int? id) 
		{
		if(id == null) return BadRequest();
			var comment = await _service.ToggleApprovalAsync(id.Value);
			return RedirectToAction(nameof(Index));

		}
        public async Task<IActionResult> CommentUserSearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("Index");
            }

            var users = await _userService.SearchUsersAsync(searchQuery);
            ViewData["searchQuery"] = searchQuery;
            return View("Index", users);
        }


    }
}
