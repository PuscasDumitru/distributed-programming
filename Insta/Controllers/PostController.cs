using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;

namespace Insta.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class PostController : ControllerBase
    {
        private readonly DBContext _context;

        public PostController(DBContext context)
        {
            _context = context;
        }

        // GET: Post
        [HttpGet]
        public async Task<IEnumerable<Post>> Index()
        {
            return await _context.Posts.ToListAsync();
        }

        [HttpGet]
        // GET: Post/Details/5
        public async Task<Post> Details(int? id)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);

            return post;
        }
        
        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<Post> Create([Bind("Id,Title,Image")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();

            }
            return post;
        }
        
        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<Post> Edit(int id, [Bind("Id,Title,Image")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
            }
            return post;
        }
        
        // POST: Post/Delete/5
        [HttpPost]
        public async Task Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }
}
