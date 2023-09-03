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

# Works used
- Sample from [vrc-oscquery-lib](https://github.com/vrchat-community/vrc-oscquery-lib) - MIT License
