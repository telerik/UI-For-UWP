SET MSBUILD=%WINDIR%\microsoft.net\framework\v4.0.30319\MSBuild.exe
%MSBUILD% BuildNuget.UWP.proj /property:Version=1.0.0.3
