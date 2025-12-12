@echo off
"C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" "Corno.Web.csproj" /t:Rebuild /p:Configuration=Debug /v:minimal
