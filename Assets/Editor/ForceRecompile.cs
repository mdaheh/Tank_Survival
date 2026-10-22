using UnityEditor;
using UnityEditor.Compilation;

public static class ForceRecompile
{
    [MenuItem("Tools/Force Recompile Scripts (Show Warnings)")]
    public static void Recompile()
    {
        // Принудительно перекомпилировать все скрипты, даже если не было изменений
        CompilationPipeline.RequestScriptCompilation(
            RequestScriptCompilationOptions.CleanBuildCache
        );
    }
}