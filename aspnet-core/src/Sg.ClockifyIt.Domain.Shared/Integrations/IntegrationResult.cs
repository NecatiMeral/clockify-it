using System;
using System.Collections.Generic;
using Sg.ClockifyIt.Integrations.Completion;
using Volo.Abp;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationResult : Dictionary<string, TimeEntryResult>
    {
        public List<ICompletionAction> CompletionActions { get; }

        public IntegrationResult()
        {
            CompletionActions = new List<ICompletionAction>();
        }

        public void MarkAsProcessed(string id)
        {
            this[id] = new TimeEntryResult();
        }

        public void MarkAsFailed(string id, Exception exception)
        {
            this[id] = new TimeEntryResult(exception);
        }

        public void AddAction(ICompletionAction completionAction)
        {
            Check.NotNull(completionAction, nameof(completionAction));

            CompletionActions.Add(completionAction);
        }

        public void AddActions(ICollection<ICompletionAction> completionActions)
        {
            Check.NotNullOrEmpty(completionActions, nameof(completionActions));

            CompletionActions.AddRange(completionActions);
        }
    }
}
