using Newtonsoft.Json;
using SingleResponsibilityPrinciple.Model;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(" List of Books");

        Console.WriteLine(" -----");

        var cadJSON = File.ReadAllText("Data/BookStore.json");

        var bookList = JsonConvert.DeserializeObject<Book[]>(cadJSON);

        foreach (var item in bookList)
        {
            Console.WriteLine($" {item.Title.PadRight(39, ' ')} " +
                              $"{item.Author.PadRight(15, ' ')} {item.Price}");
        }

        Console.Read();
    } //end Main
}