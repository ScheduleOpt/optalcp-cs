# OptalCP C# API

This package provides a C# interface to [OptalCP solver](https://optalcp.com).
The name of the package is `OptalCP`.

To use this package, you need the OptalCP binary on your system.
You can use the `OptalCP.Bin.Preview` package, which provides a preview version.
The preview version solves all problems but reports only objective values, not values of the decision variables.

Usually, there's no need to install `OptalCP` directly.
It is automatically installed with `OptalCP.Bin.Preview` or `OptalCP.Bin` package.

## Installation

GitHub Packages requires authentication even for public NuGet packages.
Create a **classic** token (not fine-grained) at [github.com/settings/tokens](https://github.com/settings/tokens)
with the `read:packages` scope. See [GitHub's NuGet registry documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry) for details.

Configure the NuGet source (one-time setup):

```bash
dotnet nuget add source "https://nuget.pkg.github.com/ScheduleOpt/index.json" \
  --name ScheduleOpt \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
```

> **Note:** `--store-password-in-clear-text` is required on Linux and macOS where
> the .NET credential encryption is not available. The token is stored in
> `~/.nuget/NuGet/NuGet.Config`.

Install the preview edition:

```bash
# This automatically installs the OptalCP API package as a dependency
dotnet add package OptalCP.Bin.Preview
```

## Full and Academic Editions

For full functionality with solution details:

- **Full edition** (`OptalCP.Bin`): Commercial license, requires GitHub authentication
- **Academic edition** (`OptalCP.Bin.Academic`): **Free for academic use**, requires GitHub authentication

Contact [ScheduleOpt](https://optalcp.com) for licensing options. Academic licenses are provided free of charge for qualified academic institutions.

## Documentation

The documentation can be found on the [OptalCP web page](https://optalcp.com/docs/api).
