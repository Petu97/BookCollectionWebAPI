using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WebApplication1.Models;

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
            try //attempts to find object, if fails returns null
            {
                Book? result = await DbContext.books.Where(b => b.Id == id).FirstAsync();
                return result;
            }
            catch
            {
                return null; //refactoring idea: retrun empty instead of null, using null as retunr type is probably not the best idea
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
        //hate this method because it makes a query request for each parameter and then compares items (heavy on sql request side which slows the application down). Definately a lot to improve here.
        public async Task<IEnumerable<Book>> RefactoredFindItems(string? author, string? publisher, int? year) //Add improved FindItems method here
        {
            List<Book> authorList = new List<Book>();
            List<Book> publisherList = new List<Book>();
            List<Book> yearList = new List<Book>();

            if (author is not null) //checks if author value is given, if it is then searches for books from same author. If no author param given skips this step (authorlist stays empty)
            { 
                authorList = await DbContext.books.Where(b => b.Author == author).ToListAsync();
                if (authorList.Count() is 0) //failed to find books by author, return empty list
                    return authorList;
            }

            if (publisher is not null) 
            {
                publisherList = await DbContext.books.Where(b => b.Publisher == publisher).ToListAsync();
                if (publisherList.Count() is 0)
                    return publisherList;
            }

            if (year is not null)
            {
                yearList = await DbContext.books.Where(b => b.Year == year).ToListAsync();
                if (yearList.Count() is 0)
                    return yearList;
            }

            var resultList = Intersect(authorList, publisherList, yearList); //finds identical books from lists
            return resultList;
        }

        private static IEnumerable<T> Intersect<T>(params IEnumerable<T>[] lists) //method comparing lists: ignores empty lists and finds same books in lists which it then returns 
        {
            return lists.Where(l => l.Any()).Aggregate((l1, l2) => l1.Intersect(l2));
        }
    }
}
