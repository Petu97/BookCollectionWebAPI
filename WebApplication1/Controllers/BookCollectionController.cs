using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("BookCollection")]
    public class BookCollectionController : Controller
    {
        private readonly BookCollectionDbContext context;

        public BookCollectionController(BookCollectionDbContext _context)
        {
            context = _context;
        }

        [HttpGet("books")]
        public IActionResult GetBooks([FromQuery] string ?author, [FromQuery] string ?publisher, [FromQuery] int ?year)
        {
            return Ok("author:" + author + " publisher:" + publisher + " year:" + year);
        }

        [HttpPost("books")]
        public async Task<IActionResult> CreateBook(JsonObject newBook)
        {
            Book book = JsonSerializer.Deserialize<Book>(newBook.ToString()); //Deserializes given JSONdataobject into a book model
            if (TryValidateModel(book))
            {
                return Ok(book); //Validates new book, if successfull returns ok
            }
           
            else return BadRequest();
        }

        [HttpDelete("books")]
        public IActionResult DeleteBook(int id)
        {
            return Ok();
        }
    }
}
