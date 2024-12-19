using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Library_system;

class Program
{
    static void Main()
    {
        string _input = "";
        string[] _books;
        string _filePath = "books.txt";
        try
        {
            _books = File.ReadAllLines(_filePath);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Файл с книгами не найден. Будет создан новый файл.");
            File.Create(_filePath).Close();
            _books = new string[] { };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return;
        }
        _books = File.ReadAllLines(_filePath);
        Lib _library = new Lib(_filePath);
        Console.WriteLine("Добро пожаловать в систему управления библиотекой!");
        while (true)
        {
            Console.WriteLine("\nВведите номер необходимого действия:\n1. Добавление книги\n2. Поиск книги\n3. Удаление книги\n4. Просмотр списка книг\n5. Завершение работы");
            _input = Console.ReadLine();
            if (int.TryParse(_input, out int number))
            {
                Console.WriteLine("Загрузка действия...");
            }
            else
            {
                Console.WriteLine("Ошибка: введите номер действия");
                continue;
            }

            try
            {
                if (_input == "1")
                {
                    Console.WriteLine("Введите данные книги для занесения в библиотеку...");
                    string _stroka = Console.ReadLine();
                    string[] _data = _stroka.Split(' ');
                    if (_data.Length != 4)
                    {
                        Console.WriteLine("Ошибка: Некорректный формат ввода. Ожидается: ID Год Название Автор");
                        continue;
                    }
                    int _id = Convert.ToInt32(_data[0]);
                    int _date = Convert.ToInt32(_data[1]);
                    string _title = _data[2];
                    string _author = _data[3];
                    _library.Add_to_lib(new Book(_title, _author, _date, _id));
                }
                else if (_input == "2")
                {
                    Console.WriteLine("Введите название или автора книги...");
                    string _stroka = Console.ReadLine();
                    Task.Run(() => _library.Search_in_lib(_stroka));
                }
                else if (_input == "3")
                {
                    Console.WriteLine("Введите Id книги для удаления...");
                    string _stroka = Console.ReadLine();
                    _library.Delete_from_lib(_stroka);
                }
                else if (_input == "4")
                {
                    _library.List_of_lib();
                }
                else if (_input == "5")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Ошибка: Неверный номер действия.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Ошибка формата данных: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}

class Lib
{
    private List<Book> _books = new List<Book>();
    private string _filePath;
    public Lib(string _filePath)
    {
        this._filePath = _filePath;
        Load_Books_From_File();
    }
    public void Add_to_lib(Book _book)
    {
        _books.Add(_book);
        Save_Books_To_File();
        Console.WriteLine("Книга успешно добавлена!");
    }
    public void List_of_lib()
    {
        if (_books.Count == 0)
        {
            Console.WriteLine("Библиотека пуста.");
        }
        else
        {
            Console.WriteLine("Список книг в библиотеке:");
            _books.ForEach(book => Console.WriteLine($"ID: {book.Id}, Название: {book.Title}, Автор: {book.Author}, Год издания: {book.Year}"));
        }
    }
    
    public void Search_in_lib(string _stroka)
    {
        foreach (var book in _books)
        {
            if (book.Title == _stroka || book.Author == _stroka)
            {
                Console.WriteLine("Результат поиска:");
                Console.WriteLine($"ID: {book.Id}, Название: {book.Title}, Автор: {book.Author}, Год издания: {book.Year}");
            }
        }
    }
    public void Delete_from_lib(string _stroka)
    {
        try
        {
            int id = Convert.ToInt32(_stroka);
            var _removebook = _books.Find(_book => _book.Id == id);
            if (_removebook != null)
            {
                _books.Remove(_removebook);
                Save_Books_To_File();
                Console.WriteLine("Книга удалена.");
            }
            else
            {
                Console.WriteLine("Указанная книга не найдена.");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка: ID книги должен быть числом.");
        }
    }
    private void Save_Books_To_File()
    {
        File.WriteAllLines(_filePath, _books.ConvertAll(_book => _book.ToString()));
    }
    private void Load_Books_From_File()
    {
        if (File.Exists(_filePath))
        {
            string[] _lines = File.ReadAllLines(_filePath);
            foreach (var _line in _lines)
            {
                try
                {
                    _books.Add(Book.FromString(_line));
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Ошибка загрузки книги: {ex.Message}");
                }
            }
        }
    }
}
class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public int Id { get; set; }
    public Book(string _title, string _author, int _year, int _id)
    {
        Title = _title;
        Author = _author;
        Year = _year;
        Id = _id;
    }
    public override string ToString()
    {
        return $"{Id} {Title} {Author} {Year}";
    }
    public static Book FromString(string data)
    {
        var _parts = data.Split(' ');
        if (_parts.Length == 4)
        {
            return new Book(_parts[1], _parts[2], Convert.ToInt32(_parts[3]), Convert.ToInt32(_parts[0]));
        }
        throw new FormatException("Некорректный формат данных книги.");
    }
}