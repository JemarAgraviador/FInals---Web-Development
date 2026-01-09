using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using LibrarySystem.Models;
using LibrarySystem.Data;

namespace LibrarySystem.Controllers
{
    [ApiController]
    [Route("api/library")]
    public class LibraryController : ControllerBase
    {
        private readonly Db _db = new Db();

        [HttpGet("books")]
        public IActionResult GetBooks()
        {
            var books = new List<Book>();
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM Books WHERE IsAvailable=TRUE", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    BookId = reader.GetInt32("BookId"),
                    Code = reader.GetString("Code"),
                    Title = reader.GetString("Title"),
                    Author = reader.GetString("Author")
                });
            }
            return Ok(books);
        }

        [HttpGet("borrowed")]
        public IActionResult GetBorrowedBooks()
        {
            var borrowed = new List<Book>();
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM Books WHERE IsAvailable=FALSE", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                borrowed.Add(new Book
                {
                    BookId = reader.GetInt32("BookId"),
                    Code = reader.GetString("Code"),
                    Title = reader.GetString("Title"),
                    Author = reader.GetString("Author")
                });
            }
            return Ok(borrowed);
        }

        [HttpPost("add")]
        public IActionResult AddBook([FromBody] Book book)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            string code = "B" + DateTime.Now.Ticks.ToString().Substring(10);
            var cmd = new MySqlCommand(
                "INSERT INTO Books (Code, Title, Author) VALUES (@code,@title,@author)", conn);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@title", book.Title);
            cmd.Parameters.AddWithValue("@author", book.Author);
            cmd.ExecuteNonQuery();
            return Ok($"Book added with code {code}");
        }

        [HttpPost("borrow")]
        public IActionResult BorrowBook([FromBody] string code)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM Books WHERE Code=@code AND IsAvailable=TRUE", conn);
            cmd.Parameters.AddWithValue("@code", code);
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return BadRequest("Book not found or already borrowed");
            int bookId = reader.GetInt32("BookId");
            string title = reader.GetString("Title");
            string author = reader.GetString("Author");
            reader.Close();
            cmd = new MySqlCommand("UPDATE Books SET IsAvailable=FALSE WHERE BookId=@bookId", conn);
            cmd.Parameters.AddWithValue("@bookId", bookId);
            cmd.ExecuteNonQuery();
            return Ok($"Borrowed {title} by {author}");
        }

        [HttpPost("return")]
        public IActionResult ReturnBook([FromBody] string code)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM Books WHERE Code=@code AND IsAvailable=FALSE", conn);
            cmd.Parameters.AddWithValue("@code", code);
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return BadRequest("Book not found in borrowed records");
            int bookId = reader.GetInt32("BookId");
            string title = reader.GetString("Title");
            string author = reader.GetString("Author");
            reader.Close();
            cmd = new MySqlCommand("UPDATE Books SET IsAvailable=TRUE WHERE BookId=@bookId", conn);
            cmd.Parameters.AddWithValue("@bookId", bookId);
            cmd.ExecuteNonQuery();
            return Ok($"Returned {title} by {author}");
        }
    }
}
