using Bookle.BL.Extentions;
using Bookle.BL.Helpers;
using Bookle.BL.Services.Implements;
using Bookle.BL.Services.Interfaces;
using Bookle.Core.Entities;
using Bookle.Core.Enums;
using Bookle.Core.Repositories;
using Bookle.DAL.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookle.MVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = RoleConstants.Book)]


	public class BookController(BookleDbContext _context, IWebHostEnvironment _env, IBookService _service, IBookRepository _bookRepo) : Controller
	{
		public async Task<IActionResult> Index(int? page = 1, int? take = 6)
		{
			if (!page.HasValue) page = 1;
			if (!take.HasValue) take = 6;

			var query = _bookRepo.GetAllBooksWithDetails();

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
			var data = await _service.GetBookByIdAsync(id.Value);
			if (data == null) return NotFound();
			await _service.DeleteBookAsync(id.Value);

			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Hide(int? id)
		{
			if (id == null) return BadRequest();
			await _service.SoftDeleteBookAsync(id.Value);
			return RedirectToAction(nameof(Index));

		}
		public async Task<IActionResult> Show(int? id)
		{
			if (id == null) return BadRequest();
			await _service.RestoreBookAsync(id.Value);
			return RedirectToAction(nameof(Index));

		}
		public async Task<IActionResult> Create()
		{
			ViewBag.Authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
			ViewBag.Languages = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish", "French", "Spanish" });
			ViewBag.Countries = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish" });
			ViewBag.Formats = new SelectList(Enum.GetNames(typeof(Format)));
			ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genre)));

			return View();

		}
		[HttpPost]
		public async Task<IActionResult> Create(BookCreateVM vm)
		{
			if (vm.File != null)
			{
				// Validate file type and size
				if (!vm.File.IsValidType("image"))
					ModelState.AddModelError("File", "File must be an image");

				if (!vm.File.IsValidSize(400)) // Size should be less than 400 KB
					ModelState.AddModelError("File", "File must be less than 400 KB");
			}

			if (!ModelState.IsValid)
			{
				ViewBag.Authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
				ViewBag.Languages = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish", "French", "Spanish" });
				ViewBag.Formats = new SelectList(Enum.GetNames(typeof(Format)));
				ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genre)));
				ViewBag.Countries = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish" });

				return View(vm);
			}

			if (!await _context.Authors.AnyAsync(x => x.Id == vm.AuthorId))
			{
				ViewBag.Authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
				ModelState.AddModelError("AuthorId", "Author not found");
				return View(vm);
			}

			// Convert ViewModel to Entity
			Book book = new Book
			{
				Title = vm.Title,
				AuthorId = vm.AuthorId,
				ShortDescription = vm.ShortDescription,
				Description = vm.Description,
				RoleOfBook = vm.RoleOfBook,
				Genre = vm.Genre,
				Format = vm.Format,
				ISBN = vm.ISBN,
				PublishingCountry = vm.Country,
				PublishedYear = vm.PublishedYear,
				PageCount = vm.PageCount,
				Price = vm.Price,
				Language = vm.Language,
				CoverImageUrl = vm.FileUrl // We'll set this URL once we upload the file
			};

			// If File is not null, upload the file
			if (vm.File != null)
			{
				// Define file path and upload
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
				string filePath = Path.Combine(_env.WebRootPath, "imgs", "books", fileName);

				// Upload the file
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await vm.File.CopyToAsync(stream);
				}

				// Set the file URL for saving in the database
				book.CoverImageUrl = "/imgs/books/" + fileName;
			}

			// Save the book to the database
			await _service.AddBookAsync(book);

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Info(int? id)
		{
			if (id == null) return BadRequest();
			var book = await _service.GetBookByIdAsync(id.Value);
			return View(book);
		}

		public async Task<IActionResult> Update(int? id)
		{
			if (id == null) return BadRequest();

			var book = await _service.GetBookByIdAsync(id.Value);
			if (book == null) return NotFound();

			var authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
			ViewBag.Authors = authors.Any() ? new SelectList(authors, "Id", "AuthorName") : new SelectList(Enumerable.Empty<SelectListItem>());

			ViewBag.Languages = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish", "French", "Spanish" });
			ViewBag.Countries = new SelectList(new List<string> { "USA", "Azerbaijan", "Turkey" });
			ViewBag.Formats = new SelectList(Enum.GetNames(typeof(Format)));
			ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genre)));
            var bookVM = new BookUpdateVM
            {
                Title = book.Title,
                AuthorId = book.AuthorId,
                ShortDescription = book.ShortDescription,
                Description = book.Description,
                PublishedYear = book.PublishedYear,
                ISBN = book.ISBN,
                RoleOfBook = book.RoleOfBook,
                Language = book.Language,
                Country = book.PublishingCountry,
                Genre = book.Genre,
                Format = book.Format,
                PageCount = book.PageCount,
                Price = book.Price,
                FileUrl = book.CoverImageUrl  // Burada CoverImageUrl ötürülür
            };



            return View(bookVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(int? id, BookUpdateVM vm)
		{
			if (id == null) return BadRequest();

			// Müəllifin mövcud olub-olmadığını yoxlayırıq
			var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == vm.AuthorId);
			if (author == null || author.IsDeleted)
			{
				ModelState.AddModelError("AuthorId", "The selected author is either deleted or does not exist.");
			}

            if (!ModelState.IsValid)
            {
                // Log or display validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Səhv mesajlarını ekranda göstərmək
                }

                // Səhv baş verərsə, ViewBag-ləri doldururuq
                var authors = await _context.Authors.Where(x => !x.IsDeleted).ToListAsync();
                ViewBag.Authors = authors.Any()
                    ? new SelectList(authors, "Id", "AuthorName")
                    : new SelectList(Enumerable.Empty<SelectListItem>());

                ViewBag.Languages = new SelectList(new List<string> { "English", "Azerbaijani", "Turkish", "French", "Spanish" });
                ViewBag.Countries = new SelectList(new List<string> { "USA", "Azerbaijan", "Turkey" });
                ViewBag.Formats = new SelectList(Enum.GetNames(typeof(Format)));
                ViewBag.Genres = new SelectList(Enum.GetNames(typeof(Genre)));

                return View(vm);
            }

            await _service.UpdateBookAsync(id.Value, vm);
			return RedirectToAction(nameof(Index));
		}


		public async Task<IActionResult> ToggleIsFeatured(int? id)
		{
			if (id == null) return BadRequest();
			await _service.ToggleIsFeaturedAsync(id.Value);
			return RedirectToAction(nameof(Index));

		}
		public async Task<IActionResult> BookSearch(string searchQuery)
		{
			if (string.IsNullOrEmpty(searchQuery))
			{
				return RedirectToAction("Index");  
			}

			var books = await _service.SearchBooksAsync(searchQuery);
			ViewData["searchQuery"] = searchQuery;  
			return View("Index", books); 
		}


	}
}
