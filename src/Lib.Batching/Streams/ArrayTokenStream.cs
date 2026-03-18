using System;
using Xunit;
using Lib.Batching;

namespace Lib.Batching.Tests;

public class ArrayTokenStream : ITokenStream
{
    private readonly int[] _tokens;

    public ArrayTokenStream(int[] tokens)
    {
        _tokens = tokens;
    }

    public int[] GetTokens() => _tokens;
}

public class TokenBatchProviderTests
{
    [Fact]
    public void GetBatch_ReturnsCorrectDimensions()
    {
        int[] dummyTokens = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var stream = new ArrayTokenStream(dummyTokens);
        var provider = new TokenBatchProvider(stream);
        
        int batchSize = 2;
        int blockSize = 3;

        var batch = provider.GetBatch(batchSize, blockSize, new Random(42));

        Assert.Equal(batchSize, batch.Inputs.Length);
        Assert.Equal(batchSize, batch.Targets.Length);
        Assert.Equal(blockSize, batch.Inputs[0].Length);
        Assert.Equal(blockSize, batch.Inputs[1].Length);
    }

    [Fact]
    public void GetBatch_TargetIsImmediatelyAfterBlock()
    {
        int[] dummyTokens = { 10, 20, 30, 40, 50 };
        var stream = new ArrayTokenStream(dummyTokens);
        var provider = new TokenBatchProvider(stream);

        var batch = provider.GetBatch(1, 2, new Random());

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
        var provider = new TokenBatchProvider(stream);

        Assert.Throws<InvalidOperationException>(() => provider.GetBatch(1, 2));
    }
}