﻿using System;

namespace DotNet.Basics.IO
{
    public static class Paths
    {
        static Paths()
        {
            try
            {
                UseFileSystem(new NetCoreWin32FileSystemLongPaths());
            }
            catch (AggregateException)
            {
                UseFileSystem(new NetFrameworkWin32FileSystemLongPaths());
            }
        }

        public static IFileSystem FileSystem { get; private set; }

        public static void UseFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
    }
}
