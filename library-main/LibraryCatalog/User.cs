public class User
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }

    public User(string name, string phoneNumber)
    {
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public override string ToString()
    {
        return $"{Name} ({PhoneNumber})";
    }
}