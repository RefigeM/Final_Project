using Bookle.BL.ViewModels.CommentVMs;
using Bookle.Core.Entities;

namespace Bookle.BL.Services.Interfaces;

public interface ICommentService
{
	void AddComment(int bookId, string userId, string content);
	List<Comment> GetCommentsByBookId(int bookId);
	Task<List<Comment>> GetAllCommentsAsync();
	Task<Comment> GetCommentByIdAsync(int id);
	Task AddCommentAsync(Comment comment);
	Task DeleteCommentAsync(int id);
	Task RestoreCommentAsync(int id);
	Task SoftDeleteCommentAsync(int id);
	Task UpdateCommentAsync(int id, CommentUpdateVM vm);
	Task<List<Comment>> GetCommentWithBookAndUser();
	Task<Comment> ToggleApprovalAsync(int id);
	Task<Comment> GetCommentIdtWithDetailsAsync(int id);
	IQueryable<Comment> GetAllCommentsWithDetails();


}
