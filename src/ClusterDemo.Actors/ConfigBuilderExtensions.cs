﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using AkkaLogLevel = Akka.Event.LogLevel;

namespace ClusterDemo.Actors
{
    /// <summary>
    ///		Extension methods for <see cref="ConfigBuilder"/>.
    /// </summary>
    public static class ConfigBuilderExtensions
    {
        /// <summary>
        ///		Set the system log level.
        /// </summary>
        /// <param name="configBuilder">
        ///		The configuration builder.
        /// </param>
        /// <param name="level">
        ///		The target log level.
        /// </param>
        /// <returns>
        ///		The configuration builder (enables method chaining).
        /// </returns>
        public static ConfigBuilder SetLogLevel(this ConfigBuilder configBuilder, AkkaLogLevel level)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            string logLevelValue;
            switch (level)
            {
                case AkkaLogLevel.DebugLevel:
                {
                    logLevelValue = "debug";

                    break;
                }
                case AkkaLogLevel.InfoLevel:
                {
                    logLevelValue = "info";

                    break;
                }
                case AkkaLogLevel.WarningLevel:
                {
                    logLevelValue = "warning";

                    break;
                }
                case AkkaLogLevel.ErrorLevel:
                {
                    logLevelValue = "error";

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(level), level, "Invalid log level.");
                }
            }

            configBuilder.Entries["akka.loglevel"] = logLevelValue;

            return configBuilder;
        }

        /// <summary>
        ///		Add a logger to the configuration.
        /// </summary>
        /// <typeparam name="TLogger">
        ///		The type of logger to add.
        /// </typeparam>
        /// <param name="configBuilder">
        ///		The configuration builder.
        /// </param>
        /// <returns>
        ///		The configuration builder (enables method chaining).
        /// </returns>
        public static ConfigBuilder AddLogger<TLogger>(this ConfigBuilder configBuilder)
            where TLogger : ActorBase
        {
            return configBuilder.AddLogger(typeof(TLogger));
        }

        /// <summary>
        ///		Add a logger to the configuration.
        /// </summary>
        /// <param name="configBuilder">
        ///		The configuration builder.
        /// </param>
        /// <param name="loggerType">
        ///		The type of logger to add.
        /// </param>
        /// <returns>
        ///		The configuration builder (enables method chaining).
        /// </returns>
        public static ConfigBuilder AddLogger(this ConfigBuilder configBuilder, Type loggerType)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            if (loggerType == null)
                throw new ArgumentNullException(nameof(loggerType));

            object value;
            List<string> loggerTypes;
            if (!configBuilder.Entries.TryGetValue("akka.loggers", out value) || (loggerTypes = value as List<string>) == null)
                configBuilder.Entries["akka.loggers"] = loggerTypes = new List<string>();

            string loggerTypeName = $"{loggerType.FullName}, {loggerType.Assembly.GetName().Name}";
            if (!loggerTypes.Contains(loggerTypeName))
                loggerTypes.Add(loggerTypeName);

            return configBuilder;
        }

        public static ConfigBuilder SuppressJsonSerializerWarning(this ConfigBuilder configBuilder)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.suppress-json-serializer-warning"] = "on";

            return configBuilder;
        }

        public static ConfigBuilder UseRemoting(this ConfigBuilder configBuilder, string hostName, int port)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.remote.helios.tcp.hostname"] = hostName;
            configBuilder.Entries["akka.remote.helios.tcp.port"] = port;

            return configBuilder;
        }

        public static ConfigBuilder UseClusterClient(this ConfigBuilder configBuilder, IEnumerable<Address> clusterContactNodes)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.cluster.client.initial-contacts"] = new List<string>(
                clusterContactNodes.Select(
                    clusterContactNode => clusterContactNode.ToString() + "/system/receptionist"
                )
            );

            return configBuilder;
        }

        public static ConfigBuilder UseClusterClient(this ConfigBuilder configBuilder, IEnumerable<string> clusterContactNodes)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.cluster.client.initial-contacts"] = new List<string>(clusterContactNodes);

            return configBuilder;
        }

        public static ConfigBuilder UseCluster(this ConfigBuilder configBuilder, IEnumerable<Address> seedNodes, int? minNumberOfMembers = null, TimeSpan? autoDownUnreachableMembersAfter = null)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            if (seedNodes != null)
            {
                configBuilder.Entries["akka.cluster.seed-nodes"] = new List<string>(
                    seedNodes.Select(
                        seedNodeAddress => seedNodeAddress.ToString()
                    )
                );
            }

            if (minNumberOfMembers != null)
                configBuilder.Entries["akka.cluster.min-nr-of-members"] = minNumberOfMembers;

            if (autoDownUnreachableMembersAfter != null)
            {
                configBuilder.Entries["akka.cluster.auto-down-unreachable-after"] = String.Format("{0}s",
                    (long)autoDownUnreachableMembersAfter.Value.TotalSeconds
                );
            }

            return configBuilder.UseClusterActorRefProvider();
        }

        public static ConfigBuilder UseCluster(this ConfigBuilder configBuilder, IEnumerable<string> seedNodes = null, int? minNumberOfMembers = null, TimeSpan? autoDownUnreachableMembersAfter = null)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            if (seedNodes != null)
                configBuilder.Entries["akka.cluster.seed-nodes"] = new List<string>(seedNodes);

            if (minNumberOfMembers != null)
                configBuilder.Entries["akka.cluster.min-nr-of-members"] = minNumberOfMembers;

            if (autoDownUnreachableMembersAfter != null)
            {
                configBuilder.Entries["akka.cluster.auto-down-unreachable-after"] = String.Format("{0}s",
                    (long)autoDownUnreachableMembersAfter.Value.TotalSeconds
                );
            }

            return configBuilder.UseClusterActorRefProvider();
        }

        public static ConfigBuilder UseClusterActorRefProvider(this ConfigBuilder configBuilder)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));
            
            return configBuilder.UseActorRefProvider("Akka.Cluster.ClusterActorRefProvider, Akka.Cluster");
        }

        public static ConfigBuilder UseRemoteActorRefProvider(this ConfigBuilder configBuilder)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            return configBuilder.UseActorRefProvider("Akka.Remote.RemoteActorRefProvider, Akka.Remote");
        }

        public static ConfigBuilder UseActorRefProvider<TProvider>(this ConfigBuilder configBuilder)
            where TProvider : IActorRefProvider
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            Type providerType = typeof(TProvider);

            return configBuilder.UseActorRefProvider(String.Format("{0}, {1}",
                providerType.FullName,
                providerType.Assembly.GetName().Name
            ));
        }

        public static ConfigBuilder UseActorRefProvider(this ConfigBuilder configBuilder, string providerType)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.actor.provider"] = providerType;

            return configBuilder;
        }

        public static ConfigBuilder UseHyperionSerializer(this ConfigBuilder configBuilder)
        {
            if (configBuilder == null)
                throw new ArgumentNullException(nameof(configBuilder));

            configBuilder.Entries["akka.actor.serializers.wire"] = "Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion";
            configBuilder.Entries["akka.actor.serialization-bindings.\"System.Object\""] = "wire";

            return configBuilder;
        }
    }
}
