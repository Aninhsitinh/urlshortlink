using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Api.Models;

public interface IShortUrlService
{
    Task<ShortUrl> CreateAsync(string originalUrl, string? userId);
    Task<string?> ResolveAsync(string code);
    Task<IEnumerable<ShortUrl>> GetByUserAsync(string? userId);
}

public class ShortUrlService : IShortUrlService
{
    private readonly ApplicationDbContext _db;
    private readonly IDatabase _cache;

    public ShortUrlService(ApplicationDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _cache = redis.GetDatabase();
    }

    public async Task<ShortUrl> CreateAsync(string originalUrl, string? userId)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            throw new ArgumentException("Original URL cannot be empty.");

        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid URL format.");

        string code = GenerateMeaningfulSlug(originalUrl);

        if (await _db.ShortUrls.AnyAsync(s => s.Code == code))
        {
            code = $"{code}-{Random.Shared.Next(1000, 9999)}";
        }

        var shortUrl = new ShortUrl
        {
            Code = code,
            OriginalUrl = originalUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId,
            HitCount = 0
        };

        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        await _cache.StringSetAsync($"short:{code}", originalUrl, TimeSpan.FromDays(30));

        return shortUrl;
    }

    private string GenerateMeaningfulSlug(string url)
    {
        var uri = new Uri(url);

        var host = uri.Host.ToLower()
            .Replace("www.", "")
            .Replace(".", "-");

        var parts = uri.AbsolutePath
            .Trim('/')
            .ToLower()
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var stopWords = new HashSet<string>
        {
            "the","and","or","of","a","an","to","for","my","in","on","at","is","are","be","with"
        };

        var keywords = parts
            .Select(p => new string(p.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray()))
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Where(p => !stopWords.Contains(p))
            .Take(2)
            .ToList();

        var raw = host + (keywords.Count > 0 ? "-" + string.Join("-", keywords) : "");

        var cleaned = new string(raw
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .ToArray());

        if (string.IsNullOrWhiteSpace(cleaned))
            cleaned = "link-" + GenerateCode(4);

        if (cleaned.Length > 25)
            cleaned = cleaned.Substring(0, 25).Trim('-');

        return cleaned;
    }

    public async Task<string?> ResolveAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var cached = await _cache.StringGetAsync($"short:{code}");
        if (cached.HasValue)
            return cached.ToString();

        var entry = await _db.ShortUrls.FirstOrDefaultAsync(s => s.Code == code);
        if (entry == null)
            return null;

        entry.HitCount++;
        await _db.SaveChangesAsync();

        await _cache.StringSetAsync($"short:{code}", entry.OriginalUrl, TimeSpan.FromDays(30));

        return entry.OriginalUrl;
    }

    public async Task<IEnumerable<ShortUrl>> GetByUserAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
            return Enumerable.Empty<ShortUrl>();

        return await _db.ShortUrls
            .Where(s => s.CreatedByUserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    private static string GenerateCode(int length)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sb = new StringBuilder();
        var bytes = new byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        for (int i = 0; i < length; i++)
            sb.Append(chars[bytes[i] % chars.Length]);

        return sb.ToString();
    }
}
