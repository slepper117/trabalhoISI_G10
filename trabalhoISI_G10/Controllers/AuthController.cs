using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using trabalhoISI_G10.Config;
using trabalhoISI_G10.DTO;
using BC = BCrypt.Net.BCrypt;

namespace trabalhoISI_G10.Controllers
{
    /// <summary>
    /// Auth Controller
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> Login(UserLogin login)
        {
            try
            {
                // Initialize Datasource
                await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(Database.ConnectionString());

                // Get user
                string query = $"SELECT id, name, password FROM setr.users WHERE username = '{login.Username}';";
                await using NpgsqlCommand cmd = dataSource.CreateCommand(query);
                await using NpgsqlDataReader rdr = await cmd.ExecuteReaderAsync();

                // Check if there are items
                if (!rdr.HasRows) return Unauthorized("Username or Password Incorrect");

                // Verify Password
                await rdr.ReadAsync();
                int userId = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                bool verifyPassword = BC.Verify(login.Password, rdr.GetString(2));
                if (!verifyPassword) return Unauthorized("Username or Password Incorrect");

                // Create JWT
                string issuer = Jwt.Issuer;
                string audience = Jwt.Audience;
                string secret = Jwt.Secret;
                DateTime now = DateTime.UtcNow;
                JwtSecurityTokenHandler handler = new();
                Console.WriteLine(secret);

                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, issuer),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToString()),
                    new Claim("id", userId.ToString()),
                    new Claim("name", name),
                    new Claim("username", login.Username)
                };

                JwtSecurityToken token = new 
                (
                    issuer,
                    audience,
                    claims,
                    now.AddMilliseconds(-30),
                    now.AddMinutes(60),
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha512)
                );

                return Ok(handler.WriteToken(token));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
