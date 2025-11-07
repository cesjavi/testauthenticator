using ItemManager.Core.Models;

namespace ItemManager.ApiClient;

public record PushDeviceRegistration(string DeviceId, string DeviceName, DateTimeOffset RegisteredAt);

public record PushLoginChallenge(string ChallengeId, DateTimeOffset ExpiresAt, IReadOnlyList<PushChallengeTarget> Devices, string Username, string DisplayName);
