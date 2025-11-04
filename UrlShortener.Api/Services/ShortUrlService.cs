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

    // ✅ TẠO URL RÚT GỌN
    public async Task<ShortUrl> CreateAsync(string originalUrl, string? userId)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            throw new ArgumentException("Original URL cannot be empty.");

        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid URL format.");

        // ✅ Tạo mã base62 6 ký tự, đảm bảo không trùng
        string code;
        do
        {
            code = GenerateCode(6);
        }
        while (await _db.ShortUrls.AnyAsync(s => s.Code == code));

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

        // ✅ Lưu cache Redis 30 ngày
        await _cache.StringSetAsync($"short:{code}", originalUrl, TimeSpan.FromDays(30));

        return shortUrl;
    }

    // ✅ TRUY NGƯỜI ĐIỀU HƯỚNG /r/{code}
    public async Task<string?> ResolveAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        // ✅ Check cache trước
        var cached = await _cache.StringGetAsync($"short:{code}");
        if (cached.HasValue)
            return cached.ToString();

        // ✅ Nếu không có trong cache → tìm trong database
        var entry = await _db.ShortUrls.FirstOrDefaultAsync(s => s.Code == code);
        if (entry == null)
            return null;

        // ✅ Tăng lượt click
        entry.HitCount++;
        await _db.SaveChangesAsync();

        // ✅ Cache lại
        await _cache.StringSetAsync($"short:{code}", entry.OriginalUrl, TimeSpan.FromDays(30));

        return entry.OriginalUrl;
    }

    // ✅ LẤY DANH SÁCH LINK CỦA USER
    public async Task<IEnumerable<ShortUrl>> GetByUserAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
            return Enumerable.Empty<ShortUrl>();

        return await _db.ShortUrls
            .Where(s => s.CreatedByUserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    // ✅ TẠO CODE BASE62
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
