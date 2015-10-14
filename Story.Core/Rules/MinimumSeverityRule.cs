namespace Story.Core.Rules
{
    using System;
    using System.Linq;

    /// <summary>
    /// Minimum severity rule inclusive.
    /// </summary>
    [Name("minimumSeverity")]
    public class MinimumSeverityRule : PredicateRule
    {
        public MinimumSeverityRule(LogSeverity minimumSeverity, Func<IStory, IStoryHandler> valueFactory)
            : base(story => story.Log.Any(l => l.Severity >= minimumSeverity), valueFactory)
        {
        }
    }
}
