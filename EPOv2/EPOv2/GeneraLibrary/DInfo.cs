using System;
using System.Diagnostics;

namespace EPOv2.GeneraLibrary
{
    public class DInfo
    {
        public Stopwatch Watch = new Stopwatch();
        public void StartWatch()
        {
            Watch.Start();
        }

        public string Stopwatch()
        {
            Watch.Stop();
            var ts = Watch.Elapsed;
            var elapsedTime = String.Format("{0:00}h:{1:00}m:{2:00}s.{3:00}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            return elapsedTime;
        }
    }
}