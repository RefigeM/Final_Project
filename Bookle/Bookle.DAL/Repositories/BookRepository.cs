﻿using Bookle.Core.Entities;
using Bookle.Core.Enums;
using Bookle.Core.Repositories;
using Bookle.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bookle.DAL.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
	private readonly BookleDbContext _context;


	public BookRepository(BookleDbContext context) : base(context)
	{
		_context = context;
	}

    public IEnumerable<Format> GetAllFormat()
    {
        return Enum.GetValues(typeof(Format)).Cast<Format>().ToList();
    }

    public IEnumerable<Genre> GetAllGenres()
    {
        return Enum.GetValues(typeof(Genre)).Cast<Genre>().ToList();
    }

   

    public IEnumerable<Book> GetBooksByAuthor(string authorName)
    {
        return _context.Books
            .Include(b => b.Author)
            .Where(b => b.Author.AuthorName == authorName)
            .ToList();
    }

    public IEnumerable<Book> GetBooksByFormat(Format? format)
    {
        var books = _context.Books.AsQueryable();

        if (format.HasValue)
        {
            books = books.Include(b => b.BookRatings).Where(b => b.Format == format.Value);
        }

        return books.ToList();
    }

    public IEnumerable<Book> GetBooksByGenre(Genre? genre)
    {
        var books = _context.Books.AsQueryable();

        if (genre.HasValue)
        {
            books = books.Include(b => b.BookRatings).Where(b => b.Genre == genre.Value);
        }

        return books.ToList(); 
    }


    public async Task<Book> GetByIdWithDetailsAsync(int id)
	{
		return await _context.Books
	   .Include(b => b.Author)
	   .FirstOrDefaultAsync(b => b.Id == id);

	}

    public async Task<List<Book>> GetTopRatedBooksAsync(int count)
    {
        return await _context.Books
          .OrderByDescending(b => b.BookRatings.Any() ? b.BookRatings.Average(r => r.RatingRate) : 0)
          .Take(count)  
          .ToListAsync();
    }

    public IEnumerable<Book> Search(string query)
    {
        var books = _context.Books
        .Include(b => b.Author)
        .Include(b => b.BookRatings)
        .Include(b => b.Comments) 
        .Where(b => b.Title.Contains(query) || (b.Author != null && b.Author.AuthorName.Contains(query)))
        .ToList();

        return books;
    }

	

	public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
	{
		return await _context.Books
				   .Where(b => b.Title.Contains(title))
				   .ToListAsync();
	}
	public IQueryable<Book> GetAllBooksWithDetails()
	{
		return _context.Books
			.Include(b => b.Author)
			.Include(b => b.BookRatings)
			.AsQueryable();
	}
}
