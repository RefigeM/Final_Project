using Bookle.BL.Exceptions;
using Bookle.BL.Extentions;
using Bookle.BL.Services.Interfaces;
using Bookle.BL.ViewModels.AuthorVMs;
using Bookle.BL.ViewModels.BookVMs;
using Bookle.BL.ViewModels.FilterVMs;
using Bookle.BL.ViewModels.HomeVM;
using Bookle.Core.Entities;
using Bookle.Core.Enums;
using Bookle.Core.Repositories;
using Bookle.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

using Bookle.DAL.Repositories;

namespace Bookle.BL.Services.Implements;

public class BookService(IBookRepository _repo, BookleDbContext _context, IAuthorRepository _authorRepo) : IBookService
{
    public async Task AddBookAsync(Book book)
    {
        if (book == null) throw new NotFoundException("Book is null");
        await _repo.AddAsync(book);
        await _repo.SaveAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book == null) throw new NotFoundException("Book is null");
        await _repo.DeleteAsync(id);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        var data = await _repo.GetAllAsync();
        if (data == null) throw new NotFoundException();
        return data;
    }

    public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync()
    {
		var query =_repo.GetAllBooksWithDetails();  
		return await query.ToListAsync();
	}

    public IEnumerable<Format> GetAllFormats()
    {
        return _repo.GetAllFormat();
    }

    public IEnumerable<Genre> GetAllGenres()
    {
        return _repo.GetAllGenres();
    }

    public async Task<Book> GetBookByIdAsync(int id)
    {
        var book = await _repo.GetByIdWithDetailsAsync(id);
        if (book == null) throw new NotFoundException();
        return book;

    }

    public IEnumerable<Book> GetBooksByAuthor(string authorName)
    {
        return _repo.GetBooksByAuthor(authorName);
    }

    public GenreBooksVM GetBooksByGenre(Genre? genre)
    {
        var model = new GenreBooksVM()
        {
            Genres = Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList(),
            Books = _repo.GetBooksByGenre(genre).ToList() // Düzgün yerə yerləşdirildi
        };

        return model;
    }

    public FormatBookVM GetBooksByFormat(Format? format)
    {
        var model = new FormatBookVM()
        {
            Formats = Enum.GetValues(typeof(Format)).Cast<Format>().ToList(),
            Books = _repo.GetBooksByFormat(format).ToList() 
        };

        return model;
    }

    public async Task<List<Book>> GetTopRatedBooksAsync(int count)
    {
        return await _repo.GetTopRatedBooksAsync(4);
    }

    public async Task RestoreBookAsync(int id)
    {
        await _repo.RestoreAsync(id);
        await _repo.SaveAsync();
    }

    public IEnumerable<Book> Search(string query)
    {
        return _repo.Search(query);
    }

    public async Task SoftDeleteBookAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        await _repo.SaveAsync();
    }

    public async Task ToggleIsFeaturedAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book == null) throw new NotFoundException("Book is null");

        book.IsFeatured = !book.IsFeatured;
        await _repo.SaveAsync();

    }

    public async Task UpdateBookAsync(int id, BookUpdateVM vm)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book == null) throw new NotFoundException();

        book.Title = vm.Title;
        book.Price = vm.Price;
        book.Description = vm.Description;
        book.ShortDescription = vm.ShortDescription;
        book.AuthorId = vm.AuthorId;
        book.ISBN = vm.ISBN;
        book.PublishingCountry = vm.Country;
        book.Format = vm.Format;
        book.Genre = vm.Genre;
        book.PageCount = vm.PageCount;
        book.Language = vm.Language;
        book.PublishedYear = vm.PublishedYear;
        book.RoleOfBook = vm.RoleOfBook;
        book.ShortDescription = vm.ShortDescription;

        if (vm.File != null)
        {
            string newFileName = await vm.File.UploadAsync("wwwroot/imgs/books");
            book.CoverImageUrl = "/imgs/books/" + newFileName;
        }





        await _repo.SaveAsync();
    }


    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchQuery)
	{
		return string.IsNullOrEmpty(searchQuery)
			? await _repo.GetAllBooksWithDetails().ToListAsync()
			: await _repo.SearchByTitleAsync(searchQuery);
	}

}
