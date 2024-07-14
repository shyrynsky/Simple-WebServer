Http Server + Reverse Proxy
===========================

A simple C# HTTP server that handles basic web requests and serves static content. The server is designed to process HTTP requests, manage routing, and deliver HTML, CSS and images to clients.

If your resource already has a web server (e.g., Kestrel), you might prefer using reverse proxy mode. This mode will redirect all TCP traffic from one endpoint to another IP/port destination.

Usage
-----

Configure the server in the config.json. For example

    {
    "ServAddress": "127.0.0.1",
    "ServPort": 80,
    "IsReverseProxy": false,
    "RootPath": "D:/HttpServ/HttpServ/wwwroot"
    }

Reverse proxy mode:

    {
    "ServAddress": "127.0.0.1",
    "ServPort": 80,
    "IsReverseProxy": true,
    "EndPointAddress": "127.0.0.1",
    "EndPointPort": 5000
    }
