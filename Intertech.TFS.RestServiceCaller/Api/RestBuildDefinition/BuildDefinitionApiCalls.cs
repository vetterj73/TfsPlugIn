using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Intertech.Configuration.ProgramTypeTemplate;
using Intertech.Tfs.Common.Models;
using Intertech.Tfs.Common.Utilities;
using Microsoft.TeamFoundation.Build.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intertech.TFS.RestServiceCaller.Api.RestBuildDefinition
{
    public class BuildDefinitionApiCalls : BaseTfsRestApiCalls
    {

        public BuildDefinitionApiCalls(string baseUrl, string apiVersion) : base(baseUrl, apiVersion)
        {

        }

        public BuildDefinition CreateBuildDefinition(BuildDefinition template, ChangeSetItemInfo itemInfo, BuildTemplateElement buildTemplateInfo)
        {
            var paramList = new List<object> {template, itemInfo, buildTemplateInfo};

            var buildDefinition = new BuildDefinition
            {
                Name = $"{itemInfo.ProgramName}_Dev_{itemInfo.BranchName}"
            };

            PopulateVariables(buildTemplateInfo.Variables, buildDefinition.Variables, paramList);
            
            buildDefinition.Repository = template.Repository;
            buildDefinition.Repository.DefaultBranch = itemInfo.ItemPath;

            var mappingInfo = buildDefinition.Repository.Properties[Constants.TfsVersionControlMapping];
            var jObj = JObject.Parse(mappingInfo);
            jObj["mappings"][0]["serverPath"] = itemInfo.ItemPath;
            buildDefinition.Repository.Properties[Constants.TfsVersionControlMapping] = jObj.ToString();
            template.Steps.ForEach(buildDefinition.Steps.Add);
            template.RetentionRules.ForEach(buildDefinition.RetentionRules.Add);
            buildDefinition.RetentionRules[0].DaysToKeep = 5;

            var resultString = RestCaller.MakeHttpCall(string.Format(Constants.TfsCreateBuildDefinitionPattern, ApiVersion), 
                buildDefinition,
                HttpMethod.Post).Result;
            var returnedBuildDefinition = JsonConvert.DeserializeObject<BuildDefinition>(resultString);

            return returnedBuildDefinition;
        }

        public void DeleteBuildDefinition(ChangeSetItemInfo itemInfo)
        {
            var buildDefinitions = GetBuildDefinitions();

            var buildName = $"{itemInfo.ProgramName}_Dev_{itemInfo.BranchName}";
            var build = buildDefinitions.SingleOrDefault(bd => bd.Name == buildName);
            if (build != null)
                DeleteBuildDefinitionById(build.Id);
        }

        public void DeleteBuildDefinitionById(int definitionId)
        {
            RestCaller.MakeHttpCall<List<BuildDefinitionTemplate>>(string.Format(Constants.TfsDeleteBuildDefinitionPattern, definitionId, ApiVersion), 
                null, 
                HttpMethod.Delete).Wait();
        }

        public BuildDefinition GetBuildDefinitionById(int definitionId)
        {
            var resultString = RestCaller.MakeHttpCall<List<BuildDefinitionTemplate>>(string.Format(Constants.TfsDeleteBuildDefinitionPattern, definitionId, ApiVersion),
                null,
                HttpMethod.Get).Result;
            return JsonConvert.DeserializeObject<BuildDefinition>(resultString);
        }

        public List<BuildDefinition> GetBuildDefinitions()
        {
            var buildDefinitions = new List<BuildDefinition>();
            var resultString = RestCaller.MakeHttpCall<List<BuildDefinitionTemplate>>(string.Format(Constants.TfsCreateBuildDefinitionPattern, ApiVersion), 
                null,
                HttpMethod.Get).Result;

            var jObject = JObject.Parse(resultString);
            var results = jObject["value"].Children().ToList();
            results.ForEach(jt => buildDefinitions.Add(JsonConvert.DeserializeObject<BuildDefinition>(jt.ToString())));
            return buildDefinitions;
        }

        public List<BuildDefinitionTemplate> GetDefinitionTemplates()
        {
            var tempaltes = new List<BuildDefinitionTemplate>();

            var resultString = RestCaller.MakeHttpCall<List<BuildDefinitionTemplate>>(string.Format(Constants.TfsBuildTemplateListPattern, ApiVersion), 
                null,
                HttpMethod.Get).Result;

            var jObject = JObject.Parse(resultString);
            var results = jObject["value"].Children().ToList();
            results.ForEach(jt => tempaltes.Add(JsonConvert.DeserializeObject<BuildDefinitionTemplate>(jt.ToString())));

            return tempaltes;
        }
    }
}