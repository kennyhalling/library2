using System;

class Program
{
    static void Main(string[] args){
        Library library = new Library();
        library.LoadBooksFromFile();
        library.LoadCheckoutsFromFile();
        library.Run();
    }
}