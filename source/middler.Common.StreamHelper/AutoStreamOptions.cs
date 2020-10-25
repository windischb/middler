using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace middler.Common.StreamHelper
{
    public class AutoStreamOptions
    {
        public int? MemoryThreshold { get; internal set; }
        
        public string TempDirectory { get; internal set;}

        public string FilePrefix { get; internal set; }

        
    }

    public class AutoStreamOptionsBuilder
    {
        private readonly AutoStreamOptions _autoStreamOptions = new AutoStreamOptions();

        public AutoStreamOptionsBuilder WithFilePrefix(string value)
        {
            _autoStreamOptions.FilePrefix = value;
            return this;
        }
        public AutoStreamOptionsBuilder WithTempDirectory(string value)
        {
            _autoStreamOptions.TempDirectory = value;
            return this;
        }

        public AutoStreamOptionsBuilder WithMemoryThreshold(int? value)
        {
            _autoStreamOptions.MemoryThreshold = value;
            return this;
        }

        public static implicit operator AutoStreamOptions(AutoStreamOptionsBuilder builder)
        {
            return builder._autoStreamOptions;
        }

        public static implicit operator AutoStreamOptionsBuilder(Action<AutoStreamOptionsBuilder> options)
        {
            var optsBuilder = new AutoStreamOptionsBuilder();
            options?.Invoke(optsBuilder);
            return optsBuilder;
        }
    }
}
