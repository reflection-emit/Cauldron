using Cauldron.Interception;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cauldron.Core.Interceptors
{
    /// <summary>
    /// Provides an interceptor that can read and write registry values.
    /// It supports the following types:
    /// <see cref="int"/>,
    /// <see cref="uint"/>,
    /// <see cref="long"/>,
    /// <see cref="ulong"/>,
    /// <see cref="string"/>,
    /// <see cref="string"/>[],
    /// <see cref="byte"/>[],
    /// <see cref="DirectoryInfo"/> and
    /// <see cref="FileInfo"/>.
    /// <para/>
    /// This interceptor can be applied on a single property. If you require a class wide registry parsing use <see cref="RegistryClassAttribute"/> instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RegistryAttribute : Attribute, IPropertyInterceptor
    {
        private object defaultValue;
        private string name;
        private RegistryHive registryHive;
        private RegistryView registryView;
        private string subKey;

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>. The name that is used is the property name.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        public RegistryAttribute(RegistryHive registryHive, string subKey) :
            this(registryHive, RegistryView.Default, subKey, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>. The name that is used is the property name.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        /// <param name="defaultValue">The value to return if name does not exist.</param>
        public RegistryAttribute(RegistryHive registryHive, string subKey, object defaultValue) :
            this(registryHive, RegistryView.Default, subKey, null, defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>. The name that is used is the property name.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="registryView">The registry view to use.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        /// <param name="defaultValue">The value to return if name does not exist.</param>
        public RegistryAttribute(RegistryHive registryHive, RegistryView registryView, string subKey, object defaultValue) :
            this(registryHive, registryView, subKey, null, defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <param name="defaultValue">The value to return if <paramref name="name"/> does not exist.</param>
        public RegistryAttribute(RegistryHive registryHive, string subKey, string name, object defaultValue) :
            this(registryHive, RegistryView.Default, subKey, name, defaultValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="registryView">The registry view to use.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <param name="defaultValue">The value to return if <paramref name="name"/> does not exist.</param>
        public RegistryAttribute(RegistryHive registryHive, RegistryView registryView, string subKey, string name, object defaultValue)
        {
            this.registryHive = registryHive;
            this.registryView = registryView;
            this.name = name;
            this.defaultValue = defaultValue;
            this.subKey = subKey ?? throw new ArgumentNullException(nameof(subKey));
        }

        /// <exclude/>
        public void OnException(Exception e)
        {
        }

        /// <exclude/>
        public void OnExit()
        {
        }

        /// <exclude/>
        public void OnGet(PropertyInterceptionInfo propertyInterceptionInfo, object value)
        {
            if (this.defaultValue == null)
                this.defaultValue = value;

            using (var registryKey = RegistryKey.OpenBaseKey(this.registryHive, this.registryView))
            {
                using (var subKey = registryKey.CreateSubKey(this.subKey))
                {
                    var result = ConvertToSystemType(subKey.GetValue(this.name ?? propertyInterceptionInfo.PropertyName,
                        this.defaultValue,
                        RegistryValueOptions.None), propertyInterceptionInfo.PropertyType);

                    // If our property's value is not the same as the value in the registry... Let us update
                    // our property's value
                    propertyInterceptionInfo.SetValue(result);
                }
            }
        }

        /// <exclude/>
        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            using (var registryKey = RegistryKey.OpenBaseKey(this.registryHive, this.registryView))
            {
                using (var subKey = registryKey.CreateSubKey(this.subKey))
                {
                    var result = ConvertToSystemType(subKey.GetValue(this.name ?? propertyInterceptionInfo.PropertyName,
                        this.defaultValue,
                        RegistryValueOptions.None), propertyInterceptionInfo.PropertyType);

                    subKey.SetValue(this.name ?? propertyInterceptionInfo.PropertyName,
                        ConvertToRegistryValue(newValue, propertyInterceptionInfo.PropertyType),
                        GetValueKind(propertyInterceptionInfo.PropertyType));
                }
            }

            return false;
        }

        private object ConvertToRegistryValue(object value, Type dataType)
        {
            if (dataType == typeof(string) && value == null)
                return "";

            if (dataType == typeof(string[]) && value == null)
                return new string[0];

            if (dataType == typeof(byte[]) && value == null)
                return new byte[0];

            if (dataType == typeof(DirectoryInfo))
                return ReplacePathWithEnvironmentVariables((value as DirectoryInfo)?.FullName);

            if (dataType == typeof(FileInfo))
                return ReplacePathWithEnvironmentVariables((value as FileInfo)?.FullName);

            if (dataType == typeof(ulong))
                return unchecked((long)(ulong)value + long.MinValue);

            if (dataType == typeof(uint))
                return unchecked((int)(uint)value + int.MinValue);

            return value;
        }

        private object ConvertToSystemType(object value, Type dataType)
        {
            if (dataType == typeof(ulong))
                return unchecked((ulong)((long)value - long.MinValue));

            if (dataType == typeof(uint))
                return unchecked((uint)((int)value - int.MinValue));

            if (dataType == typeof(DirectoryInfo) && value is string path)
                try
                {
                    return string.IsNullOrEmpty(path) ? null : new DirectoryInfo(path);
                }
                catch
                {
                    return null;
                }

            if (dataType == typeof(FileInfo) && value is string filepath)
                try
                {
                    return string.IsNullOrEmpty(filepath) ? null : new FileInfo(filepath);
                }
                catch
                {
                    return null;
                }

            return value;
        }

        private RegistryValueKind GetValueKind(Type type)
        {
            if (type == typeof(string))
                return RegistryValueKind.String;

            if (type == typeof(DirectoryInfo) || type == typeof(FileInfo))
                return RegistryValueKind.ExpandString;

            if (type == typeof(int) || type == typeof(uint))
                return RegistryValueKind.DWord;

            if (type == typeof(long) || type == typeof(ulong))
                return RegistryValueKind.QWord;

            if (type == typeof(byte[]))
                return RegistryValueKind.Binary;

            if (type == typeof(string[]))
                return RegistryValueKind.MultiString;

            return RegistryValueKind.Unknown;
        }

        private string ReplacePathWithEnvironmentVariables(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            var list = new List<(string, string)>();
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
                list.Add((item.Key as string, item.Value as string));

            foreach (var item in list.OrderByDescending(x => x.Item2.Length).ThenBy(x => x.Item2).ThenBy(x => x.Item1))
            {
                var env = item.Item2;

                if (env.Length > path.Length)
                    continue;

                bool isEqual = true;
                for (int i = 0; i < item.Item2.Length; i++)
                {
                    if (env[i] == path[i] || char.ToUpper(env[i]) == char.ToUpper(path[i]))
                        continue;

                    isEqual = false;
                    break;
                }

                if (isEqual)
                {
                    if (env.Length == path.Length)
                        return $"%{item.Item1}%";

                    var staticPath = path.Substring(env.Length);
                    if (staticPath[0] == '\\')
                        return $"%{item.Item1}%{staticPath}";

                    return $"%{item.Item1}%\\{staticPath}";
                }
            }

            return path;
        }
    }

    /// <summary>
    /// Provides an interceptor that can read and write registry values.
    /// It supports the following types:
    /// <see cref="int"/>,
    /// <see cref="uint"/>,
    /// <see cref="long"/>,
    /// <see cref="ulong"/>,
    /// <see cref="string"/>,
    /// <see cref="string"/>[],
    /// <see cref="byte"/>[],
    /// <see cref="DirectoryInfo"/> and
    /// <see cref="FileInfo"/>.
    /// <para/>
    /// This interceptor can be applied to a class and will intercept all properties declared in the class.
    /// If you do not wish to intercept a property decorate it with the <see cref="DoNotInterceptAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RegistryClassAttribute : RegistryAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>. The name that is used is the property name.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        public RegistryClassAttribute(RegistryHive registryHive, string subKey) : base(registryHive, subKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryAttribute"/>. The name that is used is the property name.
        /// </summary>
        /// <param name="registryHive">The HKEY to open.</param>
        /// <param name="registryView">The registry view to use.</param>
        /// <param name="subKey">The name or path of the subkey to create or open.</param>
        public RegistryClassAttribute(RegistryHive registryHive, RegistryView registryView, string subKey) :
            base(registryHive, registryView, subKey, null)
        {
        }
    }
}