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
    private static MinimizeListItem[] minimizeList;

    public enum Target
    {
        All,
        PrivateOnly
    }

    public static XElement Config { get; set; }

    private static MinimizeListItem[] MinimizeList
    {
        get
        {
            if (minimizeList == null)
                minimizeList = GetMinimizeList().ToArray();

            return minimizeList;
        }
    }

    public static bool IsFiltered(this BuilderType type) =>
        MinimizeList.Any(x =>
        {
            if (x.Namespace != type.Namespace)
                return false;

            if (!type.IsNested && x.Target == Target.PrivateOnly && type.IsPublic)
                return false;

            if (x.Target == Target.All && type.IsPublic)
                type.IsInternal = true;

            return true;
        });

    [Display("Delete Members without usage")]
    public static void Z_Implement(Builder builder)
    {
        bool deleteUnusedTypes()
        {
            var unusedTypes = new List<BuilderType>();

            foreach (var item in builder.GetTypes(SearchContext.Module))
            {
                if (string.IsNullOrEmpty(item.Namespace) && (
                    item.Fullname == "CauldronInterceptionHelper" ||
                    item.Fullname == "ProcessedByFody" ||
                    item.Fullname == "<Module>"))
                    continue;

                if (item.IsStatic)
                    continue;

                if (!item.IsFiltered())
                    continue;

                if (item.IsUsed)
                    continue;

                unusedTypes.Add(item);
            }

            foreach (var item in unusedTypes)
            {
                builder.Log(LogTypes.Info, $"Deleting Type --> {item}");
                item.Remove();
            }

            return unusedTypes.Count != 0;
        }

        bool removeUnusedStuff()
        {
            while (deleteUnusedTypes())
            {
            }

            // Delete all unused static methods
            var allMethods = builder.GetTypes(SearchContext.Module)
                .SelectMany(x => x.Methods)
                .Where(x => x.OriginType.IsFiltered() && x.IsStatic && x.Name != ".cctor" && x.Name != "op_Implicit" && x.Name != "op_Explicit")
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
                .Distinct().Where(x => !x.Properties.Any() && !x.Methods.Any() && !x.Fields.Any() && x.IsFiltered()).ToArray())
            {
                builder.Log(LogTypes.Info, $"Deleting Empty Type --> {item}");
                item.Remove();
            }

            var fields = builder.GetTypes(SearchContext.Module)
                .SelectMany(x => x.Fields)
                .Where(x => x.IsPrivate && x.OriginType.IsFiltered())
                .ToArray();
            var fieldUsages = fields.SelectMany(x => x.FindUsages()).ToArray();
            var unusedFields = fields.Except(fieldUsages.Select(x => x.Field)).ToArray();

            foreach (var item in unusedFields)
            {
                builder.Log(LogTypes.Info, $"Deleting Field --> {item}");
                item.Remove();
            }

            return unusedMethods.Any() || unusedFields.Any();
        }

        while (removeUnusedStuff())
        {
        }
    }

    private static IEnumerable<MinimizeListItem> GetMinimizeList()
    {
        var element = Config.Element("Minimize");

        if (element == null)
            yield break;

        foreach (var item in element.Value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
        {
            if (string.IsNullOrEmpty(item))
                continue;

            var result = item.Split(',');
            yield return new MinimizeListItem
            {
                Namespace = result[0],
                Target = result.Length > 1 ? result[1].ToTarget() : Target.PrivateOnly
            };
        }
    }

    private static Target ToTarget(this string value)
    {
        switch (value.Trim())
        {
            case "all": return Target.All;
            default: return Target.PrivateOnly;
        }
    }

    private class MinimizeListItem
    {
        public string Namespace { get; set; }
        public Target Target { get; set; }
    }
}