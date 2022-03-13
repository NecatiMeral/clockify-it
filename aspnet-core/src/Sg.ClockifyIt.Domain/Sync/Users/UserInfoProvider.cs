using System;
using System.Linq;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.Memberships;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Sync.Users
{
    public class UserInfoProvider : IUserInfoProvider, ITransientDependency
    {
        protected IDistributedCache<UserInfo, string> DistributedCache { get; }

        public UserInfoProvider(IDistributedCache<UserInfo, string> distributedCache)
        {
            DistributedCache = distributedCache;
        }

        public async Task<UserInfo> GetCurrentUserInfoAsync(ClockifyClient client, string workspaceName)
        {
            return await DistributedCache.GetOrAddAsync(workspaceName,
                () => GetCurrentUserInfoFromApiAsync(client),
                () => GetDistributedCacheEntryOptions()
            );
        }

        protected DistributedCacheEntryOptions GetDistributedCacheEntryOptions()
        {
            return new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
            };
        }

        private async Task<UserInfo> GetCurrentUserInfoFromApiAsync(ClockifyClient client)
        {
            var user = await client.GetCurrentUserAsync();

            var userInfo = new UserInfo
            {
                Id = user.Data.Id,
                Name = user.Data.Name,
                Email = user.Data.Email
            };

            userInfo.WorkspaceIds.AddRange(
                user.Data.Memberships
                    .Where(x => x.MembershipType == "WORKSPACE" && x.MembershipStatus == MembershipStatus.Active)
                    .Select(x => x.TargetId)
            );
            return userInfo;
        }
    }
}
