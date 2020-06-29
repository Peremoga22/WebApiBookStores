using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Route("api/[controller]")]
    //[ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicatinDbContext _applicatinDbContext;
        public BookController(ApplicatinDbContext applicatinDbContext)
        {
            _applicatinDbContext = applicatinDbContext;
        }
        [HttpGet(template:ApiRoutes.Books.GetAll)]
        public async Task<ActionResult<IEnumerable<Book>>>GetAllListAsync()
        {
            var res = await _applicatinDbContext.Books.ToListAsync();
            return Ok(res);
        }
        [HttpGet(template: ApiRoutes.Books.Get)]
        public async Task<IActionResult> GetById(int id)
        {
          
            var res = await _applicatinDbContext.Books.Where(b=>b.Id ==id).FirstOrDefaultAsync();
            if (res != null)
            {
                return Ok(res);
            }
            return NotFound();
        }
        [HttpPost(template: ApiRoutes.Books.Create)]
        public async Task<IActionResult> Create(Book book)
        {
            if(book.Id ==0)
            {
                var res = _applicatinDbContext.Books.Add(book);
                if (res.State == EntityState.Added)
                {
                    await _applicatinDbContext.SaveChangesAsync();
                }
            }                      
            return CreatedAtAction(nameof(GetAllListAsync), new { id = book.Id }, book);
        }
        [HttpPut(template:ApiRoutes.Books.Update)]
        public async Task<IActionResult> PutBookAsync( Book book)
        {
            if(book.Id==0)
            {
                return BadRequest();
            }
            _applicatinDbContext.Entry(book).State = EntityState.Modified;
            try
            {
               
                _applicatinDbContext.Books.Update(book);
                await _applicatinDbContext.SaveChangesAsync();

            }
            catch(DbUpdateConcurrencyException)
            {
                if(!BookItemExists(book.Id))
                {
                    return NotFound();
                }               
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [HttpDelete(template: ApiRoutes.Books.Delete)]
        public async Task<ActionResult<Book>> DeleteAsync(int id)
        {
            var bookItem = await _applicatinDbContext.Books.FindAsync(id);
            if(bookItem == null)
            {
                return NotFound();
            }
            _applicatinDbContext.Books.Remove(bookItem);
            await _applicatinDbContext.SaveChangesAsync();
            return bookItem;
        }
        private bool BookItemExists(int id) =>
       _applicatinDbContext.Books.Any(e => e.Id == id);
    }
}
