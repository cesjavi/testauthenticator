using System.Collections.Concurrent;
using System.Linq;
using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class PushChallengeService
{
    private readonly ConcurrentDictionary<string, PushChallengeState> _challenges = new();
    private readonly TimeSpan _ttl;

    public PushChallengeService()
        : this(TimeSpan.FromMinutes(2))
    {
    }

    public PushChallengeService(TimeSpan ttl)
    {
        if (ttl <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ttl));
        }

        _ttl = ttl;
    }

    public PushChallengeContext CreateChallenge(User user, IReadOnlyList<PushDevice> devices)
    {
        if (devices.Count == 0)
        {
            throw new ArgumentException("At least one device is required", nameof(devices));
        }

        var now = DateTimeOffset.UtcNow;
        var state = new PushChallengeState(
            Guid.NewGuid().ToString("N"),
            user.Username,
            now,
            now.Add(_ttl),
            devices);

        _challenges[state.Id] = state;
        return new PushChallengeContext(state.ToChallenge(), devices);
    }

    public bool TryGetChallenge(string challengeId, out PushChallenge? challenge)
    {
        if (_challenges.TryGetValue(challengeId, out var state))
        {
            if (state.IsExpired)
            {
                _challenges.TryRemove(challengeId, out _);
                challenge = state.ToChallenge();
                return false;
            }

            challenge = state.ToChallenge();
            return true;
        }

        challenge = null;
        return false;
    }

    public void CancelChallenge(string challengeId)
    {
        _challenges.TryRemove(challengeId, out _);
    }

    public PushChallengeCompletionStatus CompleteChallenge(string challengeId, string deviceId, out PushChallenge? challenge)
    {
        if (!_challenges.TryGetValue(challengeId, out var state))
        {
            challenge = null;
            return PushChallengeCompletionStatus.NotFound;
        }

        if (state.IsExpired)
        {
            _challenges.TryRemove(challengeId, out _);
            challenge = state.ToChallenge();
            return PushChallengeCompletionStatus.Expired;
        }

        if (!state.ContainsDevice(deviceId))
        {
            challenge = state.ToChallenge();
            return PushChallengeCompletionStatus.InvalidDevice;
        }

        if (!state.TryApprove(deviceId))
        {
            if (state.IsExpired)
            {
                _challenges.TryRemove(challengeId, out _);
                challenge = state.ToChallenge();
                return PushChallengeCompletionStatus.Expired;
            }

            if (state.IsApproved)
            {
                _challenges.TryRemove(challengeId, out _);
                challenge = state.ToChallenge();
                return PushChallengeCompletionStatus.NotFound;
            }

            challenge = state.ToChallenge();
            return PushChallengeCompletionStatus.InvalidDevice;
        }

        _challenges.TryRemove(challengeId, out _);
        challenge = state.ToChallenge();
        return PushChallengeCompletionStatus.Approved;
    }

    private sealed class PushChallengeState
    {
        private readonly object _sync = new();

        public PushChallengeState(string id, string username, DateTimeOffset createdAt, DateTimeOffset expiresAt, IReadOnlyList<PushDevice> devices)
        {
            Id = id;
            Username = username;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            Devices = devices;
        }

        public string Id { get; }
        public string Username { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset ExpiresAt { get; }
        public IReadOnlyList<PushDevice> Devices { get; }
        public bool IsApproved { get; private set; }
        public string? ApprovedDeviceId { get; private set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

        public bool ContainsDevice(string deviceId) => Devices.Any(d => string.Equals(d.DeviceId, deviceId, StringComparison.Ordinal));

        public bool TryApprove(string deviceId)
        {
            lock (_sync)
            {
                if (IsApproved || IsExpired)
                {
                    return false;
                }

                if (!ContainsDevice(deviceId))
                {
                    return false;
                }

                IsApproved = true;
                ApprovedDeviceId = deviceId;
                return true;
            }
        }

        public PushChallenge ToChallenge()
        {
            var targets = Devices
                .Select(d => new PushChallengeTarget(d.DeviceId, d.DeviceName))
                .ToArray();

            return new PushChallenge(Id, Username, CreatedAt, ExpiresAt, targets, ApprovedDeviceId, IsApproved);
        }
    }
}

public record PushChallengeContext(PushChallenge Challenge, IReadOnlyList<PushDevice> Devices);
