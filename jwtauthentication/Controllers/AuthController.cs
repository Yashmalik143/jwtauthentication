using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;

namespace jwtauthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();

        private readonly IConfiguration _configuration;
        private readonly jwtDbContext _db;
        public AuthController(IConfiguration configuration,jwtDbContext db)
        {
          
            _configuration = configuration;
            _db = db;

        }
        [HttpPost("register")]


        public async Task<ActionResult<User>> Register(Userdto request)
        {
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);
            

            user.UserName = request.username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _db.Add(user);
            _db.SaveChanges();
            return Ok(user);
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Userdto request)
        {
            //if (user.UserName != request.username)
            //{
            //    return BadRequest("user not found");
            //}
            if((_db.logIn.Where(c => c.UserName.Contains(request.username)).ToList()).FirstOrDefault()==null)
            {
                return BadRequest("User Not Find");
            }
            //if (!VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt))
            //{
            //    return BadRequest("wrong password");
            //}
            if (!VerifyPasswordHash2(request.username, request.password))
            {
                return BadRequest("w22rong password");
            }


            string Token = CreateToken(user);
            return Ok(Token);
        }
        private bool VerifyPasswordHash2(string name,string password)
        {
            //var SearchTile = db.Job.Where(c => c.Title.Contains(Jobserach)).ToList();

            var s1 = (_db.logIn.Where(c => c.UserName.Contains(name)).FirstOrDefault());
           // User s1=user.FirstOrDefault();
           
            using (var hmac = new HMACSHA512(s1.PasswordSalt))
            {
                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(s1.PasswordHash);
            }


        }
            private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
               // new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            string lop = "lop";
            return jwt;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

 

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }



    }
}
