// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Net;
using VRC.OSCQuery;

var rootCommand = new RootCommand
{
    Name = "VrcAdvert",
    Description = "Advertise a VRC OSC service."
};

var nameArg = new Argument<string>("name", "The name of the service.");
var httpPortArg = new Argument<short>("http_port", "The port to use for OSCQuery.");
var oscPortArg = new Argument<short>("osc_port", "The port to use for OSC.");
var trackingOpt = new Option<bool>("--tracking", "Receive tracking data.");

rootCommand.AddArgument(nameArg);
rootCommand.AddArgument(httpPortArg);
rootCommand.AddArgument(oscPortArg);
rootCommand.AddOption(trackingOpt);
rootCommand.SetHandler(ctx =>
{
    var pr = ctx.ParseResult;
    
    var oscQuery = new OSCQueryServiceBuilder()
        .WithServiceName(pr.GetValueForArgument(nameArg))
        .WithHostIP(IPAddress.Loopback)
        .WithTcpPort(pr.GetValueForArgument(httpPortArg))
        .WithUdpPort(pr.GetValueForArgument(oscPortArg))
        .WithDiscovery(new MeaModDiscovery())
        .StartHttpServer()
        .AdvertiseOSC()
        .AdvertiseOSCQuery()
        .Build();
    
    Console.WriteLine($"{oscQuery.ServerName} running at {oscQuery.HostIP} tcp:{oscQuery.TcpPort} osc:{oscQuery.OscPort}");

    oscQuery.RefreshServices();
    oscQuery.AddEndpoint("/avatar/parameters/VSync", "b", Attributes.AccessValues.WriteOnly);

    if (pr.GetValueForOption(trackingOpt))
    {
        oscQuery.AddEndpoint("/tracking/vrsystem/head/pose", "ffffff", Attributes.AccessValues.WriteOnly);
        oscQuery.AddEndpoint("/tracking/vrsystem/leftwrist/pose", "ffffff", Attributes.AccessValues.WriteOnly);
        oscQuery.AddEndpoint("/tracking/vrsystem/rightwrist/pose", "ffffff", Attributes.AccessValues.WriteOnly);
    }

    var knownServices = new HashSet<string>();
    void LogDiscovery(OSCQueryServiceProfile profile)
    {
        if (knownServices.Contains(profile.name))
            return;
    
        Console.WriteLine($"Found service {profile.name} at {profile.address}:{profile.port} with type {profile.serviceType}");
        knownServices.Add(profile.name);
    }

    oscQuery.OnOscQueryServiceAdded += LogDiscovery;
    oscQuery.OnOscServiceAdded += LogDiscovery;

    Console.WriteLine("Ready.");
    
    Thread.Sleep(int.MaxValue);
});

return await rootCommand.InvokeAsync(args);