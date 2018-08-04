using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cauldron;
using Cauldron.Reflection;

namespace Cauldron.Yaml
{
    internal sealed class YamlDeserializer
    {
        private Dictionary<Type, TypeAndAttribute[]> typeCache = new Dictionary<Type, TypeAndAttribute[]>();

        public TItem[] Parse<TItem>(string content) where TItem : class, new()
        {
            var body = new Yaml();
            var lines = content.Split(new string[] { "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
                i = this.ParseLevel(body, ref lines, i);

            var result = this.Parse(body, typeof(TItem));

            return result as TItem[];
        }

        private static void GetEnclosedString(string characterAtEndOfLine, string[] lines, ref int index, ref string stringValue)
        {
            for (int x = index + 1; x < lines.Length; x++)
            {
                stringValue += "\r\n" + lines[x].Trim();
                if (lines[x].TrimEnd().EndsWith(characterAtEndOfLine))
                {
                    index = x;
                    break;
                }
            }
        }

        private int GetCurrentLevel(string line)
        {
            if (line.Length == 0)
                return 0;

            if (line[0] == '-')
                return 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != ' ')
                    return i;
            }

            return 0;
        }

        private object Parse(Yaml yamlNode, Type targetType)
        {
            var result = Array.CreateInstance(targetType, yamlNode.Children.Count);

            for (int i = 0; i < result.Length; i++)
            {
                var item = targetType.CreateInstance();
                this.SetValues(ref item, yamlNode.Children[i], targetType);
                result.SetValue(item, i);
            }

            return result;
        }

        private int ParseLevel(Yaml yaml, ref string[] lines, int index)
        {
            var line = lines[index];
            var level = yaml.Level;

            if (line.Trim() == "")
                return index;

            var keyValue = line.Split(new char[] { ':' }, 2);
            var stringValue = keyValue.Length > 1 ? keyValue[1].Trim() : string.Empty;

            // Check if the value starts with a ' but the value should not end with ' too That case
            // does not need a special treatment
            if (stringValue.StartsWith("\'") && !stringValue.EndsWith("\'"))
                GetEnclosedString("\'", lines, ref index, ref stringValue);
            // Same as ' ... But with " instead of ' ... YAML supports both
            else if (stringValue.StartsWith("\"") && !stringValue.EndsWith("\""))
                GetEnclosedString("\"", lines, ref index, ref stringValue);
            else /* Then there are also lines without ' or " that just continues to the next line */
            {
                for (int x = index + 1; x < lines.Length; x++)
                {
                    if (lines[x].IndexOf(':') > 0)
                    {
                        index = x - 1;
                        break;
                    }
                    stringValue += "\r\n" + lines[x].Trim();
                }
            }

            var currentLevel = this.GetCurrentLevel(line);

            var value = new Yaml
            {
                Key = keyValue[0].Trim(),
                Value = stringValue.Trim(new char[] { '\'', '"' }).ConvertEscapeSequences(),
                Level = currentLevel
            };

            yaml.Children.Add(value);

            for (int i = index + 1; i < lines.Length; i++)
                if (this.GetCurrentLevel(lines[i]) > currentLevel)
                    i = this.ParseLevel(value, ref lines, i);
                else
                    return i - 1;

            return index;
        }

        private void SetValues(ref object item, Yaml yamlNode, Type targetType)
        {
            TypeAndAttribute[] yamlProperties;

            if (typeCache.ContainsKey(targetType))
                yamlProperties = typeCache[targetType];
            else
            {
                yamlProperties = targetType
                    .GetProperties()
                    .Select(x => new TypeAndAttribute { Attrib = x.GetCustomAttribute<YamlPropertyAttribute>(), PropertyInfo = x })
                    .Where(x => x.Attrib != null)
                    .ToArray();
                this.typeCache.Add(targetType, yamlProperties);
            }

            foreach (var p in yamlProperties)
            {
                string child;

                if (p.Attrib.Name == "key")
                    child = yamlNode.Key;
                else if (p.Attrib.Name == "keyValue")
                    child = yamlNode.Value;
                else
                    child = yamlNode.Children.FirstOrDefault(x => x.Key == p.Attrib.Name)?.Value;

                if (child == null)
                    continue;

                var value = child.Convert(p.PropertyInfo.PropertyType);

#if WINDOWS_UWP || NETCORE
                if (value == null && !p.PropertyInfo.PropertyType.GetTypeInfo().IsValueType && p.PropertyInfo.PropertyType != typeof(string))
#else
                if (value == null && !p.PropertyInfo.PropertyType.IsValueType && p.PropertyInfo.PropertyType != typeof(string))
#endif
                {
                    if (targetType.IsCollectionOrList())
                        value = this.Parse(yamlNode, p.PropertyInfo.PropertyType);
                    else
                    {
                        value = p.PropertyInfo.PropertyType.CreateInstance();
                        SetValues(ref value, yamlNode.Children.FirstOrDefault(x => x.Key == p.Attrib.Name), p.PropertyInfo.PropertyType);
                    }
                }

                p.PropertyInfo.SetValue(item, value);
            }
        }

        private class TypeAndAttribute
        {
            public YamlPropertyAttribute Attrib { get; set; }

            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}