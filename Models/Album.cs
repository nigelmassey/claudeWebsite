namespace claudeWebsite.Models;

public record Album(string Id, string Name, DateTime CreatedAt, List<string> PhotoFilenames);
