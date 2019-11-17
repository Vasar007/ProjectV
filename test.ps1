$ErrorActionPreference = "Stop";

if ($env:configuration -eq "Debug")
{
    dotnet test "C:\projects\thingappraiser\ThingAppraiser\Tests\ThingAppraiser.ContentDirectories.Tests\ThingAppraiser.ContentDirectories.Tests.fsproj" --configuration Debug --no-build
}
else
{
    dotnet test "C:\projects\thingappraiser\ThingAppraiser\Tests\ThingAppraiser.ContentDirectories.Tests\ThingAppraiser.ContentDirectories.Tests.fsproj" --configuration Release --no-build
}
