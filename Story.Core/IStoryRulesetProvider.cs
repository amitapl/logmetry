namespace Story.Core
{
    public interface IStoryRulesetProvider
    {
        IRuleset<IStory, IStoryHandler> GetRuleset();
    }

    public class BasicStoryRulesetProvider : IStoryRulesetProvider
    {
        private IRuleset<IStory, IStoryHandler> ruleset;

        public BasicStoryRulesetProvider(IRuleset<IStory, IStoryHandler> ruleset)
        {
            this.ruleset = ruleset;
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }
    }
}
