﻿using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public sealed class RepeatOptions
    {
        public RepeatOptions()
        {
            RetryDelay = 250.MilliSeconds();
        }

        public TimeSpan RetryDelay { get; set; }

        public uint? MaxTries
        {
            get { return RepeatMaxTriesPredicate?.MaxTries; }
            set { RepeatMaxTriesPredicate = value == null ? null : new RepeatMaxTriesPredicate(value.Value); }
        }

        public TimeSpan? Timeout
        {
            get { return RepeatTimeoutPredicate?.Timeout; }
            set { RepeatTimeoutPredicate = value == null ? null : new RepeatTimeoutPredicate(value.Value); }
        }

        /// <summary>
        /// Will be invoked on every retry cycle
        /// </summary>
        public Action PingOnRetry { get; set; }

        /// <summary>
        /// Exceptions of this type will be ignored and task will finish with success even if exceptions of this type occur
        /// </summary>
        public Type DontRethrowOnTaskFailedType { get; set; }

        /// <summary>
        /// Will always be invoked once on finish regardless of result
        /// </summary>
        public Action Finally { get; set; }

        internal RepeatMaxTriesPredicate RepeatMaxTriesPredicate { get; private set; }
        internal RepeatTimeoutPredicate RepeatTimeoutPredicate { get; private set; }
    }
}
