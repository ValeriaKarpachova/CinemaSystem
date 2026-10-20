using Cinema.Web.Client;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace Cinema.Web.Client;

public class CustomerSession(ProtectedSessionStorage storage)
{
    private const string Key = "customer-session";
    private bool _loaded;

    private record StoredSession(string Token, string FullName, DateTime ExpiresAtUtc);

    public string? Token { get; private set; }
    public string? FullName { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsAuthenticated => Token is not null && ExpiresAtUtc > DateTime.UtcNow;

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        if (_loaded) return;
        try
        {
            var stored = await storage.GetAsync<StoredSession>(Key);
            if (stored.Success && stored.Value is { } s && s.ExpiresAtUtc > DateTime.UtcNow)
                Apply(s.Token, s.FullName, s.ExpiresAtUtc);

            _loaded = true;
            Changed?.Invoke();
        }
        catch (InvalidOperationException) { /* пререндер, повторим после рендера */ }
        catch (JSDisconnectedException) { }
    }

    public async Task SignInAsync(AuthResponse r)
    {
        Apply(r.Token, r.FullName, r.ExpiresAtUtc);
        _loaded = true;
        await storage.SetAsync(Key, new StoredSession(r.Token, r.FullName, r.ExpiresAtUtc));
        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        Token = null;
        FullName = null;
        ExpiresAtUtc = default;
        try { await storage.DeleteAsync(Key); }
        catch (Exception ex) when (ex is InvalidOperationException or JSDisconnectedException) { }
        Changed?.Invoke();
    }

    private void Apply(string token, string fullName, DateTime expires)
    {
        Token = token;
        FullName = fullName;
        ExpiresAtUtc = expires;
    }
}