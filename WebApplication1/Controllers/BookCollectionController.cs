using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("BookCollection")]
    public class BookCollectionController : Controller
    {
        [HttpGet("books")]
        public IActionResult GetBooks(int ?id)
        {
            return Ok();
        }

        [HttpPost("books")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBook(JsonObject newBook)
        {
            return Ok();
        }

        [HttpDelete("books")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBook(int id)
        {
            return Ok();
        }
    }
}
