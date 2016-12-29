using Microsoft.TeamFoundation.VersionControl.Server;

namespace Intertech.Tfs.Common.Models
{
    public class ChangeSetItemInfo
    {
        public bool IsBranch { get; set; }
        public string ItemPath { get; set; }
        public ChangeType ChangeType { get; set; }

        public string TeamProjectName
        {
            get
            {
                var itemPathParts = GetItemPathParts();

                return itemPathParts?[1];
            }
        }

        public string BranchName
        {
            get
            {
                var itemPathParts = GetItemPathParts();

                return itemPathParts?[itemPathParts.Length - 1];
            }
        }

        public string ProgramName
        {
            get
            {
                var itemPathParts = GetItemPathParts();

                return itemPathParts?[itemPathParts.Length - 3];
            }
        }

        private string[] GetItemPathParts()
        {
            if (string.IsNullOrWhiteSpace(ItemPath) || !ItemPath.StartsWith("$"))
                return null;

            var pathParts = ItemPath.Split('/');
            return pathParts;
        }
    }
}
