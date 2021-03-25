using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;

// create an account with the mobile no field and retrieve all contact record of same mobile no field.
// in plugin registration tool message name is = create and primary entity is = account 

namespace AccCreateRetriveMultAllContactBySameMobileNo
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = "telephone1";
                condition1.Operator = ConditionOperator.Equal;
                condition1.Values.Add("989898");
                FilterExpression filter1 = new FilterExpression();
                filter1.Conditions.Add(condition1);
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet.AddColumns("firstname", "lastname", "telephone1");
                query.Criteria.AddFilter(filter1);
                EntityCollection result1 = service.RetrieveMultiple(query);
                foreach (var a in result1.Entities)
                {
                    string sname = (string)a.Attributes["lastname"];
                    string moboleno = (string)a.Attributes["telephone1"];
                }
            }
        }
    }
}



//QueryExpression query = new QueryExpression("contact");
//query.ColumnSet = new ColumnSet(true);

//ConditionExpression condition1 = new ConditionExpression();
//condition1.attributename = "lastname";
//condition1.operator = conditionoperator.equal;
//condition1.Values.Add("Brown");
//FilterExpression filter1 = new FilterExpression();
//filter1.Conditions.Add(condition1);
//query.Criteria.AddFilter(filter);
//  EntityCollection result1 = service.RetrieveMultiple(query);