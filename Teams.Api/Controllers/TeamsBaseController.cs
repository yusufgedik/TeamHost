using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Teams.Api.Helper;
using Teams.Models.Models;

namespace Teams.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TeamsBaseController : ControllerBase
    {
       
    }
}
