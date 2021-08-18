﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    internal interface INugetFileReplacer
    {
        ReplacedFileRecord ReplaceNuget();
        void RevertNuget();
    }
    
}
