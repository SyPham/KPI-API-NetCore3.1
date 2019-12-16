using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Dto;
using Models.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Service;
using API.Dtos;
using Service.helpers;
using API.Helpers;
using AutoMapper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, 
            IConfiguration configuration,
            IMapper mapper)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _authService.FindByNameAsync(userForRegisterDto.Username) != null)
                return BadRequest("Username already exists");
          
            var user=  _mapper.Map<User>(userForRegisterDto);
            var createdUser = await _authService.Register(user, userForRegisterDto.Password);


            return CreatedAtRoute("GetUser", new { controller = "User", id = createdUser.ID }, userForRegisterDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _authService.FindByNameAsync(userForLoginDto.Username);

            var result = await _authService
                .Login(userForLoginDto.Username, userForLoginDto.Password);
            if (result == null)
                return Unauthorized();
            
            var userforreturn = new UserForReturnLogin
            {
                Username = user.Username,
                Permission = user.Permission,
                ID = user.ID,
                Alias = user.Alias
            };
            var userprofile = new UserProfileDto()
            {
                User = new UserForReturnLogin
                {
                    Username = user.Username,
                    Permission = user.Permission,
                    ID = user.ID,
                    Alias = user.Alias
                },
                Menus = JsonConvert.SerializeObject(await _authService.GetMenusAsync(user.Permission))
            };
            return Ok(new
            {
                token =  GenerateJwtToken(result),
                user = userprofile
            });

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] UserForLoginDto userForLoginDto)
        {
            return Ok(await _authService.Edit(userForLoginDto.Username));

        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypeEnum.LevelId.ToString(), user.LevelID.ToSafetyString(),ClaimTypeEnum.LevelId.ToString()),
                new Claim(ClaimTypeEnum.Alias.ToString(), user.Alias.ToSafetyString(),ClaimTypeEnum.Alias.ToString()),
                new Claim(ClaimTypeEnum.Role.ToString(), user.Permission.ToSafetyString(),ClaimTypeEnum.Role.ToString())

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}