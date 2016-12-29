using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intertech.Tfs.Common.Models;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;

namespace Intertech.Tfs.Common.Utilities
{
    public class TfsHelper : IDisposable
    {
        private readonly IVssRequestContext _requestContext;
        private readonly CheckinNotification _notification;
        private TfsTeamProjectCollection _tfsTeamProjectCollection;

        public TfsHelper(IVssRequestContext requestContext, CheckinNotification notification)
        {
            _requestContext = requestContext;
            _notification = notification;
        }

        #region WorkItemStore

        public NodeInfo GetIterationInfo(WorkItem workItem)
        {
            Node iterationNode = workItem.Project.FindNodeInSubTree(workItem.IterationId);
            var commonStructureService = (ICommonStructureService)GetTeamProjectCollection().GetService(typeof(ICommonStructureService));
            return commonStructureService.GetNode(iterationNode.Uri.ToString());
        }

        public WorkItem GetWorkItem(int workItemId)
        {
            var workItemStore = GetTeamProjectCollection().GetService<WorkItemStore>();
            return workItemStore.GetWorkItem(workItemId);
        }

        public int GetWorkItemQueryResultCount(string wiq)
        {
            var workItemStore = GetTeamProjectCollection().GetService<WorkItemStore>();
            var query = new Query(workItemStore, ResolveWiqMacros(wiq));
            return query.RunCountQuery();
        }

        #endregion

        #region Version Control

        public List<ChangeSetItemInfo> GetChangeSetChanges(int changeSetId)
        {
            var changeSetItems = new List<ChangeSetItemInfo>();
            var vcs = _requestContext.GetService<TeamFoundationVersionControlService>();
            var changeSet =
                vcs.QueryChangesetExtended(_requestContext, changeSetId, true, false, null, null, null)
                    .Current<Changeset>();
            while (changeSet.Changes.MoveNext())
            {
                var isBranch = false;
                var change = changeSet.Changes.Current;
                var identifier = new ItemIdentifier {DeletionId = change.Item.DeletionId, Item = change.Item.ServerItem};
                var br =
                    vcs.QueryBranchObjects(_requestContext, identifier, RecursionType.None)
                        .Current<StreamingCollection<BranchObject>>();

                if (br != null)
                {
                    while (br.MoveNext())
                    {
                        isBranch = true;
                        break;
                    }
                }

                changeSetItems.Add(new ChangeSetItemInfo
                {
                    ChangeType = change.ChangeType,
                    IsBranch = isBranch,
                    ItemPath = change.Item.ServerItem
                });
            }

            return changeSetItems;
        }

        public IEnumerable<PendingChange> GetPendingChanges()
        {
            var vcs = _requestContext.GetService<TeamFoundationVersionControlService>();
            var submittedItems = _notification.GetSubmittedItems(_requestContext).Select(a => new ItemSpec(a, RecursionType.None)).ToArray();
            return vcs.QueryPendingChangesForWorkspace(_requestContext, _notification.WorkspaceName, _notification.WorkspaceOwner.UniqueName, submittedItems, false, submittedItems.Length, null, true).CurrentEnumerable<PendingChange>();
        }

        #endregion

        #region Identities

        public bool IsInGroup(string groupName, IdentityDescriptor member)
        {
            var ims = _requestContext.GetService<TeamFoundationIdentityService>();
            var group = ims.ReadIdentity(_requestContext, IdentitySearchFactor.DisplayName, groupName);
            return ims.IsMember(_requestContext, group.Descriptor, member);
        }

        #endregion

        #region private methods

        private Uri GetTfsUri()
        {
            var locationService = _requestContext.GetService<ILocationService>();
            return new Uri(locationService.GetServerAccessMapping(_requestContext).AccessPoint + "/" + _requestContext.ServiceHost.Name);
        }

        private TfsTeamProjectCollection GetTeamProjectCollection()
        {
            if (_tfsTeamProjectCollection == null)
            {
                _tfsTeamProjectCollection = new TfsTeamProjectCollection(GetTfsUri());
            }
            return _tfsTeamProjectCollection;
        }

        private string ResolveWiqMacros(string wiq)
        {
            var associatedWorkItems = _notification.NotificationInfo.WorkItemInfo;
            string resolvedWiq = wiq;

            if (Regex.IsMatch(wiq, "@AssociatedWorkItems", RegexOptions.IgnoreCase))
            {
                var workItemIds = associatedWorkItems.AsEnumerable().Select(wiInfo => wiInfo.Id);
                var formattedWorkItemIds = string.Format("({0})", String.Join(",", workItemIds));
                resolvedWiq = Regex.Replace(resolvedWiq, "@AssociatedWorkItems", formattedWorkItemIds, RegexOptions.IgnoreCase);
            }

            if (Regex.IsMatch(wiq, "@Me", RegexOptions.IgnoreCase))
            {
                string userDisplayNameWithQuotes = "'" + GetUserDisplayName() + "'";
                resolvedWiq = Regex.Replace(resolvedWiq, "@Me", userDisplayNameWithQuotes, RegexOptions.IgnoreCase);
            }

            return resolvedWiq;
        }

        private string GetUserDisplayName()
        {
            var ims = _requestContext.GetService<TeamFoundationIdentityService>();
            var user = ims.ReadIdentity(_requestContext, IdentitySearchFactor.AccountName, _requestContext.DomainUserName);
            return user.DisplayName;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_tfsTeamProjectCollection != null)
            {
                _tfsTeamProjectCollection.Dispose();
            }
        }

        #endregion

    }
}
