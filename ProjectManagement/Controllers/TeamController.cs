using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Models.Dtos.Team;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamController(DataContext context)
        {
            _context = context;
        }

        #region CRUD API's
        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Team>>> GetAllTeams()
        {
            var teams = await _context.Teams.Include(t => t.Users).Select(t => new
            {
                t.Id,
                t.Name,
                t.Description,
                t.Username,
                t.OwnerId
            }).ToListAsync();

            if (!teams.Any())
                return NotFound("تیمی یافت نشد");

            return Ok(teams);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Team>> GetTeamById(int Id)
        {
            var teamDb = await _context.Teams.FindAsync(Id);
            if (teamDb == null)
                return NotFound("تیمی یافت نشد");

            return Ok(teamDb);
        }

        [HttpPost("CreateTeam")]
        public async Task<ActionResult<Team>> CreateTeam([FromForm] CreateTeamDto teamDto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var teamDb = await _context.Teams.Where(t => t.Username == teamDto.Username).FirstOrDefaultAsync();
            if (teamDb != null)
                return BadRequest("نام کاربری وارد شده تکراری است");

            var team = new Team
            {
                Name = teamDto.Name,
                Description = string.IsNullOrEmpty(teamDto.Description) ? string.Empty : teamDto.Description,
                Username = teamDto.Username,
                OwnerId = user.Id
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return Ok(team);
        }

        [HttpPut("EditTeam/{Id}")]
        public async Task<ActionResult<Team>> EditTeam(int Id, [FromForm] EditTeamDto teamDto)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var teamDb = await _context.Teams.Where(t => t.Id == Id && t.OwnerId == user.Id).FirstOrDefaultAsync();
            if (teamDb == null)
                return NotFound("تیمی یافت نشد");

            teamDb.Name = string.IsNullOrEmpty(teamDto.Name) ? teamDb.Name : teamDto.Name;
            teamDb.Description = string.IsNullOrEmpty(teamDto.Description) ? teamDb.Description : teamDto.Description;

            await _context.SaveChangesAsync();

            return Ok(teamDb);
        }

        [HttpDelete("ReamoveTeam/{Id}")]
        public async Task<ActionResult<string>> RemoveTeamById(int Id)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();

            var teamDb = await _context.Teams.Where(t => t.Id == Id && t.OwnerId == user.Id).FirstOrDefaultAsync();
            if (teamDb == null)
                return NotFound("تیمی یافت نشد");

            _context.Remove(teamDb);
            await _context.SaveChangesAsync();

            return Ok("تیم باموفقیت حذف شد");
        }
        #endregion

        [HttpGet("UsersOfTeam/{Id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<User>>> GetUsersOfTeam(int Id)
        {
            var team = await _context.Teams.FindAsync(Id);
            if (team == null)
                return NotFound("تیمی یافت نشد");

            var users = team.Users.ToList();
            if (!users.Any())
                return BadRequest("کاربری یافت نشد");
            
            return Ok(users);
        }

        [HttpPost("invite/teamId/username")]
        public async Task<ActionResult<string>> InviteUserToTeam(int teamId, string username)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.Where(u => u.Username == userName).FirstOrDefaultAsync();

            var team = await _context.Teams.Where(t => t.Id == teamId && t.OwnerId == user.Id || t.Users.Contains(user)).FirstOrDefaultAsync();
            if (team == null)
                return NotFound("تیمی جهت دعوت کاربر یافت نشد");
            var userDb = await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
            if (userDb == null)
                return NotFound("کاربر مورد نظر جهت دعوت به تیم یافت نشد");
            var userInvited = await _context.Teams.Where(t => t.Id == teamId && t.OwnerId == userDb.Id || t.Users.Contains(userDb)).FirstOrDefaultAsync();
            if (userInvited != null)
                return BadRequest("کاربر مورد نظر قبلا به تیم دعوت شده است");

            var request = new InviteRequest
            {
                teamId = teamId,
                Caller = userName,
                RequestDate = DateTime.Now,
                Status = "Pending",
                UserId = userDb.Id
            };

            _context.InviteRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok("درخواست دعوت به تیم برای کاربر ارسال شد");
        }
    }
}
