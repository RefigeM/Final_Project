﻿using Bookle.Core.Entities;
using Bookle.Core.Enums;

namespace Bookle.Core.Repositories
{
	public interface IBookRepository : IGenericRepository<Book>
	{
		Task<Book> GetByIdWithDetailsAsync(int id);
		IQueryable<Book> GetAllBooksWithDetails();

		Task<List<Book>> GetTopRatedBooksAsync(int count);
        IEnumerable<Book> Search(string query);
        IEnumerable<Book> GetBooksByGenre(Genre? genre);
        IEnumerable<Genre> GetAllGenres();
        IEnumerable<Book> GetBooksByAuthor(string authorName);
        IEnumerable<Book> GetBooksByFormat(Format? format);
        IEnumerable<Format> GetAllFormat();
		Task<IEnumerable<Book>> SearchByTitleAsync(string title); 





	}
}
