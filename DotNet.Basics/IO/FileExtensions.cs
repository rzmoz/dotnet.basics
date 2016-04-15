﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public static class FileExtensions
    {
        public static bool IsFileType(this FileInfo file, FileType fileType)
        {
            if (fileType == null) throw new ArgumentNullException(nameof(fileType));
            if (file == null)
                return false;
            return file.Name.EndsWith(fileType.Extension, true, null);
        }

        public static FileInfo ToFile(this string dir, params string[] paths)
        {
            return new FileInfo(dir.ToPath(paths));
        }

        public static FileInfo ToFile(this DirectoryInfo dir, params string[] paths)
        {
            return new FileInfo(dir.FullName.ToPath(paths));
        }

        public static void CopyTo(this IEnumerable<FileInfo> sourceFiles, DirectoryInfo targetDir, bool overwrite = false)
        {
            Parallel.ForEach(sourceFiles, (file) => file.CopyTo(targetDir, overwrite: overwrite));
        }

        public static void CopyTo(this FileInfo file, DirectoryInfo targetDir, bool overwrite = false)
        {
            file.CopyTo(targetDir.ToFile(file.Name), overwrite: overwrite);
        }

        public static void CopyTo(this FileInfo source, DirectoryInfo targetDir, string newFileName, bool overwrite = false)
        {
            source.CopyTo(targetDir.ToFile(newFileName), overwrite: overwrite);
        }

        public static void CopyTo(this FileInfo source, FileInfo target, bool overwrite = false)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            target.Directory.CreateIfNotExists();
            File.Copy(source.FullName, target.FullName, overwrite: overwrite);
            Debug.WriteLine($"{source.FullName} copied to: {target.FullName}");
        }

        public static void MoveTo(this FileInfo source, DirectoryInfo targetDir, bool overwrite = false)
        {
            source.MoveTo(targetDir.ToFile(source.Name), overwrite);
        }
        public static void MoveTo(this FileInfo sourceFile, FileInfo targetFile, bool overwrite = false)
        {
            if (sourceFile.FullName == targetFile.FullName)
            {
                Debug.WriteLine("MoveTo skipped. Source and target are the same: {0}", sourceFile.FullName);
                return;
            }

            if (!sourceFile.Exists())
            {
                Debug.WriteLine("MoveTo skipped. Source not found: {0}", sourceFile.FullName);
                return;
            }

            if (overwrite == false && targetFile.Exists())
            {
                Debug.WriteLine("MoveTo skipped. Target already exists and overwrite is set to false: {0}", targetFile.FullName);
                return;
            }

            targetFile.DeleteIfExists();
            File.Move(sourceFile.FullName, targetFile.FullName);
        }

        public static string NameWithoutExtension(this FileInfo file)
        {
            return Path.GetFileNameWithoutExtension(file?.Name);
        }

        public static string ReadAllText(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            return File.ReadAllText(file.FullName);
        }
    }
}