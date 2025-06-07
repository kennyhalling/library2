using System;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        //Code to create sql database from local json populated by OpenLibrary API:
        //
        // DatabaseManager db = new DatabaseManager("localhost", "library_db", "root", "Yawadetirips20!");
        // var books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText("data/books.json"));
        // foreach (var book in books)
        // {
        //     string[] languageOptions = book.Languages.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //     if (languageOptions.Length > 0)
        //     {
        //         Random rng = new Random();
        //         string selectedLanguage = languageOptions[rng.Next(languageOptions.Length)].Trim();
        //         book.Language = selectedLanguage;
        //     }
        //     else
        //     {
        //         book.Language = "unknown";
        //     }

        //     db.InsertBook(book);
        // }
        Library library = new Library();
        library.Run();
    }
}