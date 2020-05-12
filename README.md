# Overview
Many code bases are separated into independent NuGet "packages", a given product will typically include many references to pre-built packages.

A given product will typically comprise
- some start-up code
- product-specific application code
- product-specific tests
- references to other related packages
- references to third-party utility packages

In terms of structure this is ideal since we have
- strong control of versions - the packages cannot be changed directly
- faster builds - we're only building the product-specific code, the remainder is already built within the packages

However, if we want to find a problem relating to a bug report, or debug fully during development, it can be ideal to have all of the source code locally so we can dive right into the  code etc.

This is where the "MungeTool" comes in.

# Tech stack
- Microsoft WPF
- [Squirrel installer](https://github.com/Squirrel/Squirrel.Windows)

# Acknowledgements
The initial version is a (sanitised version of a) tool written by [Dan Clarke](https://github.com/dracan) as part of [Sharp Life Science](https://www.aqdrop.com/) build system.