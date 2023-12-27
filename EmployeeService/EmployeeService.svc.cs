using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using EmployeeService.Models;
using EmployeeService.Services;
using Newtonsoft.Json;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmployeeService.svc or EmployeeService.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        public List<Emploee> GetEmployeeById(int id)
        {
            List<Emploee> employees = new List<Emploee>();
            using(SqlConnection connection = SQLService.GetSqlConnection())
            {
                DataTable parent = SQLService.ExecuteReader(
                    connection,
                    $"SELECT ID, Name, ManagerID FROM Employee WHERE ID = {id} AND Enable = 1 "
                );

                foreach (DataRow row in parent.Rows)
                {
                    Emploee emploee = new Emploee();
                    emploee.ID = Convert.ToInt32(row["ID"]);
                    emploee.Name = row["Name"].ToString();
                    if (row.IsNull("ManagerID"))
                    {
                        emploee.ManagerID = null;
                    }
                    else
                    {
                        emploee.ManagerID = (int?)row["ManagerID"];
                    }
                    emploee.Employee = GetChildrenEmployee(connection, emploee.ID);
                    employees.Add(emploee);
                }
                return employees;
            }
        }



        public void EnableEmployee(int id, int enable)
        {
            HashSet<int> childrenIds = new HashSet<int>();
            int parentId = 0;
            using (SqlConnection connection = SQLService.GetSqlConnection())
            {
                DataTable parent = SQLService.ExecuteReader(
                    connection,
                    $"SELECT ID, ManagerID FROM Employee WHERE ID = {id} AND Enable = 1 "
                );

                foreach (DataRow row in parent.Rows)
                {
                    parentId = Convert.ToInt32(row["ID"]);

                    if (!row.IsNull("ManagerID") && enable == 0)
                    {
                        childrenIds = (GetChildrenId(connection, (int?)row["ManagerID"]));
                    }
                }

                SQLService.ExecuteNonQuery(connection, $"UPDATE Employee SET Enable = {enable} WHERE ID = {parentId}");

                //Remove inactive Manager from child employees.
                if(childrenIds.Count > 0)
                {
                    SQLService.ExecuteNonQuery(connection, $"UPDATE Employee SET ManagerID = NULL WHERE ID IN ({String.Join(",",childrenIds)})");
                }
            }

        }

        private List<Emploee> GetChildrenEmployee(SqlConnection connection, int? ManagerID)
        {
            List<Emploee> employees = new List<Emploee>();
            DataTable child = SQLService.ExecuteReader(connection, $"SELECT ID, Name, ManagerID FROM Employee WHERE ManagerID = {ManagerID} AND Enable = 1 ");
            if (child != null)
            {
                foreach (DataRow row in child.Rows)
                {
                    Emploee emploee = new Emploee();
                    emploee.ID = Convert.ToInt32(row["ID"]);
                    emploee.Name = row["Name"].ToString();
                    if (row.IsNull("ManagerID"))
                    {
                        emploee.ManagerID = null;
                        emploee.Employee = new List<Emploee>();
                    }
                    else
                    {
                        emploee.ManagerID = (int?)row["ManagerID"];
                        //Avoid infinite loop when ID = ManagerID
                        if(emploee.ID != emploee.ManagerID)
                        {
                            emploee.Employee = GetChildrenEmployee(connection, emploee.ID);
                        } else
                        {
                            emploee.Employee = new List<Emploee>();
                        }   
                    }
                    employees.Add(emploee);
                }
            }
            return employees;
        }

        private HashSet<int> GetChildrenId(SqlConnection connection, int? ManagerID)
        {
            HashSet<int> ids = new HashSet<int>();
            DataTable children = SQLService.ExecuteReader(connection, $"SELECT ID, ManagerID FROM Employee WHERE ManagerID = {ManagerID} AND Enable = 1");
            if (children != null)
            {
                foreach (DataRow row in children.Rows)
                {
                    ids.Add(Convert.ToInt32(row["ID"]));
                }
            }
            return ids;
        }
    }
}