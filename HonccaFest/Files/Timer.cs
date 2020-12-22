// Creator.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework;
using System;

namespace HonccaFest.Files
{
    class Timer
    {
        public TimeSpan TimerStarted = TimeSpan.Zero;
        public TimeSpan TimerTime;

        public bool SkipFirst;

        /// <summary>
        /// You choose how long a timer should be and can use it to check between times.
        /// </summary>
        /// <param name="timerInMilliseconds">How long the timer should be specified in milliseconds.</param>
        /// <param name="firstTimeNoTimer">If you want the first timer to be skipped.</param>
        public Timer(float timerInMilliseconds, bool firstTimeNoTimer = false)
        {
            TimerTime = TimeSpan.FromMilliseconds(timerInMilliseconds);

            SkipFirst = firstTimeNoTimer;
        }

        /// <summary>
        /// Checks whether the timer is finished or not.
        /// </summary>
        /// <returns>A bool that returns true if the timer is finished.</returns>
        /// <param name="currentGameTime">The current gametime object.</param>
        public bool IsFinished(GameTime currentGameTime)
        {
            if (TimerStarted == TimeSpan.Zero)
            {
                TimerStarted = currentGameTime.TotalGameTime;

                if (SkipFirst)
                    return true;
            }

            return currentGameTime.TotalGameTime > TimerStarted + TimerTime;
        }

        /// <summary>
        /// This will return the time remaining for the current timer.
        /// </summary>
        /// <returns>The time in milliseconds.</returns>
        /// <param name="currentGameTime">The current gametime object.</param>
        public double GetTimeRemaining(GameTime currentGameTime)
        {
            return TimerTime.TotalMilliseconds - (currentGameTime.TotalGameTime - TimerStarted).TotalMilliseconds;
        }

        /// <summary>
        /// Resets the timer and can now be used again.
        /// </summary>
        /// <param name="currentGameTime">The current gametime object.</param>
        public void ResetTimer(GameTime currentGameTime)
        {
            TimerStarted = currentGameTime.TotalGameTime;
        }
    }
}
