using System.Text.Json;

namespace MiniChatGPT.Contracts;

/// <summary>JSON implementation of ICheckpointIO for saving/loading checkpoints.</summary>
public sealed class JsonCheckpointIO : ICheckpointIO
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void Save(string path, Checkpoint checkpoint)
    {
        var json = JsonSerializer.Serialize(checkpoint, Options);
        File.WriteAllText(path, json);
    }

    public Checkpoint Load(string path)
    {
        var json = File.ReadAllText(path);
        var dto = JsonSerializer.Deserialize<CheckpointDto>(json, Options)
            ?? throw new InvalidOperationException("Failed to deserialize checkpoint");
        return new Checkpoint(
            dto.ModelKind,
            dto.TokenizerKind,
            dto.TokenizerPayload,
            dto.ModelPayload,
            dto.Seed,
            dto.ContractFingerprintChain
        );
    }

    private sealed record CheckpointDto(
        string ModelKind,
        string TokenizerKind,
        JsonElement TokenizerPayload,
        JsonElement ModelPayload,
        int Seed,
        string ContractFingerprintChain
    );
}
