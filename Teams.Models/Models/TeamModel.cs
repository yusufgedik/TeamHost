using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Teams.Models.Models
{
    public class TeamModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Team Name is required")]
        public string Name { get; set; }
        public int AdminUser { get; set; }
    }
    public class TeamModelResponse:ResponseBase
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Team Name is required")]
        public string Name { get; set; }
        public int AdminUser { get; set; }
    }
    public class TeamRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
    }
}
