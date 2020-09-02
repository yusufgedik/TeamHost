using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Teams.Models.Validators;

namespace Teams.Models.Models
{
    public class TaskModel
    {
        public int ID { get; set; }
        [UserValidation]
        public int UserID { get; set; }
        [Required(ErrorMessage = "ProgressType is required")]
        public ProgressTypeEnum Progress { get; set; }
        [Required(ErrorMessage = "Header is required")]
        public string Header { get; set; }
        [Required(ErrorMessage = "Detail is required")]
        public string Detail { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public UserModel CreateByModel { get; set; }
        public bool IsDelete { get; set; }
        [DateValidation]
        public DateTime StartDate { get; set; }
        [DateValidation(isEndate = true)]
        public DateTime EndDate { get; set; }
        public List<TaskLogModel> TaskLogs { get; set; }
        public int TeamID { get; set; }
    }
    public class Appointment
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ProgressTypeEnum Progress { get; set; }
        public string Text { get; set; }
    }
    public class ChartItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
    public class TaskRequest
    {
        public int[] UserID { get; set; }
        public int TeamID { get; set; }
    }
    public class TaskLogModel
    {
        public int ID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public ProgressTypeEnum Progress { get; set; }
        public Nullable<int> UserID { get; set; }
        public UserModel UserModel { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<int> ModifyBy { get; set; }
        public UserModel ModifyByModel { get; set; }
    }
}
