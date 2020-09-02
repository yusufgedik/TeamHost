using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Teams.Models.Models;

namespace Teams.Client.Data
{
    public class TeamsService
    {
        private const string AuthenticationServiceName = "User/Authenticate";
        private const string MenuServiceName = "Menu/GetMenu";
        private const string RegisterServiceName = "User/Register";
        private const string GetByTokenServiceName = "User/GetByToken";
        private const string GetAlluserServiceName = "User/GetAllUser";
        private const string SaveTaskServiceName = "Task/SaveTask";
        private const string GetTaskListServiceName = "Task/GetTaskList";
        private const string DeleteTaskServiceName = "Task/DeleteTask";
        private const string SaveTeamServiceName = "User/SaveTeam";
        private const string SaveDeviceServiceName = "User/SaveDevice";
        private const string SaveUserTeamServiceName = "User/SaveUserTeam";
        private const string DeleteTeamServiceName = "User/DeleteTeam";
        private StateManagenment _state;
        public TeamsService(IConfiguration configuration, StateManagenment state)
        {
            Configuration = configuration;
            var appSettingsSection = Configuration.GetSection("AppSettings");
            AppSettings = appSettingsSection.Get<AppSettings>();
            _state = state;
        }
        private AppSettings AppSettings { get; }
        public string Token { get; set; }
        public IConfiguration Configuration { get; }
        public async Task<string> CallAsync(string method, string Request, MethodTypeEnum MethodType, string token)
        {

            HttpRequestMessage http = new HttpRequestMessage();
            http.Method = new HttpMethod(MethodType.ToString());
            http.RequestUri = new Uri(AppSettings.ApiUrl + method);
            http.Content = new StringContent(Request);
            http.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(AppSettings.ContentType);
            HttpClient connect = new HttpClient();
            if (token != null)
            {
                connect.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await connect.SendAsync(http);
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }
        public async Task<UserModel[]> GetAllUser()
        {
            string responseString = await CallAsync(GetAlluserServiceName, "", MethodTypeEnum.POST, _state.userState.Token);
            UserModel[] response = JsonConvert.DeserializeObject<UserModel[]>(responseString);
            return response;
        }
        public async Task<AuthenticateResponseModel> Authenticate(AuthenticateModel user)
        {
            AuthenticateModel request = new AuthenticateModel { Email = user.Email, Remember = user.Remember };

            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(AppSettings.HashKey)))
            {
                request.Password = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password)));
            }
            string responseString = await CallAsync(AuthenticationServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, null);
            AuthenticateResponseModel response = JsonConvert.DeserializeObject<AuthenticateResponseModel>(responseString);
            return response;
        }

        public async Task<AuthenticateResponseModel> GetUserByAccessTokenAsync(string token)
        {
            AuthenticateResponseModel request = new AuthenticateResponseModel();
            request.Token = token;
            string responseString = await CallAsync(GetByTokenServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, token);
            AuthenticateResponseModel response = JsonConvert.DeserializeObject<AuthenticateResponseModel>(responseString);
            return response;
        }

        public async Task<AuthenticateResponseModel> Register(RegisterModel user)
        {
            RegisterModel request = new RegisterModel { Email = user.Email, LastName = user.LastName, FirstName = user.FirstName };
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(AppSettings.HashKey)))
            {
                request.Password = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password)));
            }
            string responseString = await CallAsync(RegisterServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, null);
            AuthenticateResponseModel response = JsonConvert.DeserializeObject<AuthenticateResponseModel>(responseString);
            return response;
        }
        public async Task<MenuModel[]> GetMenu()
        {
            string responseString = await CallAsync(MenuServiceName, "", MethodTypeEnum.POST, null);
            MenuModel[] response = JsonConvert.DeserializeObject<MenuModel[]>(responseString);
            response = response.OrderBy(p => p.Order).ToArray();
            return response;
        }

        public async Task<ResponseBase> SaveTask(TaskModel task)
        {
            string responseString = await CallAsync(SaveTaskServiceName, JsonConvert.SerializeObject(task), MethodTypeEnum.POST, _state.userState.Token);
            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(responseString);
            return response;
        }
        public async Task<ResponseBase> SaveDevice(DeviceRequest device)
        {
            string responseString = await CallAsync(SaveDeviceServiceName, JsonConvert.SerializeObject(device), MethodTypeEnum.POST, _state.userState.Token);
            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(responseString);
            return response;
        }
        public async Task<TaskModel[]> GetTaskList(TaskRequest task)
        {
            string responseString = await CallAsync(GetTaskListServiceName, JsonConvert.SerializeObject(task), MethodTypeEnum.POST, _state.userState.Token);
            TaskModel[] response = JsonConvert.DeserializeObject<TaskModel[]>(responseString);
            return response;
        }
        public async Task<ResponseBase> DeleteTask(TaskModel task)
        {
            string responseString = await CallAsync(DeleteTaskServiceName, JsonConvert.SerializeObject(task), MethodTypeEnum.POST, _state.userState.Token);
            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(responseString);
            return response;
        }
        public async Task<TeamModelResponse> SaveTeam(TeamRequest request)
        {
            string responseString = await CallAsync(SaveTeamServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, _state.userState.Token);
            TeamModelResponse response = JsonConvert.DeserializeObject<TeamModelResponse>(responseString);
            return response;
        }
    public async Task<ResponseBase> DeleteTeam(DeleteTeamRequest request)
        {
            string responseString = await CallAsync(DeleteTeamServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, _state.userState.Token);
            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(responseString);
            return response;
        }
public async Task<ResponseBase> SaveUserTeam(SaveUserTeamRequest request)
        {
            string responseString = await CallAsync(SaveUserTeamServiceName, JsonConvert.SerializeObject(request), MethodTypeEnum.POST, _state.userState.Token);
            ResponseBase response = JsonConvert.DeserializeObject<ResponseBase>(responseString);
            return response;
        }
    }
}
