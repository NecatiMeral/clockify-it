using System.Collections.Generic;

namespace Sg.ClockifyIt.Sync.Users
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public List<string> WorkspaceIds { get; set; }

        public UserInfo()
        {
            WorkspaceIds = new List<string>();
        }
    }
}
