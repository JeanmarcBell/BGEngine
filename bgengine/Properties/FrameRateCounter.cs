using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BGEngine
{
    public class FrameRateCounter
    {
        public double ForgettingFactor { get; set; }
        public double AverageFrameRate { get; private set; }

        public FrameRateCounter(double forgettingFactor)
        {
            ForgettingFactor = forgettingFactor;
            AverageFrameRate = 0.0;
        }

        public void Update(double time)
        {
            if (AverageFrameRate == 0.0)
            { 
                AverageFrameRate = 1/time;
            }
            else
            {
                AverageFrameRate = (AverageFrameRate * (1 - ForgettingFactor) + (1/time) * ForgettingFactor);
            }
        }
    }

    public class FrameTimeCounter
    {
        public double ForgettingFactor { get; set; }
        public double AverageFrameTime { get; private set; }

        public FrameTimeCounter(double forgettingFactor)
        {
            ForgettingFactor = forgettingFactor;
            AverageFrameTime = 0.0;
        }

        public void Update(double timeInMs)
        {
            if (AverageFrameTime == 0.0)
            {
                AverageFrameTime =  timeInMs;
            }
            else
            {
                AverageFrameTime = (AverageFrameTime * (1 - ForgettingFactor) + (timeInMs) * ForgettingFactor);
            }
        }
    }
}
