﻿namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Utils;

    [Serializable]
    public class StoryData : IStoryData
    {
        private readonly Dictionary<string, object> entries = new Dictionary<string, object>();

        public StoryData(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");
            this.Story = story;
        }

        protected IStory Story { get; private set; }

        public virtual object this[string key]
        {
            get
            {
                Ensure.ArgumentNotEmpty(key, "key");

                object result;
                return this.entries.TryGetValue(key, out result) ? result : null;
            }

            set
            {
                Ensure.ArgumentNotEmpty(key, "key");

                this.entries[key] = value;
                this.Story.Log.Debug("Added key '{0}' to data.", key);
            }
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.entries.GetEnumerator();
        }
    }
}
