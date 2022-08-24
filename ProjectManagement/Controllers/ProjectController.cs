using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Models.Dtos.Project;
using System.Security.Claims;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProjectController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("List")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Project>>> GetAllProject()
        {
            var projects = await _context.Projects.ToListAsync();
            if(!projects.Any())
                return NotFound("پروژه ای یافت نشد");

            return Ok(projects);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Project>> GetProjectById(int Id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var project = await _context.Projects.Where(p => p.Id == Id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (project == null)
                return NotFound("پروژه ای یافت نشد");

            return Ok(project);
        }

        [HttpPost("CreateProject")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Project>> CreateProject([FromForm] CreateProjectDto projectDto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            string filename = "";
            string extension = "";
            if (projectDto?.File?.Length > 0)
            {
                var webRootPath = _hostEnvironment.ContentRootPath;
                filename = Guid.NewGuid().ToString();
                extension = Path.GetExtension(projectDto.File.FileName);
                var upload = Path.Combine(webRootPath, @"Public\Images\");

                using var fileStream = new FileStream(Path.Combine(upload, filename + extension), FileMode.Create);
                projectDto?.File?.CopyTo(fileStream);
            }

            var project = new Project
            {
                Title = projectDto.Title,
                Description = projectDto.Description != null ? projectDto.Description : string.Empty,
                Image = projectDto?.File?.Length > 0 ? Path.Combine(@"Public\Images\", filename + extension) : string.Empty,
                IsPrivate = projectDto.IsPrivate,
                User = user
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpPut("EditProject/{Id}")]
        public async Task<ActionResult<Project>> EditProject([FromForm] EditProjectDto projectDto, int Id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var project = await _context.Projects.Where(p => p.Id == Id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (project == null)
                return NotFound("پروژه ای یافت نشد");

            project.Title = string.IsNullOrEmpty(projectDto.Title) ? project.Title : projectDto.Title;
            project.Description = string.IsNullOrEmpty(projectDto.Description) ? project.Description : projectDto.Description;
            project.IsPrivate = string.IsNullOrEmpty(projectDto.IsPrivate.ToString()) ? project.IsPrivate : projectDto.IsPrivate;

            if(projectDto?.File?.Length > 0)
            {
                var webRootPath = _hostEnvironment.ContentRootPath;
                var filename = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(projectDto.File.FileName);
                var upload = Path.Combine(webRootPath, @"Public\Images\");
                var imagePath = Path.Combine(webRootPath, project.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using var fileStream = new FileStream(Path.Combine(upload, filename + extension), FileMode.Create);
                projectDto?.File?.CopyTo(fileStream);
                project.Image = Path.Combine(@"Public\Images\", filename + extension);
            }

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        [HttpDelete("RemoveProject/{Id}")]
        public async Task<ActionResult<string>> RemoveProject(int Id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var project = await _context.Projects.Where(p => p.Id == Id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (project == null)
                return NotFound("پروژه ای یافت نشد");

            var webRootPath = _hostEnvironment.ContentRootPath;
            var imagePath = Path.Combine(webRootPath, project.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok("پروژه باموفقیت حذف شد");
        }

        //eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9
        //.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibW1Ib3NzZWluaSIsImV4cCI6MTY2MTI1MjAxOH0
        //.YZcu5hS2sGaCn9EI81AYs3Q38Y0ymK1fB9M0wNfLh807uXTtsXVlvXgv6eE1ZbN2nfhUp6rzxlTLSqaKzNWucg
    }
}
