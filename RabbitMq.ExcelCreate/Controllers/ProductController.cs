using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreate.Models;
using RabbitMq.ExcelCreate.Services;
using Shared;

namespace RabbitMq.ExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly RabbitMqPublisher _rabbitMqPublisher;

        public ProductController(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RabbitMqPublisher rabbitMqPublisher)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.GetUserAsync(User);
            var fileName = $"product-{user.Id}-{Guid.NewGuid()}.xlsx";
            UserFile userFile = new UserFile
            {
                UserId = user.Id,
                FileName = fileName,
                CreatedAt = DateTime.Now,
                Status = FileStatus.Created
            };
            await _appDbContext.UserFiles.AddAsync(userFile);
            await _appDbContext.SaveChangesAsync();

            _rabbitMqPublisher.Publish(new CreateExcelMessage
            {
                UserFileId = userFile.Id, //Ef core will set this property to the id of the userFile object after saving it to the database.
                UserId = user.Id
            });

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.GetUserAsync(User);

            var userFiles = _appDbContext.UserFiles.ToList();
            return View(userFiles);
        }
    }
}
