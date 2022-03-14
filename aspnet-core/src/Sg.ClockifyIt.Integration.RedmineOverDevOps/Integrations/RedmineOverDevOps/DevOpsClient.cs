using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Sg.ClockifyIt.Integrations.RedmineOverDevOps
{
    public class DevOpsClient
    {
        VssBasicCredential _credentials;
        private VssConnection _connection;

        public string Host { get; }
        public string AccessToken { get; }

        public DevOpsClient(string host, string accessToken)
        {
            Host = host;
            AccessToken = accessToken;
        }

        protected virtual VssConnection GetConnection()
        {
            if (_connection == null)
            {
                _credentials = new VssBasicCredential(string.Empty, AccessToken);
                _connection = new VssConnection(new Uri(Host), _credentials);
            }
            return _connection;
        }

        public async Task<WorkItem> GetWorkItemAsync(int id)
        {
            var connection = GetConnection();

            var witClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>();

            return await witClient.GetWorkItemAsync(id, expand: WorkItemExpand.Relations);
        }
    }
}
