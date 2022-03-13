using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockify.Net;
using Microsoft.Extensions.Caching.Distributed;
using Sg.ClockifyIt.Integrations.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Sg.ClockifyIt.Clockify.Tags
{
    using ClockifyTag = global::Clockify.Net.Models.Tags.TagDto;
    using TagRequest = global::Clockify.Net.Models.Tags.TagRequest;

    public class ClockifyTagProvider : IClockifyTagProvider, ITransientDependency
    {
        protected IDistributedCache<WorkspaceTagsDto, string> Cache { get; }
        protected IObjectMapper ObjectMapper { get; }

        public ClockifyTagProvider(IDistributedCache<WorkspaceTagsDto, string> cache,
            IObjectMapper objectMapper)
        {
            Cache = cache;
            ObjectMapper = objectMapper;
        }

        public async Task<TagDto> GetOrCreateTagByName(ClockifyClient client, string workspaceId, string name)
        {
            var knownWorkspaceTags = await GetClockifyTagsAsync(client, workspaceId);
            var tag = knownWorkspaceTags.Tags.FirstOrDefault(x => x.Name == name);

            if (tag == null)
            {
                tag = await GetOrCreateTagFromApiAsync(client, workspaceId, name);

                knownWorkspaceTags.Tags.Add(tag);
                await Cache.SetAsync(workspaceId, knownWorkspaceTags, GetDistributedCacheEntryOptions());
            }

            return tag;
        }

        protected virtual async Task<WorkspaceTagsDto> GetClockifyTagsAsync(ClockifyClient client, string workspaceId)
        {
            var cacheItem = await Cache.GetAsync(workspaceId);
            if (cacheItem == null)
            {
                var tags = (await client.FindAllTagsOnWorkspaceAsync(workspaceId)).Data;
                var tagDtos = ObjectMapper.Map<List<ClockifyTag>, List<TagDto>>(tags);

                cacheItem = new WorkspaceTagsDto
                {
                    Tags = tagDtos
                };

                await Cache.SetAsync(workspaceId, cacheItem, GetDistributedCacheEntryOptions());
            }
            return cacheItem;
        }

        protected virtual DistributedCacheEntryOptions GetDistributedCacheEntryOptions()
        {
            return new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
        }

        protected virtual async Task<TagDto> GetOrCreateTagFromApiAsync(ClockifyClient client, string workspaceId, string name)
        {
            var tags = await client.FindAllTagsOnWorkspaceAsync(workspaceId);
            var tag = tags.Data.FirstOrDefault(x => x.Name == name);

            if (tag == null)
            {
                var tagResponse = await client.CreateTagAsync(workspaceId, new TagRequest
                {
                    Name = name
                });
                tag = tagResponse.Data;
            }

            return ObjectMapper.Map<ClockifyTag, TagDto>(tag);
        }

        protected virtual string CalculateCacheKey(string workspaceId, string name)
        {
            return $"{workspaceId}-{name}";
        }

        public class WorkspaceTagsDto
        {
            public List<TagDto> Tags { get; set; }

            public WorkspaceTagsDto()
            {
                Tags = new List<TagDto>();
            }
        }
    }
}
