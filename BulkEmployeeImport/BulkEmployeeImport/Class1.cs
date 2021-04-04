using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using BulkEmployeeImport;
//using BulkEmployeeImport.getEmployeeDetailsServiceReference;
using System.Data;
using BulkEmployeeImport.EmpDetailsServiceReference1;
namespace BulkEmployeeImport
{
    public class acount
    {
        string _firstname;

        string _lastname;


        public string Firstname
        {
            get
            {
                return _firstname;
            }

            set
            {
                _firstname = value;
            }
        }

        public string Lastname
        {
            get
            {
                return _lastname;
            }

            set
            {
                _lastname = value;
            }
        }
    }

    public class Class1 : IPlugin
    {
        
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
           
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
           OptionSetValue obj = entity.GetAttributeValue<OptionSetValue>("new_bulkemp");

                DataTable dt = new DataTable();

                BasicHttpBinding myBinding = new BasicHttpBinding();
                myBinding.Name = "BasicHttpBinding_IPOCService";
                myBinding.Security.Mode = BasicHttpSecurityMode.None;
                myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                myBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                EndpointAddress endPointAddress = new EndpointAddress("http://localhost:62830/Service1.svc");
                ChannelFactory<IService1> factory = new ChannelFactory<IService1>(myBinding, endPointAddress);
                IService1 channel = factory.CreateChannel();
                //  acount[] lst = new acount[2];
                //   acount ac=new acount();
                //   List<acount>[] lst = new List<acount>[2]; 
               List<string> l = new List<string>();
                account[] a = new account[2];
                   a= channel.getData();
                factory.Close();

                if (obj.Value == 100000000)
                {
                    for (int i = 0; i < a.Length; i++)
                {
                    
                        Entity emp = new Entity("new_employee");
                        // emp["new_empid"] = l.Rows[0][0];
                        emp["new_firstname"] = a[i].Firstname; // dt.Rows[0][1];
                        emp["new_lastname"] = a[i].Lastname; //dt.Rows[0][2];
                        emp["new_address"] = a[i].Address;
                        emp["new_designation"] = a[i].Designation;
                        emp["new_emailid"] = a[i].Emailid;
                        emp["new_empid1"] = a[i].Empid.ToString();
                        emp["new_mobilenumber1"] = a[i].Mobilenumber.ToString();
                        emp["new_salary"] = a[i].Salary;
                        emp["new_technology"] = a[i].Technology;
                        emp["new_name"] = a[i].Firstname +" "+ a[i].Lastname;

                        //emp["new_emailid"] = dt.Rows[0][2];
                        //emp["new_designation"] = dt.Rows[0][2];
                        //emp["new_technology"] = dt.Rows[0][2];
                        //emp["new_salary"] = dt.Rows[0][2];
                        //emp["new_address"] = dt.Rows[0][2];
                        //emp["new_lastname"] = dt.Rows[0][2];
                        //  emp["new_lastname"] = dt.Rows[0][2];
                        service.Create(emp);
                    }
                 //   throw new InvalidPluginExecutionException("Employee Records are Created");

                }
                
            }
        }
    }
}