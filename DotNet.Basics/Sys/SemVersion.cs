﻿using System;
using static System.String;

namespace DotNet.Basics.Sys
{
    public class SemVersion : IComparable<SemVersion>
    {
        public SemVersion()
            : this(Empty)
        { }

        public SemVersion(object semVer)
            : this(semVer?.ToString())
        { }
        public SemVersion(string semVer)
        : this(Parse(semVer))
        { }
        public SemVersion(SemVersion semVer)
            : this(semVer.Major, semVer.Minor, semVer.Patch, semVer.PreRelease, semVer.Metadata)
        { }

        public SemVersion(int major, int minor, int patch, string preRelease = null, string metadata = null)
        : this(major, minor, patch, new SemVersionPreRelease(preRelease), metadata)
        {

        }
        public SemVersion(int major, int minor, int patch, SemVersionPreRelease preRelease, string metadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease ?? new SemVersionPreRelease();
            Metadata = metadata ?? Empty;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public SemVersionPreRelease PreRelease { get; set; }
        public string Metadata { get; set; }

        public string SemVer10String => $"{Major}.{Minor}.{Patch}";

        public string SemVer20String
        {
            get
            {
                var semVer20String = SemVer10String;
                if (PreRelease.Any)
                    semVer20String += $"{SemVersionLexer.PreReleaseSeparator}{PreRelease}";
                if (IsNullOrWhiteSpace(Metadata) == false)
                    semVer20String += $"{SemVersionLexer.MetadataSeparator}{Metadata.TrimStart(SemVersionLexer.MetadataSeparator)}";
                return semVer20String;
            }
        }
        public static SemVersion Parse(string semVer)
        {
            if (semVer == null)
                return null;

            semVer = semVer.RemovePrefix("v", StringComparison.InvariantCultureIgnoreCase);
            var lexer = new SemVersionLexer();
            var tokens = lexer.Lex(semVer);
            var major = int.Parse(tokens[0]);
            var minor = int.Parse(tokens[1]);
            var patch = int.Parse(tokens[2]);
            var preRelease = new SemVersionPreRelease(tokens[3]);
            var metaData = tokens[4];
            return new SemVersion(major, minor, patch, preRelease, metaData);
        }

        public override string ToString()
        {
            return SemVer20String;
        }

        public static bool operator ==(SemVersion a, SemVersion b)
        {
            return a.Major == b.Major &&
                   a.Minor == b.Minor &&
                   a.Patch == b.Patch &&
                   a.PreRelease == b.PreRelease;
        }
        public static bool operator !=(SemVersion a, SemVersion b)
        {
            return !(a == b);
        }
        public static bool operator <(SemVersion a, SemVersion b)
        {
            if (a.Major < b.Major)
                return true;
            if (a.Major > b.Major)
                return false;

            if (a.Minor < b.Minor)
                return true;
            if (a.Minor > b.Minor)
                return false;

            if (a.Patch < b.Patch)
                return true;
            if (a.Patch > b.Patch)
                return false;

            return a.PreRelease < b.PreRelease;
        }
        public static bool operator >(SemVersion a, SemVersion b)
        {
            if (a.Major > b.Major)
                return true;
            if (a.Major < b.Major)
                return false;

            if (a.Minor > b.Minor)
                return true;
            if (a.Minor < b.Minor)
                return false;

            if (a.Patch > b.Patch)
                return true;
            if (a.Patch < b.Patch)
                return false;

            return a.PreRelease > b.PreRelease;
        }

        protected bool Equals(SemVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && String.Equals(PreRelease, other.PreRelease);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemVersion)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ (PreRelease != null ? PreRelease.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int CompareTo(SemVersion other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }
    }
}
