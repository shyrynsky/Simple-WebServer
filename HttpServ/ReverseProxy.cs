using System.Net.Sockets;
using System.Net;

namespace HttpServ;

public class ReverseProxy
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
        using var sender = new TcpClient(_config.EndPointAddress, _config.EndPointPort);
        await using var toClientStream = handler.GetStream();
        await using var toServerStream = sender.GetStream();

        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        
        var toClientTask = toClientStream.CopyToAsync(toServerStream, token);
        var toServerTask = toServerStream.CopyToAsync(toClientStream, token);
        
        await Task.WhenAny(toClientTask, toServerTask);
        
        cancellationTokenSource.Cancel();
        
        handler.Dispose();
    }

    public ReverseProxy(Config config)
    {
        _config = config;
        _listener = new TcpListener(IPAddress.Parse(config.ServAddress), config.ServPort);
        _listener.Start(8);
    }
}