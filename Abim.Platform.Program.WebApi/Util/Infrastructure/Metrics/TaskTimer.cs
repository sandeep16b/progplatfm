using Abim.Platform.Program.Util.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Util.Testing.Metrics
{
    /// <summary>
    /// A task timer class
    /// </summary>
    public sealed class TaskTimer
    {
        #region Fields

        /// <summary>
        /// The total timers running
        /// </summary>
        private static int totalTimersRunning;

        /// <summary>
        /// The maximum permitted timers
        /// </summary>
        private const int maxPermittedTimers = 10000;

        #endregion

        #region Properties

        /// <summary>
        /// The total times for each task, in milliseconds
        /// </summary>
        private Dictionary<string, List<long>> TaskTimes { get; set; }

        /// <summary>
        /// The timers
        /// </summary>
        private Dictionary<string, Dictionary<string, Stopwatch>> Timers { get; set; }

        /// <summary>
        /// The task parents
        /// </summary>
        private Dictionary<string, string> TaskParents { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTimer"/> class.
        /// </summary>
        public TaskTimer()
        {
            TaskTimes = new Dictionary<string, List<long>>();
            Timers = new Dictionary<string, Dictionary<string, Stopwatch>>();
            TaskParents = new Dictionary<string, string>();
        }

        /// <summary>
        /// Starts a new timer under the given task category, optionally identified with an individual timer id.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="parentTaskName">Name of the parent task, if applicable.</param>
        public void Started(string taskName, string identifier = null, string parentTaskName = null)
        {
            if(!Timers.ContainsKey(taskName))
                Timers[taskName] = new Dictionary<string, Stopwatch>();
            if(parentTaskName != null)
                TaskParents[taskName] = parentTaskName;
            if(identifier == null)
            {
                identifier = Guid.NewGuid().ToString();
                Timers[taskName][identifier] = new Stopwatch();
            }
            else if(!Timers[taskName].ContainsKey(identifier))
                Timers[taskName][identifier] = new Stopwatch();
            if(!Timers[taskName][identifier].IsRunning)
            {
                if(totalTimersRunning < maxPermittedTimers)
                {
                    totalTimersRunning++;
                    Timers[taskName][identifier].Start();
                }
            }
        }

        /// <summary>
        /// Marks a task as having ended and ends the associated timer.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="identifier">The identifier.</param>
        public void Ended(string taskName, string identifier = null)
        {
            if(Timers.ContainsKey(taskName))
            {
                Stopwatch timer = null;
                if(identifier != null && Timers[taskName].ContainsKey(identifier))
                    timer = Timers[taskName][identifier];
                else if(Timers[taskName].Keys.Any())
                    timer = Timers[taskName].First().Value;
                if(timer != null)
                {
                    long milliseconds = timer.ElapsedMilliseconds;
                    timer.Stop();
                    if(!TaskTimes.ContainsKey(taskName))
                        TaskTimes[taskName] = new List<long>();
                    TaskTimes[taskName].Add(milliseconds);
                    totalTimersRunning--;
                }
            }
        }

        /// <summary>
        /// Adds an already-known timing result.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public void AddCompletedTime(string taskName, long milliseconds)
        {
            if(!TaskTimes.ContainsKey(taskName))
                TaskTimes[taskName] = new List<long>();
            TaskTimes[taskName].Add(milliseconds);
        }

        /// <summary>
        /// Watches the task and adds it to the timing logs.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="parentTaskName">Name of the parent task, if applicable.</param>
        public async void WatchTask(Task task, string taskName, string parentTaskName = null)
        {
            if(parentTaskName != null)
                TaskParents[taskName] = parentTaskName;
            if(totalTimersRunning < maxPermittedTimers)
            {
                totalTimersRunning++;
                var taskWatch = new Stopwatch();
                taskWatch.Start();
                try
                {
                    await task;
                }
                catch(Exception)
                {
                    taskWatch.Stop();
                    totalTimersRunning--;
                    return;
                }
                AddCompletedTime(taskName, taskWatch.ElapsedMilliseconds);
                taskWatch.Stop();
                totalTimersRunning--;
            }
        }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        /// <returns></returns>
        public List<TaskMetric> GetMetrics()
        {
            var lookup = new Dictionary<string, TaskMetric>();
            foreach(var kvp in TaskTimes)
            {
                if(kvp.Value.Count == 0) continue;
                List<long> millisecondCounts = kvp.Value;
                long sum = 0;
                foreach(var count in millisecondCounts)
                    sum += count;
                lookup[kvp.Key] = new TaskMetric()
                {
                    TaskName = kvp.Key,
                    AverageMilliseconds = (long)(sum / (double)(millisecondCounts.Count)),
                    TimesRun = millisecondCounts.Count,
                    TotalMilliseconds = sum
                };
            }
            foreach(var kvp in TaskParents)
            {
                if(!lookup.ContainsKey(kvp.Key) || !lookup.ContainsKey(kvp.Value))
                    continue;
                var child = lookup[kvp.Key];
                var parent = lookup[kvp.Value];
                parent.ChildTasks.Add(child);
            }
            var rootTasks = lookup.Values.Where(m => !lookup.Any(kvp => kvp.Value.ChildTasks.Any(t => t.TaskName == m.TaskName))).ToList();
            return rootTasks;
        }

        /// <summary>
        /// Gets metrics results json.
        /// </summary>
        /// <returns></returns>
        public string GetMetricsJson()
        {
            try
            {
                return JsonConvert.SerializeObject(GetMetrics().ToArray());
            }
            catch(Exception ex)
            {
                return "Error calculating metrics: " + ex.Stringify();
            }
        }
    }

    /// <summary>
    /// TaskMetric struct. Returned by the TaskTimer
    /// </summary>
    public class TaskMetric
    {
        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        /// <value>
        /// The name of the task.
        /// </value>
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the average milliseconds.
        /// </summary>
        /// <value>
        /// The average milliseconds.
        /// </value>
        public long AverageMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the times run.
        /// </summary>
        /// <value>
        /// The times run.
        /// </value>
        public int TimesRun { get; set; }

        /// <summary>
        /// Gets or sets the total milliseconds.
        /// </summary>
        /// <value>
        /// The total milliseconds.
        /// </value>
        public long TotalMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the child tasks.
        /// </summary>
        /// <value>
        /// The child tasks.
        /// </value>
        public List<TaskMetric> ChildTasks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMetric" /> class.
        /// </summary>
        public TaskMetric()
        {
            ChildTasks = new List<TaskMetric>();
        }
    }
}
