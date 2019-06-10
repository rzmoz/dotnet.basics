﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public class SemVersionPreRelease
    {
        private const string _preReleaseAllowedCharsFormat = @"^[a-zA-Z0-9\.]+$";
        private static readonly Regex _preReleaseAllowedCharsRegex = new Regex(_preReleaseAllowedCharsFormat, RegexOptions.Compiled);

        private const string _allNumbersFormat = @"^[0-9]+$";
        private static readonly Regex _allNumbersFormatRegex = new Regex(_allNumbersFormat, RegexOptions.Compiled);

        private readonly string _hashBase;

        public SemVersionPreRelease()
            : this(Enumerable.Empty<SemVersionIdentifier>())
        { }
        public SemVersionPreRelease(string preRelease)
        : this(ParsePreRelease(preRelease))
        { }

        public SemVersionPreRelease(IEnumerable<SemVersionIdentifier> identifiers)
        {
            if (identifiers == null) throw new ArgumentNullException(nameof(identifiers));
            //lowercase identifiers to ignore case and because lower case chars have a higher ascii value than numerics so numerics are always smaller / lower than chars
            Identifiers = identifiers.ToList();
            _hashBase = Identifiers.Select(i => i.ToString()).JoinString(SemVersionLexer.VersionSeparator.ToString());

            if (_hashBase.Length > 0 && _preReleaseAllowedCharsRegex.IsMatch(_hashBase) == false)
                throw new ArgumentOutOfRangeException($"Invalid character(s) found in PreRelease input. ASCII alphanumerics are alloed [a-zA-Z0-9]. Input was: '{_hashBase}'");
            Any = Identifiers.Any(i => string.IsNullOrWhiteSpace(i.ToString()) == false);
        }

        public IReadOnlyList<SemVersionIdentifier> Identifiers { get; }
        public bool Any { get; }

        private static IEnumerable<SemVersionIdentifier> ParsePreRelease(string preRelease)
        {
            return preRelease?.Split(SemVersionLexer.VersionSeparator).Select(i => new SemVersionIdentifier(i)) ?? new List<SemVersionIdentifier>();
        }

        public static bool operator ==(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            return !(a == b);
        }
        public static bool operator <(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (ReferenceEquals(null, a) || string.IsNullOrWhiteSpace(a._hashBase))
                return false;//a can only be higher or same as b when b is not set

            if (ReferenceEquals(null, b) || string.IsNullOrWhiteSpace(b._hashBase))
                return true;//a is always lower if a is set and b is not

            for (var skip = 0; skip < a.Identifiers.Count; skip++)
            {
                var aIdentifier = a.Identifiers.Skip(skip).FirstOrDefault();
                var bIdentifier = b.Identifiers.Skip(skip).FirstOrDefault();

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier < bIdentifier;
            }
            return a.Identifiers.Count < b.Identifiers.Count;
        }
        public static bool operator >(SemVersionPreRelease a, SemVersionPreRelease b)
        {
            if (ReferenceEquals(null, b) || string.IsNullOrWhiteSpace(b._hashBase))
                return false;//a can only be lower or same as b when a is not set
            if (ReferenceEquals(null, a) || string.IsNullOrWhiteSpace(a._hashBase))
                return true;//a is always higher if b is set and a is not

            for (var skip = 0; skip < a.Identifiers.Count; skip++)
            {
                var aIdentifier = a.Identifiers.Skip(skip).FirstOrDefault();
                var bIdentifier = b.Identifiers.Skip(skip).FirstOrDefault();

                if (aIdentifier == bIdentifier)
                    continue;

                return aIdentifier > bIdentifier;
            }
            return a.Identifiers.Count > b.Identifiers.Count;
        }
        protected bool Equals(SemVersionPreRelease other)
        {
            return other._hashBase.Equals(_hashBase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemVersionPreRelease)obj);
        }

        public override int GetHashCode()
        {
            return _hashBase.GetHashCode();
        }

        public int CompareTo(SemVersionPreRelease other)
        {
            if (this < other)
                return -1;
            if (this > other)
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return _hashBase;
        }
    }
}
