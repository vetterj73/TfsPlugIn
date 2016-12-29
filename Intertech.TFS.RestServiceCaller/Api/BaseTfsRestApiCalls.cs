using System;
using System.Collections.Generic;
using System.Linq;
using Intertech.Configuration.ProgramTypeTemplate;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;

namespace Intertech.TFS.RestServiceCaller.Api
{
    public abstract class BaseTfsRestApiCalls
    {
        public readonly RestCaller RestCaller;
        public readonly string ApiVersion;

        public BaseTfsRestApiCalls(string baseUrl, string apiVersion)
        {
            RestCaller = new RestCaller(baseUrl);
            ApiVersion = apiVersion;
        }

        public void PopulateVariables(VariableCollection variables,
            IDictionary<string, ConfigurationVariableValue> definitionVaribles,
            List<object> paramList)
        {
            var arguments = definitionVaribles.GetType().GetGenericArguments();
            
            foreach (VariableElement ele in variables)
            {
                var propertyTypeToGetValue = Type.GetType(ele.VariableType);
                var val = string.Empty;
                if (!string.IsNullOrWhiteSpace(ele.VariableValue))
                {
                    val = ele.VariableValue;
                }
                else
                {
                    var foundParam = paramList.SingleOrDefault(pl => pl.GetType() == propertyTypeToGetValue);
                    if (foundParam != null &&
                        foundParam.GetType().GetProperties().ToList().Exists(p => p.Name == ele.VariableProp))
                    {
                        var prop = foundParam.GetType().GetProperty(ele.VariableProp);
                        var propValue = prop.GetValue(foundParam);
                        val = propValue.ToString();
                    }
                }

                bool exists = definitionVaribles.ContainsKey(ele.Name);

                if (exists)
                    definitionVaribles[ele.Name].Value = val;
                else
                    definitionVaribles.Add(ele.Name, new ConfigurationVariableValue {Value = val});
            }
        }


        public void PopulateVariables(VariableCollection variables, IDictionary<string, BuildDefinitionVariable> definitionVaribles,
            List<object> paramList)
        {
            foreach (VariableElement ele in variables)
            {
                var propertyTypeToGetValue = Type.GetType(ele.VariableType);
                var val = string.Empty;
                if (!string.IsNullOrWhiteSpace(ele.VariableValue))
                {
                    val = ele.VariableValue;
                }
                else
                {
                    var foundParam = paramList.SingleOrDefault(pl => pl.GetType() == propertyTypeToGetValue);
                    if (foundParam != null &&
                        foundParam.GetType().GetProperties().ToList().Exists(p => p.Name == ele.VariableProp))
                    {
                        var prop = foundParam.GetType().GetProperty(ele.VariableProp);
                        var propValue = prop.GetValue(foundParam);
                        val = propValue.ToString();
                    }
                }

                bool exists = definitionVaribles.ContainsKey(ele.Name);


                if (exists)
                    definitionVaribles[ele.Name].Value = val;
                else
                    definitionVaribles.Add(ele.Name, new BuildDefinitionVariable {Value = val});


            }
        }
    }
}