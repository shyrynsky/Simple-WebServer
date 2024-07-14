using System.Text.RegularExpressions;

namespace HttpServ;

public static partial class HttpHandler
{
    private static readonly Regex RequestTypeRegex = MyRegex();

    public static string GetRequestType(string header, out string path, out string version)
    {
        Match match = RequestTypeRegex.Match(header);
        if (match.Success)
        {
            path = match.Groups[2].Value;
            version = match.Groups[3].Value;
            return match.Groups[1].Value;
        }
        else
        {
            path = "";
            version = "";
            return "";
        }
    }

    public static string GetErrorHeader(string error)
    {
        return $"""
               HTTP/1.1 {error}
               Content-Type: text/html; charset=utf-8
               Content-Length: 0
               Connection: close
               Server: serv/1.0.0

               """;
    }

    [GeneratedRegex(@"^(GET|POST|PUT|DELETE|HEAD|OPTIONS|PATCH|TRACE)\s([^\s]+)\s(HTTP/\d\.\d)$", RegexOptions.IgnoreCase)]
    private static partial Regex MyRegex();
}