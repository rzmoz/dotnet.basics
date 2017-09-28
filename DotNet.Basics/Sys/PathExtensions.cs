﻿namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return path.ToPath(IsFolder.Unknown, segments);
        }
        public static PathInfo ToPath(this string path, IsFolder isfolder, params string[] segments)
        {
            return path.ToPath(isfolder, PathSeparator.Unknown, segments);
        }
        public static PathInfo ToPath(this string path, IsFolder isFolder, PathSeparator pathSeparator, params string[] segments)
        {
            if (isFolder == IsFolder.Unknown)
                isFolder = PathInfo.DetectIsFolder(path, segments) ? IsFolder.True : IsFolder.False;

            return isFolder == IsFolder.True ? path.ToDir(pathSeparator, segments) as PathInfo : path.ToFile(pathSeparator, segments) as PathInfo;
        }
        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return pi.ToDir(pi.PathSeparator, segments);
        }
        public static DirPath ToDir(this PathInfo pi, PathSeparator pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToDir(pathSeparator, segments);
        }
        public static DirPath ToDir(this string path, params string[] segments)
        {
            return path.ToDir(PathSeparator.Unknown, segments);
        }
        public static DirPath ToDir(this string path, PathSeparator pathSeparator, params string[] segments)
        {
            return new DirPath(path, pathSeparator, segments);
        }


        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return pi.ToFile(pi.PathSeparator, segments);
        }
        public static FilePath ToFile(this PathInfo pi, PathSeparator pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToFile(pathSeparator, segments);
        }
        public static FilePath ToFile(this string path, params string[] segments)
        {
            return path.ToFile(PathSeparator.Unknown, segments);
        }
        public static FilePath ToFile(this string path, PathSeparator pathSeparator, params string[] segments)
        {
            return new FilePath(path, pathSeparator, segments);
        }

        public static T Add<T>(this T pi, params string[] segments) where T : PathInfo
        {
            return pi.RawPath.ToPath(pi.IsFolder ? IsFolder.True : IsFolder.False, segments) as T;
        }
    }
}
