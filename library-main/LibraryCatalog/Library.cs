using System;
using System.Net.Http;
using Newtonsoft.Json;

public class Library
{

    private List<Book> books = new List<Book>();
    private List<Checkout> checkouts = new List<Checkout>();
    private LibraryAddress address = new LibraryAddress("123 Main St", "Rexburg", "83440");

    public void LoadBooksFromFile()
    {
        try
        {
            Directory.CreateDirectory("data");
            if (File.Exists("data/books.json"))
            {
                string json = File.ReadAllText("data/books.json");
                books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
            }
            else
            {
                Console.WriteLine("Book file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading books: " + ex.Message);
        }
    }

    public void LoadCheckoutsFromFile()
    {
        try
        {
            string path = "data/checkouts.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                checkouts = JsonConvert.DeserializeObject<List<Checkout>>(json) ?? new List<Checkout>();
                foreach (var checkout in checkouts)
                {
                    foreach (var book in checkout.Books)
                    {
                        Book? match = books.FirstOrDefault(b => b.SerialNumber == book.SerialNumber);
                        if (match != null)
                        {
                            match.IsAvailable = false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading checkouts: " + ex.Message);
        }
    }

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
        LoadBooksFromFile();
        LoadCheckoutsFromFile();
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

        if (books.Count == 0)
        {
            Console.WriteLine("No books currently in catalog");
        }
        else
        {
            foreach (Book book in books)
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
        string query = Console.ReadLine() ?? "";

        var matches = books.FindAll(book =>
        book.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        book.Author.Contains(query, StringComparison.OrdinalIgnoreCase)
        );

        Console.WriteLine();

        if (matches.Count == 0)
        {
            Console.WriteLine("No books matched your search");
        }
        else
        {
            Console.WriteLine($"Found {matches.Count} book(s):\n");
            foreach (Book book in matches)
            {
                Console.WriteLine(book);
            }
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

            Book book = books.FirstOrDefault(b => b.SerialNumber == input)!;
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

        User user = new User(name, phone);
        Checkout checkout = new Checkout(user, cart.SelectedBooks.ToList());

        LoadCheckoutsFromFile();
        checkouts.Add(checkout);
        cart.Clear();

        Console.WriteLine("Checkout Complete: ");
        Console.WriteLine(checkout);
        Pause();

        SaveCheckoutsToFile();
        SaveBooksToFile();
    }

    public void ReturnBooks()
    {
        Console.WriteLine("===== Return Books =====");

        Console.WriteLine("DEBUG: Known users:");
        foreach (var c in checkouts)
        {
            Console.WriteLine($"- {c.User.Name} / {c.User.PhoneNumber}");
        }

        Console.Write("Enter your name: ");
        string name = (Console.ReadLine() ?? "").Trim();

        Console.Write("Enter your phone number: ");
        string phone = (Console.ReadLine() ?? "").Trim();

        var matchingCheckouts = checkouts.Where(c =>
            string.Equals(c.User.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase) &&
            c.User.PhoneNumber.Trim() == phone.Trim()
        ).ToList();

        if (matchingCheckouts.Count == 0)
        {
            Console.WriteLine("No matching checkouts found.");
            Pause();
            return;
        }

        foreach (var checkout in matchingCheckouts)
        {
            foreach (var book in checkout.Books)
            {
                Book? match = books.FirstOrDefault(b => b.SerialNumber == book.SerialNumber);
                if (match != null)
                {
                    match.IsAvailable = true;
                }
            }

            Console.WriteLine($"Returned {checkout.Books.Count} book(s) for {checkout.User.Name}.");
            checkouts.Remove(checkout);
        }

        Pause();

        SaveCheckoutsToFile();
        SaveBooksToFile();
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

    public void SaveCheckoutsToFile()
    {
        try
        {
            Directory.CreateDirectory("data");
            string json = JsonConvert.SerializeObject(checkouts, Formatting.Indented);
            File.WriteAllText("data/checkouts.json", json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving checkouts:" + ex.Message);
        }
    }
    
    public void SaveBooksToFile()
    {
        try
        {
            Directory.CreateDirectory("data");
            string json = JsonConvert.SerializeObject(books, Formatting.Indented);
            File.WriteAllText("data/books.json", json);
            Console.WriteLine("Books saved.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving books: " + ex.Message);
        }
    }
}