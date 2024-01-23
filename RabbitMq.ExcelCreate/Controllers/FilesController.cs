using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreate.Models;

namespace RabbitMq.ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public FilesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId )
        {
            if(file is not { Length: > 0})
            {
                return BadRequest();
            }

            var userFile = await _appDbContext.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId);

            var filePath = userFile.FileName + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);//path of the file to be uploaded

            using var stream = new FileStream(path, FileMode.Create); 

            await file.CopyToAsync(stream); //copy the file to the path

            userFile.Status = FileStatus.Completed;
            userFile.FilePath = filePath;
            userFile.CreatedAt = DateTime.Now;
            await _appDbContext.SaveChangesAsync();

            //todo: with SignalR, we can send a message to the client side when the file upload is complete.
            return Ok();

        }
    }
}
