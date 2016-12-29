using System.Collections.Generic;
using System.Linq;
using Intertech.Configuration;
using Intertech.Configuration.ProgramTypeTemplate;
using Intertech.Tfs.Common.Models;
using Intertech.Tfs.Common.Utilities;
using Intertech.TFS.RestServiceCaller.Api.RestBuildDefinition;
using Intertech.TFS.RestServiceCaller.Api.RestReleaseDefinition;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;

namespace Intertech.TFS.RestServiceCaller.Api
{
    public class TfsServiceCalls
    {
        #region Release Definition
        public List<ReleaseDefinition> GetReleaseDefinitions()
        {
            var releaseApi = GetReleaseApiCaller();
            return releaseApi.GetReleaseDefinitions();
        }

        public ReleaseDefinition GetReleaseDefinitionById(int definitionId)
        {
            var releaseApi = GetReleaseApiCaller();
            return releaseApi.GetReleaseDefinitionById(definitionId);
        }

        public ReleaseDefinition CreateReleaseDefinition(BuildDefinition buildDefinition, ChangeSetItemInfo itemInfo, ProgramTypeTemplateElement configTemplate)
        {
            var releaseApi = GetReleaseApiCaller();
            var partialRelease = FindReleaseTemplate(configTemplate);
            var release = GetReleaseDefinitionById(partialRelease.Id);
            return releaseApi.CreateReleaseDefinition(release, buildDefinition, itemInfo, configTemplate.ReleaseTemplate);
        }
        #endregion

        #region Build Definition
        public List<BuildDefinitionTemplate> GetBuildTemplateList()
        {
            var buildApi = GetBuildApiCaller();
            return buildApi.GetDefinitionTemplates();
        }

        public BuildDefinition GetBuildDefinitionById(int definitionId)
        {
            var buildApi = GetBuildApiCaller();
            return buildApi.GetBuildDefinitionById(definitionId);
        }

        public void DeleteBuildAndReleseDefinitions(ChangeSetItemInfo itemInfo)
        {
            DeleteBuildDefinition(itemInfo);
            var releaseApi = GetReleaseApiCaller();
            releaseApi.DeleteReleaseDefinition(itemInfo);
        }

        public void DeleteBuildDefinition(ChangeSetItemInfo itemInfo)
        {
            var buildApi = GetBuildApiCaller();
            buildApi.DeleteBuildDefinition(itemInfo);
        }

        public BuildDefinition CreateBuildDefinition(ChangeSetItemInfo itemInfo, ProgramTypeTemplateElement programTypeConfigInfo)
        {
            var buildDefinition = new BuildDefinition();
            var itemTemplate = FindTemplate(programTypeConfigInfo);
            if (itemTemplate != null)
                buildDefinition = CreateBuildDefinition(itemTemplate.Template, itemInfo, programTypeConfigInfo.BuildTemplate);
            return buildDefinition;
        }

        public ProgramTypeTemplateElement FindProgramTypeConfigurationInformation(ChangeSetItemInfo itemInfo)
        {
            var programTypeTemplates = PluginConfigurationManager.Section.ProgramTypeTemplates;
            var itemTemplateName = string.Empty;

            if (!programTypeTemplates.Enabled)
                return null;

            foreach (ProgramTypeTemplateElement ele in programTypeTemplates)
            {
                if (itemInfo.ItemPath.StartsWith(ele.ProgramTypePathStartsWith) && ele.Enabled)
                {
                    return ele;
                }
            }
            return null;
        }

        public BuildDefinitionTemplate FindTemplate(ChangeSetItemInfo itemInfo)
        {
            var configurationElement = FindProgramTypeConfigurationInformation(itemInfo);
            if (configurationElement == null)
                return null;
            return FindTemplate(configurationElement);
        }

        public BuildDefinitionTemplate FindTemplate(ProgramTypeTemplateElement configurationElement)
        {

            var buildDefinitions = GetBuildTemplateList();
            return buildDefinitions.SingleOrDefault(tl => tl.Name == configurationElement.BuildTemplate.TemplateName);
        }

        public ReleaseDefinition FindReleaseTemplate(ProgramTypeTemplateElement configurationElement)
        {
            var releaseDefinitions = GetReleaseDefinitions();
            return releaseDefinitions.SingleOrDefault(rd => rd.Name == configurationElement.ReleaseTemplate.TemplateName);
        }

        //public void CreateBuildDefinition(BuildDefinition template, ChangeSetItemInfo itemInfo)
        public BuildDefinition CreateBuildDefinition(BuildDefinition template, ChangeSetItemInfo itemInfo, BuildTemplateElement buildTemplateInfo)
        {
            var buildApi = GetBuildApiCaller();
            var createBuildDefinition = buildApi.CreateBuildDefinition(template, itemInfo, buildTemplateInfo);
            return createBuildDefinition;
        }
        #endregion

        private ReleaseDefinitionApiCalls GetReleaseApiCaller()
        {
            return new ReleaseDefinitionApiCalls(Constants.TfsBuildBaseRestUrl, Constants.TfsReleaseRestApiVersion);
        }

        private BuildDefinitionApiCalls GetBuildApiCaller()
        {
            return new BuildDefinitionApiCalls(Constants.TfsBuildBaseRestUrl, Constants.TfsBuildRestApiVersion);
        }
    }
}
