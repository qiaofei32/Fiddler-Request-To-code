@echo off

REM set path=C:\Windows\Microsoft.NET\Framework64\v4.0.30319

set path=C:\Windows\Microsoft.NET\Framework64\v3.5

if exist "%ProgramFiles%\Fiddler2\Fiddler.exe" set "FIDDLERPATH=%ProgramFiles%\Fiddler2"

if exist "%ProgramFiles(x86)%\Fiddler2\Fiddler.exe" set "FIDDLERPATH=%ProgramFiles(x86)%\Fiddler2"

if not defined FIDDLERPATH echo Can't find Fiddler in Program Files & exit

csc /d:TRACE /target:library /out:"Req2Code.dll" Req2Code.cs /reference:"%FIDDLERPATH%\Fiddler.exe"

REM move /Y "%cd%\Req2Code.dll" "C:\Program Files (x86)\Fiddler2\Scripts\Req2Code.dll"

pause