using System.Threading.Tasks;
using Clockify.Net;

namespace Sg.ClockifyIt.Sync.Users
{
    public interface IUserInfoProvider
    {
        Task<UserInfo> GetCurrentUserInfoAsync(ClockifyClient client, string workspaceName);
    }
}
