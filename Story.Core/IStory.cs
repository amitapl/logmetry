﻿namespace Story.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IStory
    {
        /// <summary>
        /// Gets the friendly-name of the story.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the instance identifier of the story.
        /// </summary>
        string InstanceId { get; }

        /// <summary>
        /// Starts this story and invokes the start handlers.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this story and invokes the stop handlers.
        /// </summary>
        /// <param name="task">The task wrapped by this story.</param>
        void Stop(Task task);

        /// <summary>
        /// Gets the elapsed time-span.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the start date time.
        /// </summary>
        DateTime StartDateTime { get; }

        /// <summary>
        /// Gets the story data.
        /// </summary>
        IStoryData Data { get; }

        /// <summary>
        /// Gets the story log.
        /// </summary>
        IStoryLog Log { get; }

        /// <summary>
        /// Gets the story parent or null or this is a root story.
        /// </summary>
        IStory Parent { get; }

        IRuleset<IStory, IStoryHandler> HandlerProvider { get; }
    }
}
