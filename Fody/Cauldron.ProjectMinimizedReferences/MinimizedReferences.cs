using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using System.Linq;
using Mono.Cecil;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

public static class MinimizedReferences
{
    public const string Name = "Project minimize references";
    public const int Priority = int.MaxValue;
    private static string[] minimizeList;
    public static XElement Config { get; set; }

    private static string[] MinimizeList
    {
        get
        {
            if (minimizeList == null)
                minimizeList = GetMinimizeList().ToArray();

            return minimizeList;
        }
    }

    public static bool IsFiltered(this BuilderType type) => MinimizeList.Any(x => x == type.Namespace);

    [Display("Delete Members without usage")]
    public static void Z_Implement(Builder builder)
    {
        bool RemoveUnusedMethods()
        {
            foreach (var item in builder.GetTypes(SearchContext.Module).Where(x => x.Name != "<Module>" && !x.IsPublic && !x.IsUsed && x.IsFiltered()))
            {
                builder.Log(LogTypes.Info, $"Deleting Type --> {item}");
                item.Remove();
            }

            var allMethods = builder.GetTypes(SearchContext.Module)
                .SelectMany(x => x.Methods)
                .Where(x => !x.OriginType.IsPublic && x.OriginType.IsFiltered() && x.IsStatic && x.Name != ".cctor" && x.Name != "op_Implicit" && x.Name != "op_Explicit")
                .ToArray();
            var usages = allMethods.SelectMany(x => x.FindUsages()).ToArray();

            var unusedMethods = allMethods.Except(usages.Select(x => x.Method));
            var types = unusedMethods.Select(x => x.OriginType).Distinct().ToArray();

            foreach (var item in unusedMethods)
            {
                builder.Log(LogTypes.Info, $"Deleting Static Method --> {item}");
                item.Remove();
            }

            foreach (var item in types.SelectMany(x => x.Properties).Where(x => x.Setter == null && x.Getter == null).ToArray())
            {
                builder.Log(LogTypes.Info, $"Deleting Property --> {item}");
                item.Remove();
            }

            foreach (var item in builder.GetTypes(SearchContext.Module)
                .Distinct().Where(x => x.Name != "<Module>" && !x.IsPublic && !x.Properties.Any() && !x.Methods.Any() && !x.Fields.Any()).ToArray())
            {
                builder.Log(LogTypes.Info, $"Deleting Empty Type --> {item}");
                item.Remove();
            }

            //var fields = builder.GetTypes(SearchContext.Module)
            //    .SelectMany(x => x.Fields)
            //    .Where(x => !x.OriginType.IsPublic && x.OriginType.IsFiltered())
            //    .ToArray();
            //var fieldUsages = fields.SelectMany(x => x.FindUsages()).ToArray();

            //foreach (var item in fields.Except(fieldUsages.Select(x => x.Field)))
            //{
            //    builder.Log(LogTypes.Info, $"Deleting Field --> {item}");
            //    item.Remove();
            //}

            return unusedMethods.Any();
        }

        while (RemoveUnusedMethods())
        {
        }

        //foreach (var item in types.Distinct().Where(x => !x.Properties.Any() && !x.Methods.Any() && !x.Fields.Any()).ToArray())
        //{
        //    builder.Log(LogTypes.Info, $"Deleting --> {item}");
        //    item.Remove();
        //}

        //foreach (var item in builder.GetTypes(SearchContext.Module).Where(x => x.IsPublic))
        //    if (GetMinimizeList().Any(x => x == item.Namespace))
        //        item.Attributes = item.Attributes & ~TypeAttributes.Public | TypeAttributes.NotPublic;
    }

    private static IEnumerable<string> GetMinimizeList()
    {
        var element = Config.Element("Minimize");

        if (element == null)
            yield break;

        foreach (var item in element.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
        {
            if (string.IsNullOrEmpty(item))
                continue;

            yield return item;
        }
    }
}