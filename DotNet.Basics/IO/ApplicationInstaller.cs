﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DotNet.Basics.IO
{
    public class ApplicationInstaller
    {
        private const string _installingHandleName = "installing.dat";
        private const string _installedHandleName = "installed.dat";
        private readonly FilePath _installedHandle;
        private readonly IList<Action> _installActions;

        public ApplicationInstaller(DirPath installDir, string executableFilename)
        {
            if (installDir == null) throw new ArgumentNullException(nameof(installDir));
            InstallDir = installDir;
            EntryFile = installDir.ToFile(executableFilename);
            _installedHandle = installDir.ToFile(_installedHandleName);
            _installActions = new List<Action>();
        }

        public ApplicationInstaller(DirPath applicationsRoot, string applicationName, string executableFilename)
            : this(applicationsRoot.ToDir(applicationName), executableFilename)
        {
            if (applicationsRoot == null) throw new ArgumentNullException(nameof(applicationsRoot));
            if (applicationName == null) throw new ArgumentNullException(nameof(applicationName));
        }

        public DirPath InstallDir { get; }
        public FilePath EntryFile { get; }

        public void AddFromBytes(string filename, byte[] content)
        {
            var target = InstallDir.ToFile(filename);
            _installActions.Add(() =>
            {
                target.DeleteIfExists();//we ensure file integrity if we got this far. No guarantess that corrupt files haven't been left behind by a faulty installation
                using (var fsDst = new FileStream(target.FullName, FileMode.Create, FileAccess.Write))
                    fsDst.Write(content, 0, content.Length);
            });
        }

        public void Install()
        {
            //if already installed
            if (_installedHandle.Exists())
                return;

            using (var iolock = new IoLock(InstallDir, _installingHandleName))
            {
                var lockAcquired = iolock.TryAcquire();

                //someone else already installed the app in another thread so we're aborting
                if (lockAcquired && IsInstalled())
                    return;

                DebugOut.WriteLine($"Installing {EntryFile.Name} in {InstallDir.FullName}");

                //install and don't rely on rollback (try/catch) since it might conflict with other task
                foreach (var installAction in _installActions)
                    installAction();

                //app installed succesfully
                File.Create(_installedHandle.FullName);

                DebugOut.WriteLine($"{EntryFile.Name} successfully installed");
            }
        }

        public bool IsInstalled()
        {
            return _installedHandle.Exists();
        }

        public void UnInstall()
        {
            InstallDir.DeleteIfExists();
            DebugOut.WriteLine(InstallDir.Exists()
                ? $"{EntryFile.Name} failed to uninstall at {InstallDir}"
                : $"{EntryFile.Name} successfully uninstalld at {InstallDir}");
        }
    }
}
