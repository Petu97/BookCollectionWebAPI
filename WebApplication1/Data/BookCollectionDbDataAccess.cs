using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Linq;


namespace WebApplication1.Data
{
    public class BookCollectionDbDataAccess
    {
        private readonly BookCollectionDbContext DbContext;

        public BookCollectionDbDataAccess(BookCollectionDbContext _context)
        {
            DbContext = _context;
        }

        public async Task<List<Book>> FindAllItems() //find and return all books in database
        {
            var result = await DbContext.books.ToListAsync();
            return result;
        }

        public async Task<Book> FindItemById(int id) //find and return all books in database
        {
            Book? result = await DbContext.books.Where(b => b.Id == id).FirstAsync();
            return result;
        }

        public async Task<List<Book?>> FindItem(string ?author, string ?publisher, int ?year)
        { 

            List<Book> ?booksByAuthor = await DbContext.books.Where(s =>  s.Author.Equals(author) && s.Publisher.Equals(publisher) && s.Year.Equals(year)).ToListAsync();

            return booksByAuthor;
        }

        public async Task<int?> AddItem(Book newBook) //create a new book if identical one doesn't exist
        {
            try
            {
                Console.WriteLine("Attempting to find a duplicate book.");
                List<Book> ?checkForDuplicates = await DbContext.books.Where(b => b.Title == newBook.Title).ToListAsync(); //fetch books with the same title as the new book from db and add them to a list
                checkForDuplicates = checkForDuplicates.Where(b => b.Author == newBook.Author).ToList(); //iterate through the list to find matching authors
                checkForDuplicates = checkForDuplicates.Where(b => b.Year == newBook.Year).ToList(); //iterates through the  list for the final time to find books published the same year as new book

                Console.WriteLine("items in list" + checkForDuplicates.Count);
                //if the list is empty after these steps, the new book is unique and will be added to db
                if (checkForDuplicates == null || checkForDuplicates.Count == 0)
                {
                    Console.WriteLine("No duplicates found");
                    var result = DbContext.books.Update(newBook);
                    Console.WriteLine(result + " State: " + result.State);
                    await DbContext.SaveChangesAsync();
                    return result.Entity.Id;
                }
                else return null; //the new book already existes returning null probably not the way to go
            }
            catch 
            {
                return null;
            }
        }

        public async Task<bool> DeleteItem(int id) //Search for an item by id and delete it if found
        {
            try
            {
                var book = await DbContext.books.FindAsync(id); //look for a book with id
                if (book is not null)
                {
                    DbContext.books.Remove(book);
                    await DbContext.SaveChangesAsync(); //save changes to db
                    return true;
                }
                else return false;
            }
            catch 
            {
                return false;
            }
        }
    }
}
