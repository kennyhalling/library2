public class Cart
{
    public List<Book> SelectedBooks { get; private set; } = new List<Book>();

    public void AddBook(Book book)
    {
        if (book.IsAvailable)
        {
            SelectedBooks.Add(book);
            book.IsAvailable = false;
        }
    }

    public void Clear()
    {
        SelectedBooks.Clear();
    }

    public override string ToString()
    {
        return string.Join("\n", SelectedBooks.Select(b => b.ToString()));
    }
}