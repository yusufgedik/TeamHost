using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Teams.Models.Models;
using Data = Teams.Data;

namespace Notification.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        List<Data.Task> taskEntity = new List<Data.Task>();
        List<Data.UserDevice> userDevice = new List<Data.UserDevice>();
        DateTime today = DateTime.Now.Date;

        public Worker(ILogger<Worker> logger,AppSettings options)
        {
            _logger = logger;
            _options = options;
        }
        private readonly AppSettings _options;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var key = _options.Interval;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    today = DateTime.Now.Date;
                    taskDetail();
                    foreach (var taskItem in taskEntity)
                    {
                        if (userDevice.Any(p => p.UserID == taskItem.UserID))
                        {
                            List<Data.UserDevice> temp = userDevice.Where(p => p.UserID == taskItem.UserID).ToList();
                            foreach (var deviceItem in temp)
                            {
                                DeviceMessageRequest request = new DeviceMessageRequest();
                                request.to = deviceItem.NotificationKey;
                                request.data = new NotificationMessage();
                                request.data.icon = "https://localhost:44340/logo.png";
                                request.data.title = "Task Date is approaching";
                                request.data.url = "https://localhost:44340";
                                request.data.body = taskItem.Header + ": ";
                                if (taskItem.StartDate == today)
                                {
                                    request.data.title = "Task Date is approaching";
                                }
                                else if (taskItem.EndDate == today)
                                {
                                    request.data.title = "Task Date is about to end";
                                }
                                if (taskItem.Detail.Length > 10)
                                {
                                    request.data.body += taskItem.Detail.Substring(0, 10);
                                }
                                else
                                {
                                    request.data.body += taskItem.Detail;
                                }
                                await CallAsync("", JsonConvert.SerializeObject(request), MethodTypeEnum.POST, "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,ex.Message,null);
                }
                await Task.Delay(60000 * Convert.ToInt16(key), stoppingToken);
            }
        }
        public async Task<string> CallAsync(string method, string Request, MethodTypeEnum MethodType, string token)
        {

            HttpRequestMessage http = new HttpRequestMessage();
            http.Method = new HttpMethod(MethodType.ToString());
            http.RequestUri = new Uri(_options.ApiUrl + method);
            http.Content = new StringContent(Request);
            http.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_options.ContentType);
            HttpClient connect = new HttpClient();
            if (token != null)
            {
                connect.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.AuthanticationKey);
            }
            var response = await connect.SendAsync(http);
            var responseString = response.Content.ReadAsStringAsync();
            return responseString.Result;
        }
        private void taskDetail()
        {
            using (Data.TeamsDbEntities entities = new Data.TeamsDbEntities())
            {
                taskEntity = entities.Tasks.Where(p=>p.StartDate == today || p.EndDate == today).ToList();
                List<int> userIdList = taskEntity.Select(p => p.UserID.Value).ToList();
                userDevice = entities.UserDevices.Where(p => userIdList.Contains(p.UserID.Value)).ToList();
            }
        }
    }
}
