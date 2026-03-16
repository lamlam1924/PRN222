using Demo_IoC_Pattern.Model;

namespace Demo_IoC_Pattern;

public class ReaderFactory
{
    public IMovieReader _IMovieReader { get; }

    public ReaderFactory(string fileType)
    {
        switch (fileType)
        {
            case "XML":

                _IMovieReader = new XMLMovieReader();

                break;

            case "JSON":

                _IMovieReader = new JSONMovieReader();

                break;

            default: break;
        }
    }

//end ReaderFactory

    class Program
    {
        static IMovieReader IMovieReader;

        static void Main(string[] args)
        {
            Console.Title = "IoC Pattern";

            Console.WriteLine("Please, select the file type to read (1) XML, (2) JSON: ");

            var ans = Console.ReadLine();

            var fileType = (ans == "1") ? "XML" : "JSON";

            IMovieReader = new ReaderFactory(fileType)._IMovieReader;

            var typeSelected = IMovieReader.GetType().Name;

            var movieCollection = IMovieReader.ReadMovies();

            Console.WriteLine($"Movie Titles ({typeSelected})");

            Console.WriteLine("----");

            foreach (var movie in movieCollection)
            {
                Console.WriteLine($"{movie.ID}, {movie.Title}, " +
                                  $"{movie.OscarNominations}, {movie.OscarWins}");
            }

            Console.ReadLine();
        } //end Main
    } //end Program
}