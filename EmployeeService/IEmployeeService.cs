using EmployeeService.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Services;


namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmployeeService" in both code and config file together.
    [ServiceContract]
    public interface IEmployeeService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetEmployeeById?id={id}",
            ResponseFormat = WebMessageFormat.Json,  BodyStyle = WebMessageBodyStyle.Bare)]
        List<Emploee> GetEmployeeById(int id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "EnableEmployee?id={id}&enable={enable}", 
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void EnableEmployee(int id, int enable);
    }

	
}
