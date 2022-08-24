using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Models.Dtos.User;
using System.IO;
using System.Security.Claims;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            if (user == null)
                return NotFound("کاربری یافت نشد");

            return Ok(user);
        }

        [HttpPut("EditProfile")]
        public async Task<ActionResult<User>> EditProfile([FromForm] EditUserDto request)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            user.PhoneNumber = request.PhoneNumber;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _context.SaveChangesAsync();

            return Ok(user);
        }
        
        [HttpPost("UploadImage")]
        public async Task<ActionResult<User>> UploadImage(IFormFile file)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            if (file.Length > 0)
            {
                var webRootPath = _hostEnvironment.ContentRootPath;
                var upload = Path.Combine(webRootPath, @"Public\Images\");
                var fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);

                if (user.ProfileImage != null)
                {
                    var imagePath = Path.Combine(webRootPath, user.ProfileImage).TrimStart('\\');
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                using var filesStreams = new FileStream(Path.Combine(upload, (fileName + extension)), FileMode.Create);
                file.CopyTo(filesStreams);

                user.ProfileImage = @"\Public\Images\" + fileName + extension;
                await _context.SaveChangesAsync();
            }

            return Ok(user);
        }

        [HttpGet("InviteRequests")]
        public async Task<ActionResult<List<InviteRequest>>> GetAllInviteRequest()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var requests = await _context.InviteRequests.Where(i => i.UserId == user.Id).ToListAsync();
            if (!requests.Any())
                return NotFound("درخواستی یافت نشد");

            return Ok(requests);
        }

        [HttpGet("InviteRequests/{status}")]
        public async Task<ActionResult<List<InviteRequest>>> GetRequestsByStatus([FromRoute] string status)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var requests = await _context.InviteRequests.Where(i => i.UserId == user.Id && i.Status == status).ToListAsync();
            if (!requests.Any())
                return NotFound("درخواستی یافت نشد");

            return Ok(requests);
        }

        [HttpPost("InviteRequests/{requestId}/{status}")]
        public async Task<ActionResult<string>> ChangeRequestStatus(int requestId, string status)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var request = await _context.InviteRequests.FindAsync(requestId);
            if (request == null)
                return NotFound("درخواستی یافت نشد");

            if (request.Status != "Pending")
                return BadRequest("");

            return Ok();
        }
    }
}
