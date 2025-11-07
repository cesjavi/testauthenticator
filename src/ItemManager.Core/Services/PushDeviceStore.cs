using System.Collections.Concurrent;
using System.Linq;
using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class PushDeviceStore
{
    private readonly ConcurrentDictionary<string, List<PushDevice>> _devices = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<PushDevice> GetDevices(string username)
    {
        if (_devices.TryGetValue(username, out var list))
        {
            lock (list)
            {
                return list.ToList();
            }
        }

        return Array.Empty<PushDevice>();
    }

    public PushDevice RegisterDevice(string username, string deviceName, string registrationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(deviceName))
        {
            throw new ArgumentException("Device name is required", nameof(deviceName));
        }

        if (string.IsNullOrWhiteSpace(registrationToken))
        {
            throw new ArgumentException("Registration token is required", nameof(registrationToken));
        }

        var normalizedToken = registrationToken.Trim();
        var devices = _devices.GetOrAdd(username, _ => new List<PushDevice>());
        lock (devices)
        {
            var existingByToken = devices.FirstOrDefault(d => string.Equals(d.RegistrationToken, normalizedToken, StringComparison.Ordinal));
            if (existingByToken is not null)
            {
                var updated = existingByToken with
                {
                    DeviceName = deviceName.Trim(),
                    RegisteredAt = DateTimeOffset.UtcNow
                };
                var index = devices.IndexOf(existingByToken);
                devices[index] = updated;
                return updated;
            }

            var device = new PushDevice(Guid.NewGuid().ToString("N"), deviceName.Trim(), normalizedToken, DateTimeOffset.UtcNow);
            devices.Add(device);
            return device;
        }
    }
}
