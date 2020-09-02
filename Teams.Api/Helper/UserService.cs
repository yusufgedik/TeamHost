using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;

namespace Teams.Api.Helper
{
    public class UserService : IUserService
    {
        public const string hashKey = "xQ3kww3w6jks5dN44vSRpdd3mTX1khd13C2CDZ1cqB2BM2MToZ5QoKEeDLv7kkgA26K8rMOWSaATZvU/AxKi+pm/EhPXyRDJgPL/rzstGjjXMMKB5wO9S7mKQpxsgyhWky82REY3h7FW8mEASmQY7S5RS7TVeYrqd1aNV9eNxPw=";

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                var user = entities.Users.FirstOrDefault(entity => entity.Email == email && entity.PasswordHash== password);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, hashKey))
                    return null;

                return user;
            }
        }

        public User[] GetAll()
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                return entities.Users.ToArray();
            }
        }

        public User GetById(int id)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                return entities.Users.Find(id);
            }
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                if (entities.Users.Any(x => x.Email == user.Email))
                    throw new AppException("Username \"" + user.Email + "\" is already taken");
                user.PasswordHash = password;

                entities.Users.Add(user);
                entities.SaveChanges();

                return user;
            }
            
        }

        public void Update(User userParam, string password = null)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                var user = entities.Users.Find(userParam.ID);

                if (user == null)
                    throw new AppException("User not found");

                // update username if it has changed
                if (!string.IsNullOrWhiteSpace(userParam.Email) && userParam.Email != user.Email)
                {
                    // throw error if the new username is already taken
                    if (entities.Users.Any(x => x.Email == userParam.Email))
                        throw new AppException("Email " + userParam.Email + " is already taken");

                    user.Email = userParam.Email;
                }

                // update user properties if provided
                if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                    user.FirstName = userParam.FirstName;

                if (!string.IsNullOrWhiteSpace(userParam.LastName))
                    user.LastName = userParam.LastName;

                // update password if provided
                if (!string.IsNullOrWhiteSpace(password))
                {
                    string passwordHash;
                    CreatePasswordHash(password, out passwordHash);

                    user.PasswordHash = passwordHash;
                }
                entities.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                var user = entities.Users.Find(id);
                if (user != null)
                {
                    entities.Users.Remove(user);
                    entities.SaveChanges();
                }
            }
           
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out string passwordHash)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(hashKey)))
            {
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }
        private static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(hashKey)))
            {
                var computedHash =Convert.FromBase64String(password);
                byte[] storedHashByte = Convert.FromBase64String(storedHash);
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHashByte[i]) return false;
                }
            }

            return true;
        }
    }
}
