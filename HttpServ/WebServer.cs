using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace HttpServ;

public partial class WebServer
{
    private readonly Config _config;

    private readonly TcpListener _listener;

    public async Task ListenAsync()
    {
        var taskList = new List<Task>();

        while (true)
        {
            TcpClient handler = await _listener.AcceptTcpClientAsync();

            taskList.Add(ProcessRequest(handler));

            for (var i = 0; i < taskList.Count; i++)
            {
                if (taskList[i].IsCompleted)
                {
                    await taskList[i];
                    taskList.RemoveAt(i);
                    i--;
                }
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task ProcessRequest(TcpClient handler)
    {
        string errorHeaderStr = "";
        string path = "";
        await using var stream = handler.GetStream();
        using var reader = new StreamReader(stream);

        if (await reader.ReadLineAsync() is { } requestStr)
        {
            if (HttpHandler.GetRequestType(requestStr, out path, out _) is not ("GET" or "POST" or "HEAD"))
                errorHeaderStr = HttpHandler.GetErrorHeader("405 Method Not Allowed");
        }

        while (await reader.ReadLineAsync() is { } str)
        {
            if (str.Length == 0)
                break;
        }

        if (errorHeaderStr.Length == 0 && !File.Exists(Path.Join(_config.RootPath, path)))
        {
            if (File.Exists(Path.Join(_config.RootPath, path, "Index.html")))
                path = Path.Join(path, "Index.html");
            else
                errorHeaderStr = HttpHandler.GetErrorHeader("404 Not Found");
        }


        await using (var writer = new StreamWriter(stream))
        {
            if (errorHeaderStr.Length > 0)
            {
                await writer.WriteAsync(errorHeaderStr);
            }
            else
            {
                string contentType;
                switch (FileExtRegex().Match(path).Groups[1].Value)
                {
                    case ".jpg" or ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".svg":
                        contentType = "image/svg+xml";
                        break;
                    case ".css":
                        contentType = "text/css";
                        break;
                    default:
                        contentType = "text/html";
                        break;
                }

                writer.AutoFlush = true;
                await using var file = File.OpenRead(Path.Join(_config.RootPath, path));
                await writer.WriteAsync(GetSuccessHeader(file.Length, contentType));
                await file.CopyToAsync(stream);
            }
        }

        handler.Dispose();
    }

    private static string GetSuccessHeader(long contentLength, string contentType = "text/html")
    {
        return $"""
                HTTP/1.1 200 OK
                Content-Type: {contentType}; charset=utf-8
                Content-Length: {contentLength}
                Connection: close
                Server: serv/1.0.0


                """;
    }

    public WebServer(Config config)
    {
        _config = config;
        _listener = new TcpListener(IPAddress.Parse(config.ServAddress), config.ServPort);
        _listener.Start(8);
    }

    [GeneratedRegex(@"(\.\w+)$")]
    private static partial Regex FileExtRegex();
}