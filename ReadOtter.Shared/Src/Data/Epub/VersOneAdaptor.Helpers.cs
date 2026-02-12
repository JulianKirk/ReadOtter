using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Epub;

public partial class VersOneAdaptor
{
    [GeneratedRegex(@"(<img\b[^>]*?\bsrc\s*=\s*"")([^""]+)("")", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ImageSrcRegex();

    [GeneratedRegex(@"(<image\b[^>]*?\bxlink:href\s*=\s*"")([^""]+)("")", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SvgImageHrefRegex();

    static void ValidateFileExists(Book book)
    {
        if (!File.Exists(book.FilePath))
        {
            throw new FileNotFoundException(
                $"EPUB file not found at '{book.FilePath}' for book '{book.Title}' (Id: {book.Id}).",
                book.FilePath);
        }
    }

    string ResolveImagesInHtml(
        string html,
        string chapterFilePath,
        Dictionary<string, (string MimeType, byte[] Bytes)> imageLookup)
    {
        var chapterDir = GetDirectoryPath(chapterFilePath);

        html = ImageSrcRegex().Replace(
            html,
            match => ReplaceImageMatch(match, chapterDir, imageLookup));

        html = SvgImageHrefRegex().Replace(
            html,
            match => ReplaceImageMatch(match, chapterDir, imageLookup));

        return html;
    }

    string ReplaceImageMatch(
        Match match,
        string chapterDir,
        Dictionary<string, (string MimeType, byte[] Bytes)> imageLookup)
    {
        var src = match.Groups[2].Value;

        if (
            src.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || src.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return match.Value;
        }

        var decodedSrc = Uri.UnescapeDataString(src);
        var resolvedPath = ResolveRelativePath(chapterDir, decodedSrc);

        if (imageLookup.TryGetValue(resolvedPath, out var imageData))
        {
            var base64 = Convert.ToBase64String(imageData.Bytes);
            var dataUri = $"data:{imageData.MimeType};base64,{base64}";
            return $"{match.Groups[1].Value}{dataUri}{match.Groups[3].Value}";
        }

        logger.LogError(
            "Unable to resolve image '{Src}' (resolved to '{ResolvedPath}') in chapter '{ChapterDir}'",
            src,
            resolvedPath,
            chapterDir);

        return match.Value;
    }

    static string ResolveRelativePath(string chapterDir, string relativePath)
    {
        if (relativePath.StartsWith("/"))
        {
            return NormalizePath(relativePath);
        }

        var combined = string.IsNullOrEmpty(chapterDir)
            ? relativePath
            : $"{chapterDir}/{relativePath}";

        return NormalizePath(combined);
    }

    static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/').TrimStart('/');

        var segments = path.Split('/');
        var stack = new List<string>();
        foreach (var segment in segments)
        {
            if (segment == "..")
            {
                if (stack.Count > 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                }
            }
            else if (segment != "." && segment.Length > 0)
            {
                stack.Add(segment);
            }
        }

        return string.Join("/", stack);
    }

    static string GetDirectoryPath(string filePath)
    {
        var normalized = filePath.Replace('\\', '/');
        var lastSlash = normalized.LastIndexOf('/');
        return lastSlash >= 0 ? normalized[..lastSlash] : string.Empty;
    }
}
