using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Api.Helper;
using Teams.Data;
using Teams.Models.Models;
using Data = Teams.Data;

namespace Teams.Api.Controllers
{
    public class TaskController : TeamsBaseController
    {
        [HttpPost]
        public IActionResult DeleteTask(TaskModel task)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                Data.Task taskEntity = entities.Tasks.FirstOrDefault(x => x.ID == task.ID && task.TeamID == x.TeamID);
                taskEntity.IsDelete = true;
                entities.SaveChanges();
                return Ok(new ResponseBase
                {
                    Result = "Success"
                });
            }
        }
        [HttpPost]
        public IActionResult SaveTask(TaskModel task)
        {
            DateTime today = DateTime.Now;
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                Data.Task taskEntity = entities.Tasks.FirstOrDefault(x => x.ID == task.ID);
                Data.Task taskResponse = new Data.Task();
                if (taskEntity == default)
                {
                    taskEntity = new Data.Task
                    {
                        Header = task.Header,
                        Detail = task.Detail,
                        UserID = task.UserID,
                        Progress = (short)task.Progress,
                        CreateBy = task.CreateBy,
                        CreateDate = today,
                        IsDelete = false,
                        StartDate = task.StartDate,
                        EndDate = task.EndDate,
                        TeamID = task.TeamID
                    };
                    taskResponse = entities.Tasks.Add(taskEntity);
                }
                else
                {
                    taskEntity.Header = task.Header;
                    taskEntity.Detail = task.Detail;
                    taskEntity.StartDate = task.StartDate;
                    taskEntity.EndDate = task.EndDate;
                    taskEntity.UserID = task.UserID;
                    taskEntity.Progress = (short)task.Progress;
                }
                Data.TaskLog taskLog = new TaskLog
                {
                    TaskID = taskEntity == default ? taskResponse.ID : taskEntity.ID,
                    ProgressID = (short)task.Progress,
                    UserID = task.UserID,
                    ModifyBy = task.CreateBy,
                    ModifyDate = DateTime.Now,
                };
                entities.TaskLogs.Add(taskLog);
                entities.SaveChanges();
                return Ok(new ResponseBase
                {
                    Result = "Success"
                });
            }
        }
        [HttpPost]
        public IActionResult GetTaskList(TaskRequest taskRequest)
        {
            using (TeamsDbEntities entities = new TeamsDbEntities())
            {
                List<Data.Task> taskEntity = entities.Tasks.Where(x => taskRequest.UserID.Contains(x.UserID.Value) && x.IsDelete == false && taskRequest.TeamID == x.TeamID).ToList();
                List<TaskModel> taskList = new List<TaskModel>();
                foreach (var task in taskEntity)
                {
                    TaskModel tempTask = new TaskModel
                    {
                        Header = task.Header,
                        Detail = task.Detail,
                        UserID = task.UserID.Value,
                        Progress = (ProgressTypeEnum)Enum.ToObject(typeof(ProgressTypeEnum), task.Progress),
                        CreateByModel = new UserModel
                        {
                            Email = task.User1.Email,
                            FirstName = task.User1.FirstName,
                            LastName = task.User1.LastName,
                            ID = task.CreateBy.Value
                        },
                        CreateBy = task.CreateBy.Value,
                        CreateDate = task.CreateDate.Value,
                        StartDate = task.StartDate.Value,
                        EndDate = task.EndDate.Value,
                        ID = task.ID
                    };
                    tempTask.TaskLogs = new List<TaskLogModel>();
                    foreach (var tLog in task.TaskLogs)
                    {
                        tempTask.TaskLogs.Add(new TaskLogModel
                        {
                            ID = tLog.ID,
                            ModifyBy = tLog.ModifyBy,
                            UserID = tLog.UserID,
                            ModifyByModel = new UserModel
                            {
                                Email = tLog.User.Email,
                                FirstName = tLog.User.FirstName,
                                LastName = tLog.User.LastName,
                                ID = tLog.User.ID
                            },
                            UserModel = new UserModel
                            {
                                Email = tLog.User1.Email,
                                FirstName = tLog.User1.FirstName,
                                LastName = tLog.User1.LastName,
                                ID = tLog.User1.ID
                            },
                            ModifyDate = tLog.ModifyDate,
                            Progress = (ProgressTypeEnum)Enum.ToObject(typeof(ProgressTypeEnum), tLog.ProgressID),
                            TaskID = tLog.TaskID
                        });
                    }
                    taskList.Add(tempTask);
                }
                return Ok(taskList.ToArray());
            }
        }
    }
}
