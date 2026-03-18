namespace MiniChatGPT.Contracts;

/// <summary>Interface for tokenizers. Implemented by Lib.Tokenization.</summary>
public interface ITokenizer : IContractFingerprint
{
    int VocabSize { get; }
    int[] Encode(string text);
    string Decode(ReadOnlySpan<int> tokens);
    object GetPayloadForCheckpoint();
}

/// <summary>Interface for language models. Implemented by Lib.Models.NGram, TinyNN, TinyTransformer.</summary>
public interface ILanguageModel : IContractFingerprint
{
    string ModelKind { get; }
    int VocabSize { get; }
    float[] NextTokenScores(ReadOnlySpan<int> context);
    object GetPayloadForCheckpoint();
}

/// <summary>Abstraction for text generation. Consumed by Lib.ChatConsole.</summary>
public interface ITextGenerator
{
    string Generate(string prompt, int maxTokens, float temperature, int topK, int? seed = null);
}

/// <summary>Interface for checkpoint save/load. Implemented by components that persist checkpoints.</summary>
public interface ICheckpointIO
{
    void Save(string path, Checkpoint checkpoint);
    Checkpoint Load(string path);
}

/// <summary>Checkpoint structure for model + tokenizer persistence.</summary>
public sealed record Checkpoint(
    string ModelKind,
    string TokenizerKind,
    object TokenizerPayload,
    object ModelPayload,
    int Seed,
    string ContractFingerprintChain
);

/// <summary>Implemented by each Lib.* to return a stable fingerprint of its public surface.</summary>
public interface IContractFingerprint
{
    string GetContractFingerprint();
}
