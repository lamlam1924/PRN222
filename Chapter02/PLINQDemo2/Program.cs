using System.Collections.Concurrent;
using System.Diagnostics;

class Program
{
    // Kiểm tra số nguyên tố
    private static bool IsPrime(int number)
    {
        bool result = true;
        if (number < 2) return false;

        // Thử chia từ 2 đến căn bậc 2 của number
        for (var i = 2; i <= Math.Sqrt(number) && result == true; i++)
        {
            if (number % i == 0) result = false;
        }
        return result;
    } 

    // Cách 1: Tuần tự - chạy trên 1 thread
    private static IList<int> GetPrimeList(IList<int> numbers) => numbers.Where(IsPrime).ToList();

    // Cách 2: Song song - chia nhỏ ra nhiều thread
    private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
    {
        // ConcurrentBag: List thread-safe cho nhiều thread cùng Add
        var primeNumbers = new ConcurrentBag<int>();

        // Parallel.ForEach - tự động chia ra nhiều thread chạy đồng thơi
        Parallel.ForEach(numbers, number =>
        {
            if (IsPrime(number)) primeNumbers.Add(number);
            
        });
        return primeNumbers.ToList();
    } 

    static void Main()
    {
        // Tạo danh sách 2 triệu số
        // var limit = 2_000_000;
        var limit = 100_000;
        var numbers = Enumerable.Range(0, limit).ToList();
        
        // Đo thời gian cách 1
        var watch = Stopwatch.StartNew();
        var primeNumbersFromForeach = GetPrimeList(numbers);
        watch.Stop();

        // Đo thời gian cách 2
        var watchForParallel = Stopwatch.StartNew();
        var primeNumbersFromParallelForeach = GetPrimeListWithParallel(numbers);
        watchForParallel.Stop();

        // So sánh kết quả
        Console.WriteLine($"Classical foreach loop | Total prime numbers :" +
                          $" {primeNumbersFromForeach.Count} | Time Taken: " +
                          $"{watch.ElapsedMilliseconds} ms.\n");
        Console.WriteLine($"Parallel. ForEach loop | Total prime numbers : " +
                          $"{primeNumbersFromParallelForeach.Count} | Time Taken:" +
                          $"{watchForParallel.ElapsedMilliseconds} ms.\n");

        Console.WriteLine("Press any key to exit.");
        Console.ReadLine();
    } //end Main
} 