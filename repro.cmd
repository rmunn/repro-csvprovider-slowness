@echo off
cls

.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

packages\build\FAKE\tools\FAKE.exe build.fsx CopyBinaries
echo Now reproducing bug, should take about 10 minutes if bug is present...
bin\ReproCsv\ReproCsv.exe
