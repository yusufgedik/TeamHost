using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Teams.Api.Helper;
using Teams.Data;
using Teams.Models.Models;

namespace Teams.Api.Controllers
{
    public class UserController : TeamsBaseController
    {
        protected IUserService _userService;
        protected readonly AppSettings _appSettings;
        public UserController(
            IUserService userService,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { Result = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.ID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new AuthenticateResponseModel
            {
                ID = user.ID,
                Username = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString,
                TeamList = GetTeams(user)
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.Password,
                CreateDate = DateTime.Now
            };

            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok(new AuthenticateResponseModel { Result = "Success"});
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { Result = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult GetAllUser()
        {
            User[] users = _userService.GetAll();
            List<UserModel> response = new List<UserModel>();
            foreach (var user in users)
            {
                response.Add(new UserModel
                {
                    ID = user.ID,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TeamList = GetTeams(user)
                });
            }
            return Ok(response.ToArray());
        }
        [HttpPost]
        public IActionResult SaveDevice(DeviceRequest request)
        {
            try
            {
                using (TeamsDbEntities entities = new TeamsDbEntities())
                {
                    List<UserDevice> devices = entities.UserDevices.Where(p => p.UserID == request.UserID && p.NotificationKey == request.NotificationKey).ToList();
                    if (devices.Count == 0)
                    {
                        entities.UserDevices.Add(new UserDevice { NotificationKey = request.NotificationKey, UserID = request.UserID, CreateDate = DateTime.Now });
                        entities.SaveChanges();
                    }
                }
                return Ok(new ResponseBase { Result = "Success" });
            }
            catch (Exception)
            {
                return BadRequest(new ResponseBase { Result = "System Error." });
            }
            
        }
        [HttpPost]
        public IActionResult GetByToken(AuthenticateResponseModel userRequest)
        {
            User user = _userService.GetById(Convert.ToInt32(HttpContext.User.Identity.Name)); 
            return Ok(new AuthenticateResponseModel {
            ID = user.ID,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Result = "",
            Token = userRequest.Token,
            Username = user.Email,
            TeamList = GetTeams(user)
            });
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var model = new UserModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ID = user.ID,
                TeamList = GetTeams(user)
            };
            return Ok(model);
        }

        private static TeamModel[] GetTeams(User user)
        {
            List<TeamModel> teamList = new List<TeamModel>();
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                List<TeamUserStatu> teamStatus = entities.TeamUserStatus.Where(p => p.UserID == user.ID).ToList();
                if (teamStatus.Count > 0)
                {
                    foreach (var status in teamStatus)
                    {
                        teamList.Add(new TeamModel
                        {
                            ID = status.Team.ID,
                            Name = status.Team.TeamName,
                            AdminUser = status.Team.AdminUser.Value
                        });
                    }
                    return teamList.ToArray();
                }
            }
            return null;
        }
        [HttpPost]
        public IActionResult SaveTeam(TeamRequest team)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                List<Team> teamList = entities.Teams.Where(p => p.TeamName == team.Name).ToList();
                if (teamList.Count > 0)
                {
                    return BadRequest(new { Result = "Team name used before" });
                }
                else
                {
                    Team createTeam = entities.Teams.Add(new Team
                    {
                        TeamName = team.Name,
                        AdminUser = team.UserID,
                        
                    });
                    entities.TeamUserStatus.Add(new TeamUserStatu { TeamID = createTeam.ID, UserID = team.UserID });
                    entities.SaveChanges();
                    return Ok(new TeamModel {Name = createTeam.TeamName,ID=createTeam.ID,AdminUser = createTeam.AdminUser.Value });
                }
            }
        }
    }
}
