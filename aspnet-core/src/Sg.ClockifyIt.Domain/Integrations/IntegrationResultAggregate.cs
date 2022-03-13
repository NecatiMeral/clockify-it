using System.Collections.Generic;
using Sg.ClockifyIt.Integrations.Completion;
using Volo.Abp;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationResultAggregate : Dictionary<string, TimeEntryResultAggregate>
    {
        public List<ICompletionAction> CompletionActions { get; }

        public IntegrationResultAggregate()
        {
            CompletionActions = new List<ICompletionAction>();
        }

        public void Set(string id, TimeEntryResult timeEntryResult)
        {
            Check.NotNull(timeEntryResult, nameof(timeEntryResult));

            if (!TryGetValue(id, out var aggregate))
            {
                aggregate = new TimeEntryResultAggregate();
                Add(id, aggregate);
            }

            aggregate.Succeed &= timeEntryResult.Succeed;
            if (timeEntryResult.Exception != null)
            {
                aggregate.Exceptions.Add(timeEntryResult.Exception);
            }
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
