using System;
using System.Text;
using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;
using Cauldron.Interception.Cecilator.Coders;

public static class MethodRenamer
{
    public const string Name = "Method renamer";
    public const int Priority = 0;

    [Display("Renamer")]
    public static void Implement(Builder builder)
    {
        var methods = builder.FindMethodsByAttribute("Cauldron.Activator.RenameAttribute");

        foreach (var method in methods)
        {
            method.Method.Name = "<>__" + method.Method.Name;
            method.Attribute.Remove();
            method.Attribute.Type.Remove();
        }
    }
}