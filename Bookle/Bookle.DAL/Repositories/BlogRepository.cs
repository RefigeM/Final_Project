﻿using Bookle.Core.Entities;
using Bookle.Core.Repositories;
using Bookle.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Bookle.DAL.Repositories;

public class BlogRepository : GenericRepository<Blog>, IBlogRepository
{
    private readonly BookleDbContext _context;
    public BlogRepository(BookleDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<Blog> GetAllPostsVisiblePostsAsync()
    {
        return  _context.Blogs
            .Where(b => b.IsVisibleOnHomepage==true)
            .OrderByDescending(p => p.CreatedDate).AsQueryable();
    }

    public  IQueryable<Blog> GetAllRecentPostsAsync()
    {
		return _context.Blogs
			   .OrderByDescending(p => p.CreatedDate)
			   .AsQueryable();
	}

	//public async Task ToggleIsVisible(int id)
	//{
 //     var data= await   _context.Blogs.FindAsync(id);
 //       if (data == null) throw new Exception();
 //       data.IsDeleted = !data.IsDeleted;   

           

	//}
}
