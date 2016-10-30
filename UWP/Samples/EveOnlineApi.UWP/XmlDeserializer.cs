using Cauldron.Core;
using Cauldron.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

#if WINDOWS_UWP

using Windows.Data.Xml.Dom;

#else

#endif

namespace EveOnlineApi
{
    /*
     * We have to create our own xml deserializer based on XmlDocument to deal uncommon and unconventional (or malformed values) xmls
     */

    internal struct ObjectFactoryStructure
    {
        public string delimeter;
        public string value;
    }

    internal sealed class XmlDeserializer<T> : DisposableBase
    {
        private static ConcurrentDictionary<Type, Dictionary<PropertyInfo, Dictionary<Type, Attribute>>> cache =
            new ConcurrentDictionary<Type, Dictionary<PropertyInfo, Dictionary<Type, Attribute>>>();

        private static object syncRoot = new object();
        private NumberFormatInfo numberFormatInfoEnUs;

        public XmlDeserializer(string xmlContent)
        {
            this.XmlContent = xmlContent;
            this.numberFormatInfoEnUs = new CultureInfo("en-US").NumberFormat;
        }

        public string XmlContent { get; private set; }

        public T Deserialize()
        {
            var xml = new XmlDocument();
            xml.LoadXml(this.XmlContent);

            var context = Activator.CreateInstance<T>();
            this.Parse(xml.LastChild, context);

            return context;
        }

        protected override void OnDispose(bool disposeManaged)
        {
        }

        private void Parse(
#if WINDOWS_UWP
            IXmlNode node,
#else
            XmlNode node,
#endif
            object context)
        {
            var type = context.GetType();

            if (!cache.ContainsKey(type))
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var propertyList = new Dictionary<PropertyInfo, Dictionary<Type, Attribute>>();

                foreach (var p in properties)
                {
                    var attributes = new Dictionary<Type, Attribute>();
                    var attrib1 = p.GetCustomAttribute<XmlDeserializerElementAttribute>();
                    var attrib2 = p.GetCustomAttribute<XmlDeserializerAttributeAttribute>();
                    var attrib3 = p.GetCustomAttribute<XmlDeserializerNodePathAttribute>();
                    var attrib4 = p.GetCustomAttribute<XmlDeserializerInnerTextAttribute>();

                    if (attrib1 != null) attributes.Add(typeof(XmlDeserializerElementAttribute), attrib1);
                    if (attrib2 != null) attributes.Add(typeof(XmlDeserializerAttributeAttribute), attrib2);
                    if (attrib3 != null) attributes.Add(typeof(XmlDeserializerNodePathAttribute), attrib3);
                    if (attrib4 != null) attributes.Add(typeof(XmlDeserializerInnerTextAttribute), attrib4);

                    propertyList.Add(p, attributes);
                    cache.TryAdd(type, propertyList);
                }
            }

            var paa = cache[type];

            foreach (var property in paa)
            {
                var currentNode = node;

                if (property.Value.ContainsKey(typeof(XmlDeserializerNodePathAttribute)))
                {
                    var attr = property.Value[typeof(XmlDeserializerNodePathAttribute)] as XmlDeserializerNodePathAttribute;

                    if (string.IsNullOrEmpty(attr.Attribute))
                        currentNode = node.SelectSingleNode(attr.NodePath);
                    else
                        currentNode = node.GetNode(attr.NodePath, attr.Attribute, attr.AttributeValue);
                }

                if (property.Value.ContainsKey(typeof(XmlDeserializerElementAttribute)))
                    this.ParseElement(currentNode, property.Key, property.Value[typeof(XmlDeserializerElementAttribute)] as XmlDeserializerElementAttribute, context);

                if (property.Value.ContainsKey(typeof(XmlDeserializerAttributeAttribute)))
                    this.ParseAttribute(currentNode, property.Key, property.Value[typeof(XmlDeserializerAttributeAttribute)] as XmlDeserializerAttributeAttribute, context);

                if (property.Value.ContainsKey(typeof(XmlDeserializerInnerTextAttribute)))
                {
                    this.PropertySetter(context, property.Key, new ObjectFactoryStructure
                    {
                        delimeter = "\r\n",
                        value = node.InnerText
                    });
                }
            }
        }

        private void ParseAttribute(
#if WINDOWS_UWP
            IXmlNode node,
#else
            XmlNode node,
#endif
            PropertyInfo propertyInfo, XmlDeserializerAttributeAttribute attrib, object context)
        {
            if (node.Attributes.Contains(attrib.Name))
            {
                this.PropertySetter(context, propertyInfo, new ObjectFactoryStructure
                {
                    delimeter = ",",
                    value = node.Attributes.GetNamedItem(attrib.Name).InnerText
                });
            }
        }

        private void ParseElement(
#if WINDOWS_UWP
            IXmlNode node,
#else
            XmlNode node,
#endif
            PropertyInfo propertyInfo, XmlDeserializerElementAttribute attrib, object context)
        {
            if (node.Contains(attrib.ElementName))
            {
                if (attrib.IsEnumerable)
                {
#if WINDOWS_UWP
                    var collection = node.SelectSingleNode(attrib.ElementName).ChildNodes.Where(x => x.NodeType == NodeType.ElementNode).ToArray();
#else
                    var collection = node.SelectSingleNode(attrib.ElementName).ChildNodes.Cast<XmlNode>().Where(x => x.NodeType == XmlNodeType.Element).ToArray();
#endif

                    this.PropertySetterCollectionOrArray(context, propertyInfo, collection.Length, (i, t) =>
                    {
                        var o = Activator.CreateInstance(t);
                        this.Parse(collection[i], o);

                        return o;
                    });
                }
                else
                {
                    this.PropertySetter(context, propertyInfo, new ObjectFactoryStructure
                    {
                        value = node.SelectSingleNode(attrib.ElementName).InnerText,
                        delimeter = ","
                    });
                }
            }
#if WINDOWS_UWP
            else if (node.NodeName == attrib.ElementName)
#else
            else if (node.Name == attrib.ElementName)
#endif
            {
                if (attrib.IsEnumerable)
                {
                    this.PropertySetterCollectionOrArray(context, propertyInfo, node.ChildNodes.Count, (i, t) =>
                    {
                        var o = Activator.CreateInstance(t);
                        this.Parse(node.ChildNodes[i], o);

                        return o;
                    });
                }
                else
                {
                    this.PropertySetter(context, propertyInfo, new ObjectFactoryStructure
                    {
                        value = node.InnerText,
                        delimeter = ","
                    });
                }
            }
        }

        private bool PropertySetter(object context, PropertyInfo property, ObjectFactoryStructure ofs)
        {
            var trimedValue = ofs.value.Trim();
            var nullableUnderlyingType = Nullable.GetUnderlyingType(property.PropertyType);

            if (nullableUnderlyingType != null && string.IsNullOrEmpty(trimedValue))
            {
                // If we have a nullable type and the value is null, we return null
                property.SetValue(context, null);
                return false;
            }
            // This is just a simple array / list without further properties... Like a list of strings
            else if (property.PropertyType.IsArray || property.PropertyType.IsCollectionOrList())
            {
                string[] values = ofs.value
                    .Split(new string[] { ofs.delimeter == null ? "," : ofs.delimeter }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToArray();

                // Get the converter
                var elementType = property.PropertyType.GetChildrenType();
                return this.PropertySetterCollectionOrArray(context, property, values.Length, (index, et) => values[index].Convert(et, this.numberFormatInfoEnUs));
            }
            else
            {
                var toType = nullableUnderlyingType == null ? property.PropertyType : nullableUnderlyingType;
                property.SetValue(context, trimedValue.Convert(toType, this.numberFormatInfoEnUs));
            }

            return true;
        }

        private bool PropertySetterCollectionOrArray(object context, PropertyInfo property, int childCount, Func<int, Type, object> objectFactory)
        {
            var elementType = property.PropertyType.GetChildrenType();
            var convertedValues = Array.CreateInstance(elementType, childCount);

            if (property.PropertyType.IsArray)
            {
                for (int i = 0; i < childCount; i++)
                    convertedValues.SetValue(objectFactory(i, elementType), i);

                property.SetValue(context, convertedValues);
            }
            else
            {
                // Create a new collection / list
                var list = Activator.CreateInstance(property.PropertyType);
                var addMethod = property.PropertyType.GetRuntimeMethod("Add", new Type[] { elementType });

                // if this has no add method with 1 parameter... then lets skip this one
                if (addMethod == null)
                    return false;

                for (int i = 0; i < childCount; i++)
                    addMethod.Invoke(list, new object[] { objectFactory(i, elementType) });

                property.SetValue(context, list);
            }

            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class XmlDeserializerAttributeAttribute : Attribute
    {
        public XmlDeserializerAttributeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class XmlDeserializerElementAttribute : Attribute
    {
        public XmlDeserializerElementAttribute(string elementName) : this(elementName, false)
        {
        }

        public XmlDeserializerElementAttribute(string elementName, bool isEnumerable)
        {
            this.IsEnumerable = isEnumerable;
            this.ElementName = elementName;
        }

        public string ElementName { get; private set; }

        public bool IsEnumerable { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class XmlDeserializerInnerTextAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class XmlDeserializerNodePathAttribute : Attribute
    {
        public XmlDeserializerNodePathAttribute(string nodePath)
        {
            this.NodePath = nodePath;
        }

        public XmlDeserializerNodePathAttribute(string nodePath, string attributeName, string attributeValue) : this(nodePath)
        {
            this.AttributeValue = attributeValue;
            this.Attribute = attributeName;
            this.HasSpecificAttribute = true;
        }

        public string Attribute { get; private set; }

        public string AttributeValue { get; private set; }

        public bool HasSpecificAttribute { get; private set; }

        public string NodePath { get; private set; }
    }
}