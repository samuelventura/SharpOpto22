TITLE NuGet Packager
@ECHO ON

CD %~dp0
CD ..
nuget pack SharpOpto22\SharpOpto22\package.nuspec
PAUSE

