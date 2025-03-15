# VrcAdvert

Advertise your OSC app via OSCQuery. To be used with VRChat.

This is meant to be used with an OSC app that's not OSCQuery-ready.

# Usage

```
Description:
  Advertise your OSC app through OSCQuery.

Usage:
  VrcAdvert <name> <http_port> <osc_port> [options]

Arguments:
  <name>       The name of your OSC app. Can be anything as long as it's unique
  <http_port>  The port to use for OSCQuery. Can be any free TCP port.
  <osc_port>   The port that your OSC app is listening on. VRC will send OSC avatar parameters to this port.

Options:
  --tracking      Add this if you want to receive /tracking/vrsystem messages.
  --version       Show version information
  -?, -h, --help  Show help and usage information
```

# Building from source

On Linux, install the build-time dependencies `dotnet-runtime-8.0` and `dotnet-sdk-8.0` using your system's package manager, then run:

```
dotnet publish -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

The final executable is located in `bin/Release/net8.0/linux-x64/publish/VrcAdvert`.

# Works used
- Sample from [vrc-oscquery-lib](https://github.com/vrchat-community/vrc-oscquery-lib) - MIT License
