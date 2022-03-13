using System.Collections.Generic;
using Sg.ClockifyIt.Integrations.Completion;
using Volo.Abp;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationProcessingResult : Dictionary<string, bool>
    {
        public List<ICompletionAction> CompletionActions { get; }

        public IntegrationProcessingResult()
        {
            CompletionActions = new List<ICompletionAction>();
        }

        public void MarkAsProcessed(string id)
        {
            this[id] = true;
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
