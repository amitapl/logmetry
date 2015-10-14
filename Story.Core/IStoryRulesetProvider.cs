using Story.Core.Utils;
using System;

namespace Story.Core
{
    public interface IStoryRulesetProvider
    {
        IRuleset<IStory, IStoryHandler> GetRuleset();
    }

    public class BasicStoryRulesetProvider : IStoryRulesetProvider
    {
        private readonly IRuleset<IStory, IStoryHandler> ruleset;

        public BasicStoryRulesetProvider(IRuleset<IStory, IStoryHandler> ruleset)
        {
            this.ruleset = ruleset;
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }
    }

    public class FileBasedStoryRulesetProvider : IStoryRulesetProvider, IDisposable
    {
        private FileWatcher fileWatcher;
        private IRuleset<IStory, IStoryHandler> ruleset;

        public FileBasedStoryRulesetProvider(string path)
        {
            this.fileWatcher = new FileWatcher(path, OnFileChanged);
        }

        private void OnFileChanged(string fileContent)
        {
            /*
            [
                {
                    'ruleType': 'type',
                    'ruleName': 'name'
                }
            ]
            */
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.fileWatcher != null)
                {
                    this.fileWatcher.Dispose();
                    this.fileWatcher = null;
                }
            }
        }
    }
}
