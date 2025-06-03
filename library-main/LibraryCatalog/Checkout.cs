public class Checkout
{
    public User User { get; set; }
    public List<Book> Books { get; set; }
    public DateTime DueDate { get; set; }

    public Checkout(User user, List<Book> books)
    {
        User = user;
        Books = books;
        DueDate = DateTime.Now.AddDays(14);
    }

    public override string ToString()
    {
        string bookList = string.Join(", ", Books.Select(b => b.Title));
        return $"{User} checked out: {bookList} (Due: {DueDate.ToShortDateString()})";
    }
}