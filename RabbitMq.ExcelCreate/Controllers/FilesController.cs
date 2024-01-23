using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreate.Hubs;
using RabbitMq.ExcelCreate.Models;

namespace RabbitMq.ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<ExcelHub> _hubContext;

        public FilesController(AppDbContext appDbContext, IHubContext<ExcelHub> hubContext)
        {
            _appDbContext = appDbContext;
            _hubContext = hubContext;
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

            await _hubContext.Clients.User(userFile.UserId).SendAsync("FileCreated"); //send a message to the client that the file has been created. Also, we can send a object to the client.

            return Ok();

        }
    }
}
