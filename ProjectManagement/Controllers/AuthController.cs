using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Models;
using ProjectManagement.Models.Dtos.User;
using ProjectManagement.Utility;
using ProjectManagement.Utility.Swagger.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PublicResponse>> Register(RegisterUserDto request)
        {
            var user = await _context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (user != null)
                return BadRequest("نام کاربری وارد شده تکراری است");

            PasswordHash.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new PublicResponse
            {
                StatusCode = 201,
                Success = true,
                Message = "ثبت نام شما با موفقیت انجام شد لطفا وارد حساب کاربری خود شوید"
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginSchema>> Login(LoginUserDto request)
        {
            var user = await _context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            if(user == null)
                return BadRequest("نام کاربری یا رمز عبور نادرست است");

            if (!PasswordHash.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return BadRequest("نام کاربری یا رمز عبور نادرست است");

            string token = CreateJwtToken(user);

            user.Token = token;
            await _context.SaveChangesAsync();

            return Ok(new LoginSchema
            {
                StatusCode = 200,
                Success = true,
                Message = "ورود شما موفقیت آمیز بود",
                Token = token,
            });
        }

        private string CreateJwtToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }
    }
}
