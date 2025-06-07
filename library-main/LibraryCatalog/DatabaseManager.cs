using MySql.Data.MySqlClient;

public class DatabaseManager
{
    private readonly string connectionString;

    public DatabaseManager(string server, string database, string user, string password)
    {
        connectionString = $"Server={server};Database={database};Uid={user};Pwd={password};";
    }

    public bool InsertBook(Book book)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "INSERT INTO books (serial, title, author, language, available) VALUES (@serial, @title, @author, @language, @available)";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serial", book.SerialNumber);
        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@author", book.Author);
        cmd.Parameters.AddWithValue("@language", book.Language);
        cmd.Parameters.AddWithValue("@available", book.IsAvailable);

        try
        {
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error inserting book: " + ex.Message);
            return false;
        }
    }

    public List<Book> SearchBooks(string keyword)
    {
        var results = new List<Book>();
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "SELECT * FROM books WHERE title LIKE @kw OR author LIKE @kw";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new Book
            {
                SerialNumber = reader["serial"].ToString() ?? "",
                Title = reader["title"].ToString() ?? "",
                Author = reader["author"].ToString() ?? "",
                Language = reader["language"].ToString() ?? "",
                IsAvailable = Convert.ToBoolean(reader["available"])
            });
        }
        return results;
    }

    public void DeleteBook(string serial)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "DELETE FROM books WHERE serial = @serial";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serial", serial);

        int rowsAffected = cmd.ExecuteNonQuery();
        Console.WriteLine(rowsAffected > 0
        ? $"Book [{serial}] deleted."
        : "No matching book found to delete");
    }

    public List<Book> GetAllBooks()
    {
        List<Book> books = new List<Book>();

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "SELECT serial, title, author, language, available FROM books";
        using var cmd = new MySqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            books.Add(new Book
            {
                SerialNumber = reader.GetString("serial"),
                Title = reader.GetString("title"),
                Author = reader.GetString("author"),
                Language = reader.GetString("language"),
                IsAvailable = reader.GetBoolean("available")
            });
        }
        return books;
    }

    public int GetOrCreateUserId(string name, string phone)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string selectQuery = "SELECT id FROM users WHERE phone = @phone";
        using var selectCmd = new MySqlCommand(selectQuery, connection);
        selectCmd.Parameters.AddWithValue("@phone", phone);

        var result = selectCmd.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt32(result);
        }

        string insertQuery = "INSERT INTO users (name, phone) VALUES (@name, @phone)";
        using var insertCmd = new MySqlCommand(insertQuery, connection);
        insertCmd.Parameters.AddWithValue("@name", name);
        insertCmd.Parameters.AddWithValue("@phone", phone);
        insertCmd.ExecuteNonQuery();

        return (int)insertCmd.LastInsertedId;
    }

    public int CreateCheckout(int userId, DateTime dueDate)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "INSERT INTO checkouts (user_id, due_date) VALUE (@userId, @dueDate)";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@userID", userId);
        cmd.Parameters.AddWithValue("@dueDate", dueDate);
        cmd.ExecuteNonQuery();

        return (int)cmd.LastInsertedId;
    }

    public void AddBookToCheckout(int checkoutId, string serial)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string insertItem = "INSERT INTO checkout_items (checkout_id, book_serial) VALUES (@checkoutId, @serial)";
        using var cmd = new MySqlCommand(insertItem, connection);
        cmd.Parameters.AddWithValue("@checkoutId", checkoutId);
        cmd.Parameters.AddWithValue("@serial", serial);
        cmd.ExecuteNonQuery();

        string updateBook = "UPDATE books SET available = false WHERE serial = @serial";
        using var updateCmd = new MySqlCommand(updateBook, connection);
        updateCmd.Parameters.AddWithValue("@serial", serial);
        updateCmd.ExecuteNonQuery();
    }

    public Book? GetBookBySerial(string serial)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "SELECT serial, title, author, language, available FROM books WHERE serial = @serial";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@serial", serial);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                SerialNumber = reader.GetString("serial"),
                Title = reader.GetString("title"),
                Author = reader.GetString("author"),
                Language = reader.GetString("language"),
                IsAvailable = reader.GetBoolean("available")
            };
        }
        return null;
    }

    public int GetUserId(string name, string phone)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = "SELECT id FROM users WHERE name = @name AND phone = @phone";
        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@phone", phone);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : -1;
    }

    public List<Book> GetBooksCheckedOutByUser(int userId)
    {
        var books = new List<Book>();

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string query = @"
            SELECT b.serial, b.title, b.author, b.language, b.available
            FROM books b
            JOIN checkout_items ci ON b.serial = ci.book_serial
            JOIN checkouts co ON ci.checkout_id = co.id
            WHERE co.user_id = @userId";

        using var cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@userId", userId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            books.Add(new Book
            {
                SerialNumber = reader.GetString("serial"),
                Title = reader.GetString("title"),
                Author = reader.GetString("author"),
                Language = reader.GetString("language"),
                IsAvailable = reader.GetBoolean("available")
            });
        }

        return books;
    }

    public void ReturnBooks(int userId)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        string getCheckouts = "SELECT id FROM checkouts WHERE user_id = @userId";
        using var getCmd = new MySqlCommand(getCheckouts, connection);
        getCmd.Parameters.AddWithValue("@userId", userId);

        var checkoutIds = new List<int>();
        using var reader = getCmd.ExecuteReader();
        while (reader.Read())
        {
            checkoutIds.Add(reader.GetInt32("id"));
        }
        reader.Close();

        foreach (int checkoutId in checkoutIds)
        {
            string getBooks = "SELECT book_serial FROM checkout_items WHERE checkout_id = @checkoutId";
            using var bookCmd = new MySqlCommand(getBooks, connection);
            bookCmd.Parameters.AddWithValue("@checkoutId", checkoutId);

            var serials = new List<string>();
            using var bookReader = bookCmd.ExecuteReader();
            while (bookReader.Read())
            {
                serials.Add(bookReader.GetString("book_serial"));
            }
            bookReader.Close();
            foreach (string serial in serials)
            {
                string update = "UPDATE books SET available = true WHERE serial = @serial";
                using var updateCmd = new MySqlCommand(update, connection);
                updateCmd.Parameters.AddWithValue("@serial", serial);
                updateCmd.ExecuteNonQuery();
            }

            string deleteItems = "DELETE FROM checkout_items WHERE checkout_id = @checkoutId";
            using var delItemsCmd = new MySqlCommand(deleteItems, connection);
            delItemsCmd.Parameters.AddWithValue("@checkoutId", checkoutId);
            delItemsCmd.ExecuteNonQuery();

            string deleteCheckout = "DELETE FROM checkouts WHERE id = @checkoutId";
            using var delCoCmd = new MySqlCommand(deleteCheckout, connection);
            delCoCmd.Parameters.AddWithValue("@checkoutId", checkoutId);
            delCoCmd.ExecuteNonQuery();
        }
    }
}