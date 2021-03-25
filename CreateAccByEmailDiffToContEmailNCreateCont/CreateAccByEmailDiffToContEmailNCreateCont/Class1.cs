using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

//If email of account is same of contact email then dont create account otherwise create account and update all data to new contact .
// message name = create .... primary entity = account 

namespace CreateAccByEmailDiffToContEmailNCreateCont
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            Entity contact = new Entity("contact");
            Guid newconId = new Guid();
            Guid accountId = new Guid();

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (context.PrimaryEntityName == "account" && context.MessageName == "Create" && context.Stage == 20)
                {

                    
                    Guid Accountid = context.PrimaryEntityId;
                    accountId = new Guid(Accountid.ToString().ToUpper());
                    Guid accId = entity.Id;
                    string Email = entity.Attributes["new_emails"].ToString();
                    string Name = entity.Attributes["name"].ToString();
                    var result = Name.Substring(Name.Length - 2);
                    bool Gender = entity.GetAttributeValue<bool>("new_gender");
                    //entity.Attributes["attributename"] = true;
                    string Jobtitle = entity.Attributes["new_jobtitle"].ToString();
                    Money Salary = entity.GetAttributeValue<Money>("new_salary");
                    DateTime JoiningDate = entity.GetAttributeValue<DateTime>("new_joiningdate");
                    string Phone = entity.Attributes["telephone1"].ToString();
                    string Mobileno = entity.Attributes["new_mobileno"].ToString();
                    string Fax = entity.Attributes["fax"].ToString();
                    int Branch = entity.GetAttributeValue<OptionSetValue>("new_branch").Value;

                    ConditionExpression condition1 = new ConditionExpression();
                    condition1.AttributeName = "emailaddress1";
                    condition1.Operator = ConditionOperator.Equal;
                    condition1.Values.Add(Email);
                    FilterExpression filter1 = new FilterExpression();
                    filter1.Conditions.Add(condition1);
                    QueryExpression query = new QueryExpression("contact");
                    query.ColumnSet = new ColumnSet(true);
                    query.Criteria.AddFilter(filter1);
                    EntityCollection result1 = service.RetrieveMultiple(query);

                    if (result1.Entities.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        //Entity contact = new Entity("contact");
                        //var result = Name.Substring(Name.Length - 2);
                        contact["lastname"] = "contact" + result;
                        //contact["contactid"] = newlookupid;
                        contact["new_gender"] = Gender;
                        contact["jobtitle"] = Jobtitle;
                        contact["new_salary"] = Salary;
                        contact["new_joiningdate"] = JoiningDate;
                        contact["telephone1"] = Phone;
                        contact["mobilephone"] = Mobileno;
                        contact["fax"] = Fax;
                        //contact["new_branch"] = Branch;
                        contact["new_branch"] = new OptionSetValue(Branch);
                        contact["emailaddress1"] = Email;
                        //contact["parentcustomerid"] = new EntityReference("account", AccountId);

                        Guid conId = service.Create(contact);
                        newconId = new Guid(conId.ToString().ToUpper());
                        
                        context.SharedVariables.Add("newConId", (object)newconId.ToString());
                        context.SharedVariables.Add("accountId", (object)accountId.ToString());
                    }
                }
                else
                {
                    if (context.PrimaryEntityName == "account" && context.MessageName == "Create" && context.Stage == 40)
                    {
                        if (context.SharedVariables.Contains("newConId") && context.SharedVariables.Contains("accountId"))
                        {
                            string ConId = (string)context.SharedVariables["newConId"].ToString().ToUpper();
                            Guid share_ConId = new Guid(ConId);

                            string AccountId = (string)context.SharedVariables["accountId"].ToString().ToUpper();
                            Guid share_AccountId = new Guid(AccountId);

                            contact.Id = share_ConId;
                            contact["parentcustomerid"] = new EntityReference("account", share_AccountId);
                            service.Update(contact);

                            Entity account = new Entity("account");
                            account.Id = share_AccountId;
                            account["primarycontactid"] = new EntityReference("contact", share_ConId);
                            Guid accID = (Guid)entity.Id;
                            service.Update(account);
                        }
                    }
                }
            }
        }
    }
}
