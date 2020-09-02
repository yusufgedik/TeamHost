using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Teams.Data;
using Teams.Models.Models;
namespace Teams.Api.Controllers
{
   
    public class MenuController : TeamsBaseController
    {
        private readonly ILogger<MenuController> _logger;

        public MenuController(ILogger<MenuController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetMenu()
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                try
                {
                    Menu[] menuList = entities.Menus.OrderBy(menuItem => menuItem.Order).ToArray();
                    List<MenuModel> menuresponse = new List<MenuModel>();
                    foreach (var item in menuList)
                    {
                        menuresponse.Add(new MenuModel
                        {
                            ID = item.ID,
                            Name = item.Name,
                            Order = item.Order,
                            Title = item.Title,
                            Icon = item.Icon
                        });
                    }
                    return Ok(menuresponse.ToArray());
                }
                catch (Exception)
                {
                    return BadRequest(new { Result = "Server Error" });
                }
                
                
            }
        }
    }
}
