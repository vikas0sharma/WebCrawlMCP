using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;

namespace WebCrawl;

[McpServerToolType]
public class WebCrawlTool(HttpClient httpClient)
{
    private static readonly Regex ScriptOrStyleRegex = new(@"<(script|style)\b[^>]*>[\s\S]*?</\1>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex TagRegex = new(@"<[^>]+>", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"[ \t]+", RegexOptions.Compiled);
    private static readonly Regex BlankLinesRegex = new(@"\n{3,}", RegexOptions.Compiled);

    [McpServerTool, Description("Crawls a web page at the given URL and returns its text content.")]
    public async Task<string> Crawl([Description("The URL of the web page to crawl.")] string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return "Error: Invalid URL. Please provide an absolute HTTP or HTTPS URL.";
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.UserAgent.ParseAdd("WebCrawlBot/1.0");
            request.Headers.Accept.ParseAdd("text/html, application/xhtml+xml, */*;q=0.8");

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            var text = ExtractTextFromHtml(html);

            return $"URL: {url}\nStatus: {(int)response.StatusCode} {response.StatusCode}\n\n{text}";
        }
        catch (HttpRequestException ex)
        {
            return $"Error crawling {url}: {ex.Message}";
        }
        catch (TaskCanceledException)
        {
            return $"Error crawling {url}: The request timed out.";
        }
    }

    private static string ExtractTextFromHtml(string html)
    {
        // Remove script and style blocks
        var cleaned = ScriptOrStyleRegex.Replace(html, " ");

        // Remove all HTML tags
        cleaned = TagRegex.Replace(cleaned, " ");

        // Decode HTML entities
        cleaned = WebUtility.HtmlDecode(cleaned);

        // Collapse whitespace
        cleaned = WhitespaceRegex.Replace(cleaned, " ");

        // Collapse multiple blank lines
        cleaned = BlankLinesRegex.Replace(cleaned.Trim(), "\n\n");

        return cleaned;
    }
}
