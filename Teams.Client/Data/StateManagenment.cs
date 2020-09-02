using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models.Models;

namespace Teams.Client.Data
{
    public class StateManagenment
    {
        public AuthenticateResponseModel userState { get; set; }
        public TeamModel team { get; set; }
        public string alert { get; set; }
        public TaskModel taskState { get; set; }
        public string workerServiceKey { get; set; }
    }
}
