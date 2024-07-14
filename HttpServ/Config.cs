namespace HttpServ;

public class Config
{
    public string ServAddress { get; set; } = "127.0.0.1";
    
    public int ServPort { get; set; } = 80;
    
    public bool IsReverseProxy { get; set; }
    
    public string RootPath { get; set; } = "";

    public string EndPointAddress { get; set; } = "127.0.0.1";

    public int EndPointPort { get; set; } = 80;
}