using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    public interface IWeaver
    {
        Action<string> LogError { get; }
        Action<string> LogInfo { get; }
        Action<string> LogWarning { get; }
        ModuleDefinition ModuleDefinition { get; }
        List<string> ReferenceCopyLocalPaths { get; set; }
    }
}