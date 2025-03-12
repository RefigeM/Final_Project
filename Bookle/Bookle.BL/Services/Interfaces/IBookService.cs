using Bookle.BL.ViewModels.AuthorVMs;
using Bookle.BL.ViewModels.FilterVMs;
using Bookle.BL.ViewModels.HomeVM;
using Bookle.Core.Entities;
using Bookle.Core.Enums;
using Bookle.Core.Repositories;

namespace Bookle.BL.Services.Interfaces;

public interface IBookService
{
	Task<IEnumerable<Book>> GetAllBooksAsync();
	Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync();
	Task<Book> GetBookByIdAsync(int id);
	Task AddBookAsync(Book book);
	Task DeleteBookAsync(int id);
	Task RestoreBookAsync(int id);
	Task SoftDeleteBookAsync(int id);
	Task UpdateBookAsync(int id, BookUpdateVM vm);
	Task ToggleIsFeaturedAsync(int id);
	Task<List<Book>> GetTopRatedBooksAsync(int count);
    IEnumerable<Book> Search(string query);
    GenreBooksVM GetBooksByGenre(Genre? genre);
    IEnumerable<Genre> GetAllGenres();
    IEnumerable<Book> GetBooksByAuthor(string authorName);
    FormatBookVM GetBooksByFormat(Format? format);

    IEnumerable<Format> GetAllFormats();
	Task<IEnumerable<Book>> SearchBooksAsync(string searchQuery);



}
