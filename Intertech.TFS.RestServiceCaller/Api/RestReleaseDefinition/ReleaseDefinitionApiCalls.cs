using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Intertech.Configuration.ProgramTypeTemplate;
using Intertech.Tfs.Common.Models;
using Intertech.Tfs.Common.Utilities;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intertech.TFS.RestServiceCaller.Api.RestReleaseDefinition
{
    public class ReleaseDefinitionApiCalls : BaseTfsRestApiCalls
    {
        public ReleaseDefinitionApiCalls(string baseUrl, string apiVersion) : base(baseUrl, apiVersion)
        {

        }

        public ReleaseDefinition CreateReleaseDefinition(ReleaseDefinition template, BuildDefinition buildArtifact, ChangeSetItemInfo itemInfo, ReleaseTemplateElement releaseTemplate)
        {
            var paramList = new List<object> {template, buildArtifact, itemInfo};

            var releaseDefinition = new ReleaseDefinition
            {
                Name = $"{itemInfo.ProgramName}_{itemInfo.BranchName}"
            };

            releaseDefinition.Environments = new List<ReleaseDefinitionEnvironment>();
            foreach (var environment in template.Environments)
            {
                releaseDefinition.Environments.Add(environment);
                environment.Variables.Clear();
                var configEnvironemt = releaseTemplate.Environments.SingleOrDefault(en => en.Name == environment.Name);
                if (configEnvironemt != null)
                    PopulateVariables(configEnvironemt.Variables, environment.Variables, paramList);
                
                var sourcePath = environment.DeployStep.Tasks[0].Inputs["SourcePath"];
                var sourPathElements = sourcePath.Split('/');
                sourPathElements[sourPathElements.Length - 2] = buildArtifact.Name;
                sourcePath = string.Join("/", sourPathElements);
                environment.DeployStep.Tasks[0].Inputs["SourcePath"] = sourcePath;
            }
            releaseDefinition.RetentionPolicy = template.RetentionPolicy;
            releaseDefinition.Triggers = template.Triggers;
            releaseDefinition.Triggers[0].ArtifactAlias = buildArtifact.Name;

            PopulateVariables(releaseTemplate.Variables, releaseDefinition.Variables, paramList);

            releaseDefinition.Artifacts = template.Artifacts;
            releaseDefinition.Artifacts[0].Alias = buildArtifact.Name;
            releaseDefinition.Artifacts[0].DefinitionReference["definition"].Id = buildArtifact.Id.ToString();
            releaseDefinition.Artifacts[0].DefinitionReference["definition"].Name = buildArtifact.Name;

            var resultString = RestCaller.MakeHttpCall(string.Format(Constants.TfsCreateReleaseDefinitionPattern, ApiVersion),
                releaseDefinition,
                HttpMethod.Post).Result;
            var returnedReleaseDefinition = JsonConvert.DeserializeObject<ReleaseDefinition>(resultString);

            return returnedReleaseDefinition;
        }

        public void DeleteReleaseDefinition(ChangeSetItemInfo itemInfo)
        {
            var releaseName = $"{itemInfo.ProgramName}_{itemInfo.BranchName}";
            var releaseDefinitions = GetReleaseDefinitions();
            var releaseToDelete = releaseDefinitions.SingleOrDefault(rd => rd.Name == releaseName);
            if (releaseToDelete != null)
                DeleteReleaseDefinitionById(releaseToDelete.Id);
        }

        public List<ReleaseDefinition> GetReleaseDefinitions()
        {
            var releaseDefinitions = new List<ReleaseDefinition>();
            var resultString = RestCaller.MakeHttpCall<List<ReleaseDefinition>>(string.Format(Constants.TfsCreateReleaseDefinitionPattern, ApiVersion),
                null,
                HttpMethod.Get).Result;

            var jObject = JObject.Parse(resultString);
            var results = jObject["value"].Children().ToList();
            results.ForEach(jt => releaseDefinitions.Add(JsonConvert.DeserializeObject<ReleaseDefinition>(jt.ToString())));
            return releaseDefinitions;
        }

        public ReleaseDefinition GetReleaseDefinitionById(int releaseDefinitionId)
        {
            var resultString = RestCaller.MakeHttpCall<List<ReleaseDefinition>>(string.Format(Constants.TfsGetReleaseDefinitionPattern,  releaseDefinitionId, ApiVersion),
                null,
                HttpMethod.Get).Result;

            return JsonConvert.DeserializeObject<ReleaseDefinition>(resultString);
        }

        public void DeleteReleaseDefinitionById(int definitionId)
        {
            RestCaller.MakeHttpCall<List<BuildDefinitionTemplate>>(string.Format(Constants.TfsGetReleaseDefinitionPattern, definitionId, ApiVersion),
                null,
                HttpMethod.Delete).Wait();
        }

        
    }
}