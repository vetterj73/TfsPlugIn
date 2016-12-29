using System;
using System.Collections.Generic;
using System.Linq;
using Intertech.Configuration;
using Intertech.Configuration.FilterByTeamProject;
using Intertech.Tfs.Common.Utilities;
using Intertech.TFS.RestServiceCaller.Api;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;

namespace NCP.TFS.PlugIns
{
    public class BranchCreated : ISubscriber
    {
        public Type[] SubscribedTypes()
        {
            var eventsToWatchFor = new List<Type> {typeof (CheckinNotification)};
            return eventsToWatchFor.ToArray();
        }

        public EventNotificationStatus ProcessEvent(IVssRequestContext requestContext, NotificationType notificationType,
            object notificationEventArgs, out int statusCode, out string statusMessage,
            out ExceptionPropertyCollection properties)
        {
        
            var response = EventNotificationStatus.ActionPermitted;
            properties = new ExceptionPropertyCollection();
            statusMessage = String.Empty;
            statusCode = 0;

            try
            {
                var notification = notificationEventArgs as CheckinNotification;
                if (notification != null
                    && notificationType == NotificationType.Notification)
                {
                    using (var tfsHelper = new TfsHelper(requestContext, notification))
                    {
                        if (notification.Changeset > 0)
                        {
                            var changeSetItemInfos = tfsHelper.GetChangeSetChanges(notification.Changeset);
                            if (changeSetItemInfos.Exists(csii => csii.IsBranch))
                            {
                                var changeSetItems = changeSetItemInfos.Where(csii => csii.IsBranch).ToList();
                                var filterByTaemProjectsConfigSection =
                                        PluginConfigurationManager.Section.FilterByTeamProjects;
                                var filterByTeamProject = filterByTaemProjectsConfigSection.Enabled;
                                foreach (var changeSetItem in changeSetItems)
                                {
                                    
                                    var allowTeamProjectThrough = false;
                                    if (filterByTaemProjectsConfigSection.Enabled)
                                    {
                                        foreach (FilterByTeamProjectConfigurationElement filterTeamProject in filterByTaemProjectsConfigSection)
                                        {
                                            if (filterTeamProject.Enabled &&
                                                changeSetItem.TeamProjectName == filterTeamProject.TeamProjectName)
                                            {
                                                allowTeamProjectThrough = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!filterByTeamProject || allowTeamProjectThrough)
                                    {
                                        var tfsTemplateList = new TfsServiceCalls();
                                        var configurationTemplateInfo =
                                            tfsTemplateList.FindProgramTypeConfigurationInformation(changeSetItem);
                                        if (changeSetItem.ChangeType == ChangeType.Add ||
                                            changeSetItem.ChangeType == ChangeType.Branch)
                                        {
                                            var buildDefinition = tfsTemplateList.CreateBuildDefinition(changeSetItem, configurationTemplateInfo);
                                            tfsTemplateList.CreateReleaseDefinition(buildDefinition, changeSetItem,
                                                configurationTemplateInfo);
                                        }
                                        else
                                            tfsTemplateList.DeleteBuildAndReleseDefinitions(changeSetItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return response;
            }

            return response;
        }

        public string Name => typeof (BranchCreated).FullName;
        public SubscriberPriority Priority => SubscriberPriority.Normal;
    }
}
