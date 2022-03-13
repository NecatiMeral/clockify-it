using System.Threading.Tasks;
using Clockify.Net;
using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Clockify.Tags
{
    /// <summary>
    /// Gets full tag information
    /// </summary>
    public interface IClockifyTagProvider
    {
        /// <summary>
        /// Gets tag information from clockify or creates it
        /// </summary>
        /// <param name="client">An externally pre-configured client instance to use</param>
        /// <param name="workspaceId">The workspaces Id to get or create the tag in.</param>
        /// <param name="name">The tags name.</param>
        /// <returns>Returns a populated <see cref="TagDto"/> instance.</returns>
        Task<TagDto> GetOrCreateTagByName(ClockifyClient client, string workspaceId, string name);
    }
}
