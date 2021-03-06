﻿namespace Story.Core.Handlers
{
    using System;
    using System.Threading.Tasks;

    using Utils;

    [Serializable]
    public class ConsoleHandler : IStoryHandler
    {
        public static readonly IStoryHandler DefaultConsoleHandler = new ConsoleHandler();
        public static readonly IStoryHandler BasicConsoleHandler = new ConsoleHandler(new BasicStoryFormatter());

        private readonly IStoryFormatter storyFormatter;

        public ConsoleHandler(IStoryFormatter storyFormatter = null)
        {
            this.storyFormatter = storyFormatter ?? new DelimiterStoryFormatter(LogSeverity.Debug);
        }

        public void OnStart(IStory story)
        {
        }

        public virtual void OnStop(IStory story, Task task)
        {
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(task, "task");

            string str = this.storyFormatter.FormatStory(story);
            Console.WriteLine(str);
        }
    }
}
