class Program
{
    public static void Main()
    {
        // Tạo dãy số từ 1 đến 1 triệu
        var range = Enumerable.Range(1, 1000_000);

        // Cách 1: Xử lý tuần tự (Sequential) - chạy trên 1 thread
        // Lọc các số chia hết cho 3
        var resultList = range.Where(i => i % 3 == 0).ToList();
        Console.WriteLine($"Sequential: Total items are {resultList.Count}\n");
        
        // Cách 2: Xử lý song song (Parallel) - chia nhỏ ra nhiều thread
        // Dùng AsParallel() để chuyển sang PLINQ (Parallel LINQ)
        resultList = range.AsParallel().Where(i => i % 3 == 0).ToList();
        Console.WriteLine($"Parallel: Total items are {resultList.Count}\n");
        
        // Cách 3: Xử lý song song dùng cú pháp query syntax
        // Kết quả giống cách 2 nhưng viết theo kiểu SQL-like
        resultList = (from i in range.AsParallel()
            where i % 3 == 0
            select i).ToList();
        Console.WriteLine($"Parallel: Total items are {resultList.Count}\n");
        Console.ReadLine();
    } //end Main
} //end Program