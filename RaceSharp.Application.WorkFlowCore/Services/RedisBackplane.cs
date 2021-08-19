﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RaceSharp.Application.WorkFlowCore
{
    public class RedisBackplane : IClusterBackplane
    {
        private readonly Guid _nodeId = Guid.NewGuid();
        private readonly ILogger _logger;
        private readonly string _connectionString;
        private readonly string _channel;
        private IConnectionMultiplexer _multiplexer;
        private ISubscriber _subscriber;
        private readonly IDefinitionRepository _repository;
        private readonly IWorkflowLoader _loader;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        public RedisBackplane(string connectionString, string channel, IDefinitionRepository repository, IWorkflowLoader loader, ILoggerFactory logFactory)
        {
            _connectionString = connectionString;
            _channel = channel;
            _repository = repository;
            _loader = loader;
            _logger = logFactory.CreateLogger(GetType());
        }

        public async Task Start()
        {
            _multiplexer = await ConnectionMultiplexer.ConnectAsync(_connectionString);
            _subscriber = _multiplexer.GetSubscriber();
            _subscriber.Subscribe(_channel, (channel, message) => {
                var evt = JsonConvert.DeserializeObject(message, _serializerSettings);
                //TODO: split out future commands
                if (evt is NewDefinitionCommand)
                {
                    try
                    {
                        if ((evt as NewDefinitionCommand).Originator == _nodeId)
                            return;
                        var def = _repository.Find((evt as NewDefinitionCommand).DefinitionId, (evt as NewDefinitionCommand).Version);
                        _loader.LoadDefinition(def);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(default(EventId), ex, ex.Message);
                    }

                }
            });
        }

        public async Task Stop()
        {
            await _subscriber.UnsubscribeAllAsync();
            await _multiplexer.CloseAsync();
            _subscriber = null;
            _multiplexer = null;
        }

        public async void LoadNewDefinition(string id, int version)
        {
            var data = JsonConvert.SerializeObject(new NewDefinitionCommand()
            {
                Originator = _nodeId,
                DefinitionId = id,
                Version = version
            }, _serializerSettings);
            await _subscriber.PublishAsync(_channel, data);
        }
    }
}
