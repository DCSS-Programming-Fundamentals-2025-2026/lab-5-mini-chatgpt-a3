using System;
using Xunit;
using Lib.Batching;
using Lib.Batching.Configuration;
using Lib.Batching.Sampling;
using Lib.Batching.Streams;

namespace Lib.Batching.Tests;

public class TokenBatchProviderTests
{
    [Fact]
    public void GetBatch_ReturnsCorrectDimensions()
    {
        int[] dummyTokens = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var stream = new ArrayTokenStream(dummyTokens);
        var sampler = new BatchWindowSampler();
        var provider = new TokenBatchProvider(stream, sampler);
        var config = new BatchConfig(BatchSize: 2, BlockSize: 3);

        var batch = provider.GetBatch(config, new Random(42));

        Assert.Equal(config.BatchSize, batch.Inputs.Length);
        Assert.Equal(config.BatchSize, batch.Targets.Length);
        Assert.Equal(config.BlockSize, batch.Inputs[0].Length);
        Assert.Equal(config.BlockSize, batch.Inputs[1].Length);
    }

    [Fact]
    public void GetBatch_TargetIsImmediatelyAfterBlock()
    {
        int[] dummyTokens = { 10, 20, 30, 40, 50 };
        var stream = new ArrayTokenStream(dummyTokens);
        var sampler = new BatchWindowSampler();
        var provider = new TokenBatchProvider(stream, sampler);
        var config = new BatchConfig(BatchSize: 1, BlockSize: 2);

        var batch = provider.GetBatch(config, new Random());

        int firstToken = batch.Inputs[0][0];
        int indexInArray = Array.IndexOf(dummyTokens, firstToken);
        
        int expectedTarget = dummyTokens[indexInArray + 2];
        Assert.Equal(expectedTarget, batch.Targets[0]);
    }

    [Fact]
    public void GetBatch_ThrowsException_WhenNotEnoughTokens()
    {
        int[] dummyTokens = { 1, 2 };
        var stream = new ArrayTokenStream(dummyTokens);
        var sampler = new BatchWindowSampler();
        var provider = new TokenBatchProvider(stream, sampler);
        var config = new BatchConfig(BatchSize: 1, BlockSize: 2);

        Assert.Throws<InvalidOperationException>(() => provider.GetBatch(config, new Random()));
    }

    [Fact]
    public void GetBatch_IsDeterministic_WithSameSeed()
    {
        int[] dummyTokens = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var stream = new ArrayTokenStream(dummyTokens);
        var sampler = new BatchWindowSampler();
        var provider = new TokenBatchProvider(stream, sampler);
        var config = new BatchConfig(BatchSize: 2, BlockSize: 3);
        int seed = 1337;

        var batch1 = provider.GetBatch(config, new Random(seed));
        var batch2 = provider.GetBatch(config, new Random(seed));

        Assert.Equal(batch1.Inputs[0], batch2.Inputs[0]);
        Assert.Equal(batch1.Inputs[1], batch2.Inputs[1]);
        Assert.Equal(batch1.Targets, batch2.Targets);
    }
}