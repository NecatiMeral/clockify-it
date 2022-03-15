using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations.Redmine
{
    public class ActivityTagDictionary : Dictionary<string, int>
    {
        public const string DefaultName = "Default";

        public int Default
        {
            get => this.GetOrDefault(DefaultName);
            set => this[DefaultName] = value;
        }
    }
}
