using System;
using System.Collections.Generic;
using System.Text;

namespace Teams.Models.Models
{
    public class MenuModel:ResponseBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        
        public int Order { get; set; }
        public string Icon { get; set; }

    }
}
