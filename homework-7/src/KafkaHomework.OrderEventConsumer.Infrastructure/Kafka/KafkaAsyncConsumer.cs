using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaHomework.OrderEventConsumer.Domain.Common;
using Microsoft.Extensions.Options;
using Polly;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;

public sealed class KafkaAsyncConsumer<TKey, TValue> : IDisposable
{
    private readonly int _channelCapacity;
    private readonly TimeSpan _bufferDelay;

    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;

    private readonly ILogger<KafkaAsyncConsumer<TKey, TValue>> _logger;

    public KafkaAsyncConsumer(
        IHandler<TKey, TValue> handler,
        IOptions<KafkaOptions> options,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer,
        ILogger<KafkaAsyncConsumer<TKey, TValue>> logger)
    {
        var kafkaOptions = options.Value;

        _channelCapacity = kafkaOptions.ChannelCapacity;
        _bufferDelay = TimeSpan.FromSeconds(kafkaOptions.BufferDelayInSeconds);

        var builder = new ConsumerBuilder<TKey, TValue>(
            new ConsumerConfig
            {
                BootstrapServers = kafkaOptions.BootstrapServers,
                GroupId = kafkaOptions.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            });

        if (keyDeserializer is not null)
        {
            builder.SetKeyDeserializer(keyDeserializer);
        }

        if (valueDeserializer is not null)
        {
            builder.SetValueDeserializer(valueDeserializer);
        }

        _handler = handler;
        _logger = logger;

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(_channelCapacity)
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        _consumer = builder.Build();
        _consumer.Subscribe(kafkaOptions.Topic);
    }


    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }

    private async Task HandleCore(CancellationToken token)
    {
        await Task.Yield();

        var retryPolicy = Policy
            .Handle<Exception>() 
            .WaitAndRetryAsync(3, 
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timeSpan, attempt, context) =>
                {
                    _logger.LogWarning($"Retrying due to: {exception.Message}. Waiting {timeSpan} before next attempt.");
                });

        await foreach (var consumeResults in _channel.Reader
                           .ReadAllAsync(token)
                           .Buffer(_channelCapacity, _bufferDelay)
                           .WithCancellation(token))
        {
            token.ThrowIfCancellationRequested();

            while (true)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        await _handler.Handle(consumeResults, token);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred after retries.");
                    break;
                }

                var partitionLastOffsets = consumeResults
                    .GroupBy(
                        r => r.Partition.Value,
                        (_, f) => f.MaxBy(p => p.Offset.Value));

                foreach (var partitionLastOffset in partitionLastOffsets)
                {
                    _consumer.StoreOffset(partitionLastOffset);
                }

                break;
            }
        }
    }

    private async Task ConsumeCore(CancellationToken token)
    {
        await Task.Yield();

        while (_consumer.Consume(token) is { } result)
        {
            await _channel.Writer.WriteAsync(result, token);
            _logger.LogTrace(
                "{Partition}:{Offset}:WriteToChannel",
                result.Partition.Value,
                result.Offset.Value);
        }

        _channel.Writer.Complete();
    }

    public void Dispose() => _consumer.Close();
}
