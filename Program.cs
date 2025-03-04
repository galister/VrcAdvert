// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Net;
using VRC.OSCQuery;

var rootCommand = new RootCommand
{
    Name = "VrcAdvert",
    Description = "Advertise your OSC app through OSCQuery.",
};

var nameArg = new Argument<string>(
    "name",
    "The name of your OSC app. Can be anything as long as it's unique"
);
var httpPortArg = new Argument<ushort>(
    "http_port",
    "The port to use for OSCQuery. Can be any free TCP port."
);
var oscPortArg = new Argument<ushort>(
    "osc_port",
    "The port that your OSC app is listening on. VRC will send OSC avatar parameters to this port."
);
var trackingOpt = new Option<bool>(
    "--tracking",
    "Add this if you want to receive /tracking/vrsystem messages."
);

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

    Console.WriteLine(
        $"{oscQuery.ServerName} running at {oscQuery.HostIP} tcp:{oscQuery.TcpPort} osc:{oscQuery.OscPort}"
    );

    oscQuery.RefreshServices();
    oscQuery.AddEndpoint("/avatar/parameters", "b", Attributes.AccessValues.WriteOnly);

    if (pr.GetValueForOption(trackingOpt))
    {
        oscQuery.AddEndpoint(
            "/tracking/vrsystem/head/pose",
            "ffffff",
            Attributes.AccessValues.WriteOnly
        );
        oscQuery.AddEndpoint(
            "/tracking/vrsystem/leftwrist/pose",
            "ffffff",
            Attributes.AccessValues.WriteOnly
        );
        oscQuery.AddEndpoint(
            "/tracking/vrsystem/rightwrist/pose",
            "ffffff",
            Attributes.AccessValues.WriteOnly
        );
    }

    var knownServices = new HashSet<string>();
    void LogDiscovery(OSCQueryServiceProfile profile)
    {
        if (knownServices.Contains(profile.name))
            return;

        Console.WriteLine(
            $"Found service {profile.name} at {profile.address}:{profile.port} with type {profile.serviceType}"
        );
        knownServices.Add(profile.name);
    }

    oscQuery.OnOscQueryServiceAdded += LogDiscovery;
    oscQuery.OnOscServiceAdded += LogDiscovery;

    Console.WriteLine("Ready.");

    Thread.Sleep(int.MaxValue);
});

return await rootCommand.InvokeAsync(args);

