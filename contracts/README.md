# Contracts (Integration Council)

This folder contains interface specs and schemas. The Integration Council owns these.

## Interfaces

Defined in `src/Contracts/Contracts.cs`:

- **ITokenizer**: `VocabSize`, `Encode(string)`, `Decode(ReadOnlySpan<int>)`
- **ILanguageModel**: `ModelKind`, `VocabSize`, `NextTokenScores(ReadOnlySpan<int>)`
- **ITextGenerator**: `Generate(prompt, maxTokens, temperature, topK, seed?)`
- **ICheckpointIO**: `Save(path, Checkpoint)`, `Load(path)`
- **Checkpoint**: `ModelKind`, `TokenizerKind`, `TokenizerPayload`, `ModelPayload`, `Seed`, `ContractFingerprintChain`
- **IContractFingerprint**: `GetContractFingerprint()` — implemented by each Lib.*

## Fingerprint Format

Each library must implement `GetContractFingerprint()` returning a stable string:

```
Lib.<Name>:<Major>.<Minor>.<Patch>:<KeyType1>,<KeyType2>,...
```

Example: `Lib.Corpus:1.0.0:Corpus,CorpusLoader`

**Breaking change** = major bump. No contract change without migration note.

## Checkpoint Schema

See `checkpoint.schema.json` for the checkpoint JSON structure.
