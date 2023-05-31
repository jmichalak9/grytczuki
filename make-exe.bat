@echo off
set folder=EXE
if not exist "%folder%" (
    mkdir "%folder%"
    echo EXE directory created.
) else (
    echo EXE directory exists.
)

dotnet publish .\AbelThueOnline\AbelThueOnline\AbelThueOnline.csproj -r win-x64 -c Release -o . -p:DebugType=None -p:DebugSymbols=false -p:PublishReadyToRunShowWarnings=true -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true

echo AbelThueOnline.exe created in EXE directory.

pause