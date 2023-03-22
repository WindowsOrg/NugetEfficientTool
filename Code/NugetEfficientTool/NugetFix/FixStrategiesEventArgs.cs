using System;
using System.Collections.Generic;
using NugetEfficientTool.Business;

namespace NugetEfficientTool
{
    public class FixStrategiesEventArgs : EventArgs
    {
        public FixStrategiesEventArgs( IEnumerable<NugetFixStrategy> nugetFixStrategies)
        {
            NugetFixStrategies = nugetFixStrategies ?? throw new ArgumentNullException(nameof(nugetFixStrategies));
        }

        public IEnumerable<NugetFixStrategy> NugetFixStrategies { get; }
    }
}