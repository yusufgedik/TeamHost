using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text;

namespace Teams.Models.Models
{
    public enum MethodTypeEnum
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class ResponseBase
    {
        public string Result { get; set; }
    }
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ApiUrl { get; set; }
        public string ContentType { get; set; }
        public string HashKey { get; set; }
        public string Interval { get; set; }
        public string AuthanticationKey { get; set; }
    }
    public class AuthenticateModel
    {
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
    public class AuthenticateResponseModel : ResponseBase
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TeamModel[] TeamList { get; set; }
        public string Token { get; set; }
    }
    public class DeviceRequest
    {
        public int UserID { get; set; }
        public string NotificationKey { get; set; }
    }
    public class DeviceMessageRequest
    {
        public string to { get; set; }
        public NotificationMessage data { get; set; }
    }
    public class NotificationMessage
    {
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public string url { get; set; }
    }
    public class UpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public string Email { get; set; }
        public TeamModel[] TeamList { get; set; }
    }
    public class SearchUserList : UserModel
    {
        public bool IsTeamPersonel { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
