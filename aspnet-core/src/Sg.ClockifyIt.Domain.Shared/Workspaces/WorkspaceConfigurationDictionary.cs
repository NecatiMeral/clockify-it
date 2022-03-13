using System.Collections.Generic;

namespace Sg.ClockifyIt.Workspaces
{
    public class WorkspaceConfigurationDictionary : Dictionary<string, WorkspaceConfiguration>
    {
        public const string DefaultName = "Default";

        public WorkspaceConfiguration Default
        {
            get => this.GetOrDefault(DefaultName);
            set => this[DefaultName] = value;
        }
    }
}
