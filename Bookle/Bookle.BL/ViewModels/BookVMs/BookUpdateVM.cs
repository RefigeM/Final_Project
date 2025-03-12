using Bookle.Core.Entities;
using Bookle.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class BookUpdateVM
{
    [Required]
    [MaxLength(32, ErrorMessage = "32-dən çox ola bilməz.")]
    public string Title { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    [MaxLength(700)]
    public string? Description { get; set; }

    public string? RoleOfBook { get; set; }

    [Required]
    public Genre Genre { get; set; }

    [Required]
    public Format Format { get; set; }

    public string? ISBN { get; set; }

    public string? Country { get; set; }

    [Required]
    [Range(1000, 2025)]
    public int PublishedYear { get; set; }

    [Range(1, 10000)]
    public int PageCount { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    [MaxLength(50)]
    public string Language { get; set; }

    public string? FileUrl { get; set; }

    // IFormFile nullable olmalıdır, çünki şəkil hər zaman olmaya bilər
    public IFormFile? File { get; set; }

    // Implicit operator kodu
    public static implicit operator Book(BookUpdateVM vm)
    {
        return new Book
        {
            Title = vm.Title,
            AuthorId = vm.AuthorId,
            Language = vm.Language,
            PublishedYear = vm.PublishedYear,
            Genre = vm.Genre,
            Format = vm.Format,
            PublishingCountry = vm.Country,
            ISBN = vm.ISBN,
            PageCount = vm.PageCount,
            Price = vm.Price,
            Description = vm.Description,
            ShortDescription = vm.ShortDescription,
            RoleOfBook = vm.RoleOfBook,
            CoverImageUrl = vm.File != null ? "/imgs/books/" + vm.File.FileName : null
        };
    }
}
