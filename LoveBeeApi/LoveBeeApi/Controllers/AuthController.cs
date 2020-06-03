using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LoveBeeApi.DTOs;
using LoveBeeApi.Models;
using LoveBeeApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LoveBeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configure; 

        public AuthController(IUnitOfWork unitOfWork, IConfiguration configure)
        {
            _unitOfWork = unitOfWork;
            _configure = configure;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            //validation request
            userRegisterDto.Username = userRegisterDto.Username.ToLower();

            if (await _unitOfWork.Auth.UserExists(userRegisterDto.Username))
                return BadRequest("Username already exists!");

            var newUser = new User
            {
                Username = userRegisterDto.Username
            };

             await _unitOfWork.Auth.Register(newUser, userRegisterDto.Password);
             await _unitOfWork.CompleteAsync();
             
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _unitOfWork.Auth.Login(userLoginDto.Username, userLoginDto.Password);

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                                .GetBytes(_configure.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}