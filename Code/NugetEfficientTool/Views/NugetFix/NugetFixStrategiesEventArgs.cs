using System;
using System.Collections.Generic;
using NugetEfficientTool.Business;

namespace NugetEfficientTool
{
    public class NugetFixStrategiesEventArgs : EventArgs
    {
        public NugetFixStrategiesEventArgs( IEnumerable<NugetFixStrategy> nugetFixStrategies)
        {
            NugetFixStrategies = nugetFixStrategies ?? throw new ArgumentNullException(nameof(nugetFixStrategies));
        }

        public IEnumerable<NugetFixStrategy> NugetFixStrategies { get; }
    }
}