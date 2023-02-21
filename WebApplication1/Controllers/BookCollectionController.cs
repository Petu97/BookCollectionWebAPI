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
        private readonly BookCollectionDbDataAccess bookCollectionDbDataAccess;

        public BookCollectionController(BookCollectionDbDataAccess _bookCollectionDbDataAccess)
        {
            bookCollectionDbDataAccess = _bookCollectionDbDataAccess;
        }


        [HttpGet("books")]
        public IActionResult GetBooks([FromQuery] string ?author, [FromQuery] string ?publisher, [FromQuery] int ?year)
        {
            if (author is null && publisher is null && year is null)
                return Ok(bookCollectionDbDataAccess.FindAllItems().ToString());

            else return Ok(bookCollectionDbDataAccess.FindObject(author, publisher, year));
        }

        [HttpPost("books")]
        public async Task<IActionResult> CreateBook(JsonObject newBook)
        {
            Book book = JsonSerializer.Deserialize<Book>(newBook.ToString()); //Deserializes given JSONdataobject into a book model
            Console.WriteLine("Book title: " + book.Title);
            if (TryValidateModel(book)) //checks if new book is valid
            {
                Console.WriteLine("validation successfull");
                var result = await bookCollectionDbDataAccess.AddItem(book);
                if(result is not null)  return Ok(result); //Validates new book if successfull returns ok

                else return BadRequest(); //creating a book failed
            }
            else return BadRequest(); //the new book is not valid
        }

        [HttpDelete("books")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await bookCollectionDbDataAccess.DeleteItem(id);
            if (result) return NoContent(); //Item deleted successfully

            else return BadRequest(); //item did not exist or db was unable to finish task
        }
    }
}
