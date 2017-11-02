﻿namespace DotNet.Basics.Sys
{
    public static class PathExtensions
    {
        public static PathInfo ToPath(this string path, params string[] segments)
        {
            return path.ToPath(PathType.Unknown, segments);
        }
        public static PathInfo ToPath(this string path, PathType pathType, params string[] segments)
        {
            return path.ToPath(pathType, PathSeparator.Unknown, segments);
        }
        public static PathInfo ToPath(this string path, char pathSeparator, params string[] segments)
        {
            return path.ToPath(PathType.Unknown, pathSeparator, segments);
        }
        public static PathInfo ToPath(this string path, PathType pathType, char pathSeparator, params string[] segments)
        {
            if (pathType == PathType.Unknown)
                pathType = PathInfo.DetectIsFolder(path, segments) ? PathType.Folder : PathType.File;

            return pathType == PathType.Folder ? path.ToDir(pathSeparator, segments) : path.ToFile(pathSeparator, segments) as PathInfo;
        }
        public static DirPath ToDir(this PathInfo pi, params string[] segments)
        {
            return pi.ToDir(pi.Separator, segments);
        }
        public static DirPath ToDir(this PathInfo pi, char pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToDir(pathSeparator, segments);
        }
        public static DirPath ToDir(this string path, params string[] segments)
        {
            return path.ToDir(PathSeparator.Unknown, segments);
        }
        public static DirPath ToDir(this string path, char pathSeparator, params string[] segments)
        {
            return new DirPath(path, pathSeparator, segments);
        }

        public static FilePath ToFile(this PathInfo pi, params string[] segments)
        {
            return pi.ToFile(pi.Separator, segments);
        }
        public static FilePath ToFile(this PathInfo pi, char pathSeparator, params string[] segments)
        {
            return pi.RawPath.ToFile(pathSeparator, segments);
        }
        public static FilePath ToFile(this string path, params string[] segments)
        {
            return path.ToFile(PathSeparator.Unknown, segments);
        }
        public static FilePath ToFile(this string path, char pathSeparator, params string[] segments)
        {
            return new FilePath(path, pathSeparator, segments);
        }

        public static T Add<T>(this T pi, params string[] segments) where T : PathInfo
        {
            return pi.RawPath.ToPath(pi.IsFolder ? PathType.Folder : PathType.File, segments) as T;
        }
    }
}