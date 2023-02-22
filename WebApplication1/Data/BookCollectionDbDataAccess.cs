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

        public async Task<List<Book?>> FindAllItems() //find and return all books in database
        {
            var result = await DbContext.books.ToListAsync();
            return result;
        }

        public async Task<Book?> FindItemById(int id) //find and return a book with given id from database
        {
            Book? result = await DbContext.books.Where(b => b.Id == id).FirstAsync();
            return result;
        }

        //makeshift function for finding and returning items with parameters. used until i find a way to create dynamic custom sql requests.
        //Refactored Method (WIP) at the end of the file. This method is not scalable and overall unreadable
        public async Task<List<Book?>> FindItems(string ?author, string ?publisher, int ?year) 
        {
            List<Book>? booksByAuthor, booksByPublisher, booksByYear, result;
            booksByAuthor = await DbContext.books.Where(b => b.Author.Equals(author)).ToListAsync();
            booksByYear = await DbContext.books.Where(b => b.Year == year).ToListAsync();
            booksByPublisher = await DbContext.books.Where(b => b.Publisher.Equals(publisher)).ToListAsync();

            if (publisher is null) //db allows null publisher values which we don't want to look for, clear publisher list if no param was given
                booksByPublisher.Clear();

            if (booksByAuthor.Count() > 0 && booksByPublisher.Count() > 0 && booksByYear.Count() > 0l) //none are empty
            {
                Console.WriteLine("Running function 0");
                result = booksByAuthor.Intersect(booksByPublisher).ToList();
                result = result.Intersect(booksByYear).ToList();
                return result;
            }

            else if (author is null && booksByPublisher.Count() > 0 && booksByYear.Count() > 0) //author is empty
            {
                Console.WriteLine("Running function 1");
                result = booksByPublisher.Intersect(booksByYear).ToList();
                return result;
            }

            else if (booksByAuthor.Count() > 0 && publisher is null && booksByYear.Count() > 0) //publisher is empty
            {
                Console.WriteLine("Running function 2");
                result = booksByAuthor.Intersect(booksByYear).ToList();
                return result;
            }

            else if (booksByAuthor.Count() > 0 && booksByPublisher.Count() > 0 && year is null) //year is empty
            {
                Console.WriteLine("Running function 3");
                result = booksByAuthor.Intersect(booksByPublisher).ToList();
                return result;
            }

            else if (booksByAuthor.Count() > 0 && publisher is null && year is null) //publisher and year are empty
            {
                Console.WriteLine("Running function 4");
                result = booksByAuthor;
                return result;
            }

            else if (author is null && booksByPublisher.Count() > 0 && year is null) //author and year are empty
            {
                Console.WriteLine("Running function 5");
                result = booksByPublisher;
                return result;
            }

            else if (author is null && publisher is null && booksByYear.Count() > 0) //author and publisher are empty
            {
                Console.WriteLine("Running function 6");
                result = booksByYear;
                return result;
            }

            else //everything is empty
            {
                Console.WriteLine("Running backup function");
                result = null;
                return result;
            }

        }

        public async Task<int?> AddItem(Book newBook) //create a new book if identical one doesn't exist
        {
            try
            {
                List<Book> ?checkForDuplicates = await DbContext.books.Where(b => b.Title == newBook.Title).ToListAsync(); //fetch books with the same title as the new book from db and add them to a list
                checkForDuplicates = checkForDuplicates.Where(b => b.Author == newBook.Author).ToList(); //iterate through the list to find matching authors
                checkForDuplicates = checkForDuplicates.Where(b => b.Year == newBook.Year).ToList(); //iterates through the  list for the final time to find books published the same year as new book

                //if the list is empty after these steps, the new book is unique and will be added to db
                if (checkForDuplicates == null || checkForDuplicates.Count == 0)
                {
                    var result = DbContext.books.Update(newBook);
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
                    DbContext.books.Remove(book); //remove book
                    await DbContext.SaveChangesAsync(); //save changes to db
                    return true; //removed successfully
                }
                else return false; //failed to remove book
            }
            catch 
            {
                return false;
            }
        }

        private async Task<List<Book?>> RefactoredFindItems(string? author, string? publisher, int? year) //Add improved FindItems method here
        { 
            return null;
        }

    }
}
