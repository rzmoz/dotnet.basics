﻿using System;
using System.IO;
using DotNet.Standard.Sys;

namespace DotNet.Standard.IO
{
    public static class FilePathExtensions
    {
        public static bool MoveTo(this FilePath fp, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            return fp.MoveTo(targetDir.ToFile(fp.Name), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath fp, string targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            return fp.MoveTo(targetFile.ToFile(), overwrite, ensureTargetDir);
        }

        public static bool MoveTo(this FilePath fp, FilePath targetFile, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (ensureTargetDir)
                targetFile.Directory().CreateIfNotExists();
            if (overwrite)
                targetFile.DeleteIfExists();

            FileSystem.Current.MoveFile(fp.FullName(), targetFile.FullName());
            return targetFile.Exists();
        }

        public static void CopyTo(this FilePath fp, DirPath targetDir, bool overwrite = false, bool ensureTargetDir = true)
        {
            fp.CopyTo(targetDir.ToFile(fp.Name), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath fp, DirPath targetDir, string newFileName, bool overwrite = false, bool ensureTargetDir = true)
        {
            fp.CopyTo(targetDir.ToFile(newFileName), overwrite, ensureTargetDir);
        }

        public static void CopyTo(this FilePath fp, FilePath target, bool overwrite = false, bool ensureTargetDir = true)
        {
            if (fp == null) throw new ArgumentNullException(nameof(fp));
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (fp.Exists() == false)
                throw new FileNotFoundException(fp.FullName());
            if (ensureTargetDir)
                target.Directory().CreateIfNotExists();

            FileSystem.Current.CopyFile(fp.FullName(), target.FullName(), overwrite);
        }

        public static FilePath WriteAllText(this FilePath fp, string content, bool overwrite = true)
        {
            if (overwrite == false && fp.Exists())
                throw new IOException($"Target file already exists. Set overwrite to true to ignore existing file: {fp.FullName()}");

            fp.Directory().CreateIfNotExists();

            //get filePath that's definitely not too long
            var testFile = Path.GetTempFileName();
            File.WriteAllText(testFile, content ?? string.Empty);
            fp.DeleteIfExists();
            FileSystem.Current.MoveFile(testFile, fp.FullName());
            return fp;
        }
        public static string ReadAllText(this FilePath fp, IfNotExists ifNotExists = IfNotExists.ThrowIoException)
        {
            try
            {
                return File.ReadAllText(fp.FullName());
            }
            catch (DirectoryNotFoundException)
            {
                if (ifNotExists == IfNotExists.ThrowIoException)
                    throw;
            }
            catch (FileNotFoundException)
            {
                if (ifNotExists == IfNotExists.ThrowIoException)
                    throw;
            }
            return null;
        }
    }
}