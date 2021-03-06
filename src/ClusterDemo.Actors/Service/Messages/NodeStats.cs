﻿using System;

namespace ClusterDemo.Actors.Service.Messages
{
    public class NodeStats
    {
        public NodeStats(string nodeAddress, int totalWorkerCount, int availableWorkerCount, int activeWorkerCount, TimeSpan averageJobExecutionTime, TimeSpan averageJobTurnaroundTime)
        {
            NodeAddress = nodeAddress;
            TotalWorkerCount = totalWorkerCount;
            AvailableWorkerCount = availableWorkerCount;
            ActiveWorkerCount = activeWorkerCount;
            AverageJobExecutionTime = averageJobExecutionTime;
            AverageJobTurnaroundTime = averageJobTurnaroundTime;
        }

        public string NodeAddress { get; }
        public int TotalWorkerCount { get; }
        public int AvailableWorkerCount { get; }
        public int ActiveWorkerCount { get; }
        public TimeSpan AverageJobExecutionTime { get; }
        public TimeSpan AverageJobTurnaroundTime { get; }
    }
}
