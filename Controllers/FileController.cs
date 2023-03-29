using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantAPI.Controllers;

[Authorize]
[ApiController]
[Route("file")]
public class FileController : Controller
{
    [HttpGet]
    [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
    public ActionResult GetFile([FromQuery] string fileName)
    {
        var rootPath = Directory.GetCurrentDirectory();

        var file = $"{rootPath}/PrivateFiles/{fileName}";

        var fileExits = System.IO.File.Exists(file);

        if (!fileExits)
        {
            return NotFound();
        }

        var fileContent = System.IO.File.ReadAllBytes(file);

        var contentTypeProvider = new FileExtensionContentTypeProvider();

        contentTypeProvider.TryGetContentType(file, out string fileType);

        return File(fileContent, fileType, fileName);
    }

    [HttpPost]
    public ActionResult Upload([FromForm] IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var fileName = file.FileName;
            var fullPath = $"{rootPath}/PrivateFiles/{fileName}";
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok();
        }

        return BadRequest();
    }
}