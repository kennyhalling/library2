using System;
using System.Data.Common;
using System.Net.Http;
using Newtonsoft.Json;

public class Library
{
    private DatabaseManager db = new DatabaseManager("localhost", "library_db", "root", "Yawadetirips20!");
    private LibraryAddress address = new LibraryAddress("123 Main St", "Rexburg", "83440");

    public void Run()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("===== Welcome to the Library =====");
            Console.WriteLine("1. View All Books");
            Console.WriteLine("2. Search for a Book");
            Console.WriteLine("3. Checkout Books");
            Console.WriteLine("4. Return Books");
            Console.WriteLine("5. View Library Address");
            Console.WriteLine("6. Delete Book");
            Console.WriteLine("7. Add new Book");
            Console.WriteLine("0. Exit Menu");
            Console.WriteLine("Choose an option: ");

            string input = Console.ReadLine() ?? "";

            switch (input)
            {
                case "1":
                    Console.Clear();
                    ViewAllBooks();
                    break;
                case "2":
                    Console.Clear();
                    SearchBooks();
                    break;
                case "3":
                    Console.Clear();
                    CheckoutBooks();
                    break;
                case "4":
                    Console.Clear();
                    ReturnBooks();
                    break;
                case "5":
                    Console.Clear();
                    ShowLibraryAddress();
                    break;
                case "6":
                    Console.Clear();
                    Console.Write("Enter serial of book to delete: ");
                    string serial = Console.ReadLine() ?? "";
                    db.DeleteBook(serial);
                    break;
                case "7":
                    Console.Clear();
                    AddNewBook();
                    break;
                case "0":
                    Console.Clear();
                    running = false;
                    Console.WriteLine("Exiting menu...");
                    Pause();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid option. Please try again.");
                    Pause();
                    break;
            }
        }
    }

    private void Pause()
    {
        Console.WriteLine("\nPress any key to return to menu.");
        Console.ReadKey();
    }

    public void ViewAllBooks()
    {
        Console.Clear();
        Console.WriteLine("===== All Books =====");

        var allBooks = db.GetAllBooks();

        if (allBooks.Count == 0)
        {
            Console.WriteLine("No books currently in catalog");
        }
        else
        {
            foreach (Book book in allBooks)
            {
                Console.WriteLine(book);
            }
        }
        Pause();
    }

    public void SearchBooks()
    {
        Console.Clear();
        Console.WriteLine("===== Search Books =====");
        Console.Write("Enter a title or author to search: ");
        string keyword = Console.ReadLine() ?? "";

        var results = db.SearchBooks(keyword);

        Console.WriteLine();

        if (results.Count == 0)
        {
            Console.WriteLine("No books matched your search");
        }
        else
        {
            foreach (var book in results)
                Console.WriteLine(book);
        }
        Pause();
    }

    public void CheckoutBooks()
    {
        Cart cart = new Cart();

        while (true)
        {
            Console.Write("Enter serial number of book to add to cart (or 'done'): ");
            string input = (Console.ReadLine() ?? "").Trim();

            if (input?.ToLower() == "done") break;

            Book book = db.GetBookBySerial(input);
            if (book == null)
            {
                Console.WriteLine("Book not found.");
            }
            else if (!book.IsAvailable)
            {
                Console.WriteLine("Book currently checked out.");
            }
            else
            {
                cart.AddBook(book);
                Console.WriteLine($"Added '{book.Title}' to cart.");
            }
        }

        if (cart.SelectedBooks.Count == 0)
        {
            Console.WriteLine("No books selected for checkout.");
            Pause();
            return;
        }

        Console.Write("Enter your name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Enter your phone number: ");
        string phone = Console.ReadLine() ?? "";

        int userId = db.GetOrCreateUserId(name, phone);
        DateTime dueDate = DateTime.Now.AddDays(14);
        int checkoutId = db.CreateCheckout(userId, dueDate);

        foreach (Book book in cart.SelectedBooks)
        {
            db.AddBookToCheckout(checkoutId, book.SerialNumber);
        }

        Console.WriteLine($"Checkout complete! Books due on {dueDate.ToShortDateString()}.");
        Pause();
    }

    public void ReturnBooks()
    {
        Console.WriteLine("===== Return Books =====");

        Console.Write("Enter your name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        Console.Write("Enter your phone number: ");
        string phone = (Console.ReadLine() ?? "").Trim();

        int userId = db.GetOrCreateUserId(name, phone);
        if (userId == -1)
        {
            Console.WriteLine("User not found");
            Pause();
            return;
        }

        var booksToReturn = db.GetBooksCheckedOutByUser(userId);
        if (booksToReturn.Count == 0)
        {
            Console.WriteLine("No books found for this user.");
            Pause();
            return;
        }

        Console.WriteLine("The following books will be returned");
        foreach (var book in booksToReturn)
        {
            Console.WriteLine(book);
        }

        db.ReturnBooks(userId);

        Console.WriteLine("Books returned successfully.");
        Pause();
    }

    public struct LibraryAddress
    {
        public string Street { get; }
        public string City { get; }
        public string Zip { get; }

        public LibraryAddress(string street, string city, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {Zip}";
        }
    }

    public void ShowLibraryAddress()
    {
        Console.WriteLine("===== Library Information =====");
        Console.WriteLine($"Location: {address}");
        Pause();
    }

    public void AddNewBook()
    {
        Console.WriteLine("===== Add New Book ======");

        Console.Write("Enter serial number: ");
        string serial = Console.ReadLine() ?? "";

        Console.Write("Enter title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Enter author: ");
        string author = Console.ReadLine() ?? "";

        Console.Write("Enter language (e.g. 'eng'): ");
        string language = Console.ReadLine() ?? "";

        Book newBook = new Book
        {
            SerialNumber = serial,
            Title = title,
            Author = author,
            Language = language,
            IsAvailable = true
        };

        bool success = db.InsertBook(newBook);
        if (success)
            Console.WriteLine("Book added successfully!");
        else
            Console.WriteLine("Failed to add book (maybe duplicate serial?).");

        Pause();
    }
}