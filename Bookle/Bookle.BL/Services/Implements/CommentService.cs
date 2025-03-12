using Bookle.BL.Exceptions;
using Bookle.BL.Services.Interfaces;
using Bookle.BL.ViewModels.CommentVMs;
using Bookle.Core.Entities;
using Bookle.Core.Repositories;
using Bookle.DAL.Contexts;
using Bookle.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookle.BL.Services.Implements;

public class CommentService(BookleDbContext _context, ICommentRepository _repo) : ICommentService
{
	public void AddComment(int bookId, string userId, string content)
	{
		var comment = new Comment
		{
			BookId = bookId,
			UserId = userId,
			Content = content,
			CreatedDate = DateTime.Now
		};

		_context.Comments.Add(comment);
		_context.SaveChanges();
	}

	public async Task AddCommentAsync(Comment comment)
	{
		if (comment == null) throw new NotFoundException("comment is null");

		await _repo.AddAsync(comment);
		await _repo.SaveAsync();
	}

	public async Task DeleteCommentAsync(int id)
	{
		var comment =await _repo.GetByIdAsync(id);
		if (comment == null) throw new NotFoundException();
		_repo.DeleteAsync(id);
		await _repo.SaveAsync();	

	}

	public async Task<List<Comment>> GetAllCommentsAsync()
	{
		var comments = await _repo.GetAllAsync();	
		if (comments.Count == 0) throw new NotFoundException();
		return comments;
	}

	public Task<Comment> GetCommentIdtWithDetailsAsync(int id)
	{
	return	_repo.GetCommentWithBookUserAuthorWithIdAsync(id);	
	}

	public async Task<Comment> GetCommentByIdAsync(int id)
	{
		var comment = await _repo.GetByIdAsync(id); 
		if (comment == null)
		{
			throw new NotFoundException($"Comment with ID {id} not found.");
		}
		return comment;
	}


	public List<Comment> GetCommentsByBookId(int bookId)
	{
		return _context.Comments
					.Where(c => c.BookId == bookId)
					.Include(c => c.User) // İstifadəçi məlumatlarını da çəkirik
					.OrderByDescending(c => c.CreatedDate)
					.ToList();
	}

	public async Task<List<Comment>> GetCommentWithBookAndUser()
	{
		return await _repo.GetAllCommentsWithDetailsAsync();
	}

	public Task RestoreCommentAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task SoftDeleteCommentAsync(int id)
	{
		throw new NotImplementedException();
	}

	public async Task<Comment> ToggleApprovalAsync(int id)
	{
		var comment = await _repo.GetCommentWithBookUserAuthorWithIdAsync(id);

		if (comment == null)
		{
			throw new NotFoundException($"Comment with ID {id} not found.");
		}

		comment.IsApproved = !comment.IsApproved;

		await _context.SaveChangesAsync();

		return comment;
	}


	public async Task UpdateCommentAsync(int id, CommentUpdateVM vm)
	{
		var comment = await _repo.GetByIdAsync(id);
		if(comment == null) throw new NotFoundException();

		comment.BookId = vm.BookId;
		comment.UserId=vm.UserId;
		comment.Content = vm.Content;

		await _repo.SaveAsync();

	}

	public IQueryable<Comment> GetAllCommentsWithDetails()
	{
		var query = _repo.GetAllCommentsWithDetails();
		return query;
	}
}
