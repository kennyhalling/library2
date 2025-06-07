# Overview
This is the second version of a demo program for an application that could theoretically be used by visitors at a library to easily navigate the books offered there. This program provides a way for users to easily view, search for, check out, and return books belonging to a library.

I wrote this software to test my skills with C# and with MySQL, and to refresh myself on the things that I've learned in the language over the years. The main thing I wanted to test here was my ability to get C# to interact with an outside database in MySQL.

[Software Demo Video](https://youtu.be/JCMGpZwr45s)

# Development Environment

To develop this software, I used VS Code Studio and MySQL Workbench on my Windows 11 laptop.

I wrote the software in the object-oriented language C# and used the Newtonsoft.Json library to serialize and deserialize Json in my program. The book data itself comes from the Open Library API that I fetched prior to writing the program. Within the program I build MySQL logic that interacts with the database in MySQL workbench.

# Useful Websites

{Make a list of websites that you found helpful in this project}

- [W3 Schools MySQL Documentation](https://www.w3schools.com/MySQL/default.asp)
- [C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)

# Future Work

{Make a list of things that you need to fix, improve, and add in the future.}

- I want to expand the search function to be able to search by anything, and to add more details to books such as "publisher".
- I want to add security measures to make sure only an admin (or "librarian") can add and delete books from the database.
- I want to make a GUI that is more sleek and user friendly than a terminal.