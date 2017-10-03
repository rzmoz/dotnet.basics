﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Sys
{
    public abstract class PathInfo
    {
        private static readonly char[] _separatorDetectors = { PathSeparator.Backslash, PathSeparator.Slash };

        protected PathInfo(string path, params string[] segments)
            : this(path, Sys.IsFolder.Unknown, segments)
        { }

        protected PathInfo(string path, Sys.IsFolder isFolder, params string[] segments)
            : this(path, isFolder, DotNet.Basics.Sys.PathSeparator.Unknown, segments)
        { }

        protected PathInfo(string path, Sys.IsFolder isFolder, char pathSeparator, params string[] segments)
        {
            if (path == null)
                path = string.Empty;

            var combinedSegments = path.ToArray(segments).Where(itm => itm != null).ToArray();

            IsFolder = isFolder == Sys.IsFolder.Unknown ? DetectIsFolder(path, segments) : isFolder == Sys.IsFolder.True;

            Separator = DetectPathSeparator(pathSeparator, combinedSegments);

            //Clean segments
            Segments = CleanSegments(combinedSegments, Separator).ToArray();

            //Set rawpath
            RawPath = string.Join(Separator.ToString(), Segments);
            RawPath = IsFolder ? RawPath.EnsureSuffix(Separator) : RawPath.RemoveSuffix(Separator);

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Separator));
        }


        public string RawPath { get; }
        public string Name { get; }
        public bool IsFolder { get; }

        public DirPath Parent => Segments.Count <= 1 ? null : new DirPath(null, Segments.Take(Segments.Count - 1).ToArray());
        public char Separator { get; }
        public IReadOnlyCollection<string> Segments;

        public override string ToString()
        {
            return RawPath;
        }

        private IEnumerable<string> CleanSegments(IEnumerable<string> combinedSegments, char separatorChar)
        {
            //to single string
            var joined = string.Join(separatorChar.ToString(), combinedSegments);
            //conform path separators
            joined = joined.Replace(PathSeparator.Backslash, separatorChar);
            joined = joined.Replace(PathSeparator.Slash, separatorChar);

            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{separatorChar}]{{2,}}", separatorChar.ToString(), RegexOptions.None);

            //to segments
            return joined.Split(new[] { separatorChar }, StringSplitOptions.RemoveEmptyEntries).Where(seg => String.IsNullOrWhiteSpace(seg) == false);
        }

        public static bool DetectIsFolder(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return false;

            return lookingAt.EndsWith(PathSeparator.Backslash) || lookingAt.EndsWith(PathSeparator.Slash);
        }

        private static char DetectPathSeparator(char pathSeparator, IEnumerable<string> segments)
        {
            if (_separatorDetectors.Contains(pathSeparator))
                return pathSeparator;

            if (pathSeparator == PathSeparator.Unknown)
                //auto detect supported separators
                foreach (var segment in segments)
                {
                    if (segment == null)
                        continue;
                    //first separator wins!
                    var separatorIndex = segment.IndexOfAny(_separatorDetectors);
                    if (separatorIndex >= 0)
                        return segment[separatorIndex];
                }

            return PathSeparator.Backslash;//default
        }
    }
}