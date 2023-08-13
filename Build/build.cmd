call "C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\Tools\VsMSBuildCmd.bat"

dotnet build ..\SqliteDna.Integration\SqliteDna.Integration.csproj --configuration Release
@if errorlevel 1 goto end

dotnet build ..\SqliteDna.SourceGenerator\SqliteDna.SourceGenerator.csproj --configuration Release
@if errorlevel 1 goto end

msbuild ..\SqliteDna.SQLiteCppManaged\SqliteDna.SQLiteCppManaged.vcxproj /p:Configuration=Release /p:Platform=x64
@if errorlevel 1 goto end

dotnet build ..\SqliteDna.Testing\SqliteDna.Testing.csproj --configuration Release
@if errorlevel 1 goto end

cd ..\Package
nuget.exe pack SqliteDna\SqliteDna.nuspec -OutputDirectory nupkg -Verbosity detailed -NonInteractive
@if errorlevel 1 goto end

nuget.exe pack SqliteDna.Testing\SqliteDna.Testing.nuspec -OutputDirectory nupkg -Verbosity detailed -NonInteractive
@if errorlevel 1 goto end

nuget.exe pack SqliteDna.Templates\SqliteDna.Templates.nuspec -OutputDirectory nupkg -Verbosity detailed -NonInteractive
@if errorlevel 1 goto end

:end