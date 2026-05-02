using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Common.Utilities.Helpers;

public static class AssemblyHelper
{
    private static List<Assembly>? _assemblies;
    private static List<Type>? _types;

    private static Func<string> _resolveBaseDirectory = () => AppDomain.CurrentDomain.BaseDirectory;

    private static Func<string, Assembly> _loadAssembly = dll =>
        AssemblyLoadContext.Default.LoadFromAssemblyName(
            new AssemblyName(Path.GetFileNameWithoutExtension(dll)));

    private static void LoadAllBinDirectoryAssemblies()
    {
        if (_assemblies?.Any() == true) return;

        _assemblies = new List<Assembly>();

        var binPath = _resolveBaseDirectory();

        var files = Directory.GetFiles(binPath, "*.dll")
            .Where(a =>
                Path.GetFileNameWithoutExtension(a).Contains("Services"))
            .ToList();

        foreach (var dll in files)
        {
            try
            {
                _assemblies.Add(_loadAssembly(dll));
            }
            catch (FileLoadException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (BadImageFormatException e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }

    private static List<Assembly> FindMyAssemblies()
    {
        LoadAllBinDirectoryAssemblies();
        return _assemblies ?? throw new Exception("Assemblies list is empty");
    }

    public static List<Type> FindAllTypes()
    {
        if (_types?.Any() == true) return _types;

        _types = FindMyAssemblies().SelectMany(a => a.GetTypes()).ToList();

        return _types ?? throw new Exception("Types list is empty");
    }
}