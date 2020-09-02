using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;

namespace Teams.Api.Helper
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User[] GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }
}
