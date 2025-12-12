using System;
using System.Reflection;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services;

public class AssemblyService : IAssemblyService
{
    #region -- Methods --
    public Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //var ass = AppDomain.CurrentDomain.GetAssemblies().
        //    FirstOrDefault(a => a.GetName().Name == "Corno.Concept.Clients");

        foreach (var assembly in assemblies)
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        // Get Assembly name from its type name
        var nameStrings = typeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        var assemblyName = nameStrings[0];
        for (var index = 1; index < nameStrings.Length - 1; index++)
        {
            assemblyName += "." + nameStrings[index];
            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (null == assembly)
                    continue;
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            catch
            {
                //Ignore
            }
        }

        return null;
    }
    #endregion
}