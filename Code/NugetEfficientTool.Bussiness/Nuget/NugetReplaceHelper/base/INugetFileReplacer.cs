namespace NugetEfficientTool.Business
{
    internal interface INugetFileReplacer
    {
        ReplacedFileRecord ReplaceNuget();
        void RevertNuget();
    }
    
}
