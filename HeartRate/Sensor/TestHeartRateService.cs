﻿using System;
using System.Threading;

namespace HeartRateSensor
{
    internal class TestHeartRateService : IHeartRateService
    {
        private readonly TimeSpan _tickrate;
        public bool IsDisposed { get; private set; }
        public event HeartRateService.HeartRateUpdateEventHandler HeartRateUpdated;

        public readonly int[] HeartRates = new[]
            { 10, 20, 30, 40, 50, 60, 70, 80, 90, 99 };

        private Timer _timer;
        private int _count;
        private readonly object _sync = new object();

        public TestHeartRateService() : this(TimeSpan.FromSeconds(1))
        {
        }

        public TestHeartRateService(TimeSpan tickrate)
        {
            _tickrate = tickrate;
        }

        public void InitiateDefault()
        {
            _timer = new Timer(Timer_Tick, null, _tickrate, _tickrate);
        }

        private void Timer_Tick(object state)
        {
            int count;

            lock (_sync)
            {
                count = _count = ++_count;

                if (count >= HeartRates.Length)
                {
                    return;
                }
            }

            HeartRateUpdated?.Invoke(
                ContactSensorStatus.Contact,
                HeartRates[count]);
        }

        public void Cleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _timer?.Dispose();
        }
    }
}