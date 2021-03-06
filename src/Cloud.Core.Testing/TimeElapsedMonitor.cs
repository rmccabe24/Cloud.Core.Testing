﻿namespace Cloud.Core.Testing
{
    using Xunit.Sdk;
    using System;
    using System.Reflection;
    using System.Diagnostics;

    /// <summary>
    /// Attribute for logging execution time to the Console and Debug output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LogExecutionTime : BeforeAfterTestAttribute
    {
        private readonly TimeElapsedMonitor _monitor = new TimeElapsedMonitor(false);

        /// <summary>
        /// Called before a test is executed.
        /// </summary>
        /// <param name="methodUnderTest"></param>
        public override void Before(MethodInfo methodUnderTest)
        {
            _monitor.Start();
            WriteLog($"{methodUnderTest.Name}: Started: {_monitor.StartTime?.ToString("hh:mm:ss.fff")}");
        }

        /// <summary>
        /// Called after a test is executed.
        /// </summary>
        /// <param name="methodUnderTest"></param>
        public override void After(MethodInfo methodUnderTest)
        {
            _monitor.Stop();
            WriteLog($"{methodUnderTest.Name}: Stopped: {_monitor.StopTime?.ToString("hh:mm:ss.fff")} (duration: {_monitor.ElapsedString})");
        }

        /// <summary>
        /// Writes the log message to console and debug loggers.
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteLog(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }

    /// <summary>
    /// Simple class to measure time elapsed.  Can be used with a using statement without any extra code (auto starts and stops).
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TimeElapsedMonitor : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// Gets the stop time.
        /// </summary>
        /// <value>The stop time.</value>
        public DateTime? StopTime { get; private set; }

        /// <summary>
        /// Gets the amount of elapsed time as a Timespan.
        /// </summary>
        /// <value>The elapsed timespan.</value>
        public TimeSpan Elapsed => _stopwatch.Elapsed;

        /// <summary>
        /// Gets the elapsed time as a string.
        /// </summary>
        /// <value>The elapsed time string.</value>
        public string ElapsedString => $"Time elapsed: {Elapsed:hh\\:mm\\:ss\\.fff}";

        /// <summary>
        /// Gets whether the elapsed time is being measured [true] or not [false].
        /// </summary>
        /// <value>Is currently running.</value>
        public bool IsRunning => _stopwatch.IsRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeElapsedMonitor"/> class.
        /// </summary>
        /// <param name="autoStart">if set to <c>true</c> [autoStart].</param>
        public TimeElapsedMonitor(bool autoStart = true)
        {
            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// Starts measuring time elapsed.
        /// </summary>
        public void Start()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
                StartTime = DateTime.Now;
                StopTime = null;
            }
        }

        /// <summary>
        /// Stops measuring time elapsed.
        /// </summary>
        public void Stop()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                StopTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Resets the time elapsed.
        /// </summary>
        public void Reset()
        {
            Stop();
            _stopwatch.Reset();
            StartTime = null;
            StopTime = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Stops the elapsed time stopwatch.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
