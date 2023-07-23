dotnet build ..\SqliteDna.Integration\SqliteDna.Integration.csproj --configuration Release
@if errorlevel 1 goto end

dotnet build ..\SqliteDna.SourceGenerator\SqliteDna.SourceGenerator.csproj --configuration Release
@if errorlevel 1 goto end

dotnet build ..\SqliteDna.Testing\SqliteDna.Testing.csproj --configuration Release
@if errorlevel 1 goto end

cd ..\Package
nuget.exe pack SqliteDna\SqliteDna.nuspec -OutputDirectory nupkg -Verbosity detailed -NonInteractive
@if errorlevel 1 goto end

nuget.exe pack SqliteDna.Testing\SqliteDna.Testing.nuspec -OutputDirectory nupkg -Verbosity detailed -NonInteractive
@if errorlevel 1 goto end

:end