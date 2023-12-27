using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeService.Models
{
    public class Emploee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ManagerID { get; set; }
        public Boolean Enable { get; set; }
        public List<Emploee> Employee { get; set; }
    }
}