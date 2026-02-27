using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoRazor1.Pages;

public class UploadFilesModel : PageModel
{
    private readonly IWebHostEnvironment _env;
    public UploadFilesModel(IWebHostEnvironment env) => _env = env;
    [BindProperty] public List<IFormFile> FileUploads { get; set; } = new();

    public string? Message { get; set; }


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (FileUploads == null || FileUploads.Count == 0)
        {
            Message = "Please choose at least one file.";
            return Page();
        }

        var imagesPath = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(imagesPath);

        foreach (var file in FileUploads)
        {
            if (file.Length == 0) continue;

            // demo đơn giản: giữ nguyên tên (production nên random/clean tên)
            var filePath = Path.Combine(imagesPath, Path.GetFileName(file.FileName));

            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
        }

        Message = "Upload OK!";
        return Page();
    }
}