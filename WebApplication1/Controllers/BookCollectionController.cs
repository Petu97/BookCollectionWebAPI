using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("books")]
    public class BookCollectionController : Controller
    {
        private readonly BookCollectionDbDataAccess bookCollectionDbDataAccess;

        public BookCollectionController(BookCollectionDbDataAccess _bookCollectionDbDataAccess)
        {
            bookCollectionDbDataAccess = _bookCollectionDbDataAccess;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookById(int id) //find item from db with given id
        {
            Book? result = await bookCollectionDbDataAccess.FindItemById(id);
            if (result is not null) 
                return Ok(result);

            else return 
                    BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string ?author, [FromQuery] string ?publisher, [FromQuery] int ?year) //find and return books with given params
        {
            if (!this.HttpContext.Request.QueryString.HasValue) //no querystring given, find and return all books
            {
                var allBooks = await bookCollectionDbDataAccess.FindAllItems();
                if (allBooks is not null)
                    return Ok(allBooks); //return list of all the books

                else
                    return BadRequest(); //failed to fetchbooks
            }

            if (author is null && publisher is null && year is null) //no valid params given, return bad request
                return BadRequest();

            else //find books with params
            {
                var booksWithParams = await bookCollectionDbDataAccess.FindItems(author, publisher, year); //fetch list of books with given params

                if (booksWithParams is null) //failed to find books with given params
                    return BadRequest();

                else
                    return Ok(booksWithParams); //return list of found items
            }
        }

        [HttpPost] 
        public async Task<IActionResult> CreateBook(JsonObject newBook) //method for handling post requests
        {
            Book book = JsonSerializer.Deserialize<Book>(newBook.ToString()); //Deserializes given JSONdataobject into a book model

            if (TryValidateModel(book)) //checks if new book is valid
            {
                var result = await bookCollectionDbDataAccess.AddItem(book); //adds a book to db, returns id of the created item
                if(result is not null)  
                    return Ok(result); //Validates new book if successfull returns ok

                else
                    return BadRequest(); //creating a book failed
            }
            else return 
                    BadRequest(); //the new book is not valid
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id) //method for handling delete requests
        {
            var result = await bookCollectionDbDataAccess.DeleteItem(id);
            if (result) return NoContent(); //Item deleted successfully

            else return BadRequest(); //item did not exist or db was unable to finish task
        }
    }
}
