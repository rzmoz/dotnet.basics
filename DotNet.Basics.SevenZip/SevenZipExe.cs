﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;

namespace DotNet.Basics.SevenZip
{
    public class SevenZipExe
    {
        private static readonly Assembly _sevenZipAssembly = typeof(SevenZipExe).Assembly;

        private string _defaultInstallDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static Stream _7zaDll => _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.dll");
        private static Stream _7zaExe => _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7za.exe");
        private static Stream _7zxaDll => _sevenZipAssembly.GetManifestResourceStream("DotNet.Basics.SevenZip.7zxa.dll");

        private readonly DirPath _appRootDir;

        public SevenZipExe(string appRootDir = null)
            : this(appRootDir?.ToDir())
        { }

        public SevenZipExe(DirPath appRootDir = null)
        {
            _appRootDir = appRootDir ?? _defaultInstallDir.ToDir();
        }

        public (string Input, int ExitCode, string Output) ExtractToDirectory(string archivePath, string targetDirPath)
        {
            if (File.Exists(archivePath) == false)
                throw new IOException($"Archive not found: {archivePath}");
            if (Directory.Exists(targetDirPath))
                throw new IOException($"Target dir already exists at: {targetDirPath}");
            return ExecuteSevenZip("x", $"\"{archivePath}\"", $"\"-o{targetDirPath.ToDir().FullName()}\"", "*", "-r", "aoa");
        }

        public (string Input, int ExitCode, string Output) CreateFromDirectory(string sourceDirPath, string archivePath, bool overwrite = false)
        {
            if (overwrite == false && archivePath.ToFile().Exists())
                throw new IOException($"Target archive path already exists: {archivePath}. Set overwrite to true to ignore");

            archivePath.ToFile().DeleteIfExists();
            return ExecuteSevenZip("a", $"\"{archivePath}\"", $"\"{sourceDirPath.ToDir().FullName()}\\*\"", "-tzip", "-mx3", "-mmt");
        }

        public (string Input, int ExitCode, string Output) ExecuteSevenZip(string command, params string[] @params)
        {
            var filename = InstallSevenZip();
            var paramsString = @params.Aggregate(string.Empty, (current, param) => current + $" {param}");
            var script = $"{filename} {command} {paramsString} -y";
            return CmdPrompt.Run(script);
        }

        private string InstallSevenZip()
        {
            var appInstaller = new ExecutableInstaller(_appRootDir.ToDir("SevenZip"), "7za.exe");
            appInstaller.AddFromStream(appInstaller.EntryFile.Name, _7zaExe);
            appInstaller.AddFromStream("7za.dll", _7zaDll);
            appInstaller.AddFromStream("7zxa.dll", _7zxaDll);
            appInstaller.Install();
            return appInstaller.EntryFile.FullName();
        }
    }
}
