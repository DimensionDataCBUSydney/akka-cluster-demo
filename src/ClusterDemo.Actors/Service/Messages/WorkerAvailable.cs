﻿using Akka.Actor;

namespace ClusterDemo.Actors.Service.Messages
{
    public class WorkerAvailable
        : IWorkerEvent
    {
        public WorkerAvailable(IActorRef worker)
        {
            Worker = worker;
        }

        public IActorRef Worker { get; }
    }
}
