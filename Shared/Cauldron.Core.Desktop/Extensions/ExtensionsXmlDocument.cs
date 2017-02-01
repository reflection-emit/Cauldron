using System;
using System.Linq;
using System.Xml;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides usefull extension methods for <see cref="XmlDocument"/>
    /// </summary>
    public static class ExtensionsXmlDocument
    {
        /// <summary>
        /// Checks if the node contains a child with the name defined by <paramref name="nodeName"/>
        /// </summary>
        /// <param name="target">The node to check</param>
        /// <param name="nodeName">The name of the node to find</param>
        /// <returns>Returns true if found; otherwise false</returns>
        public static bool Contains(this XmlNode target, string nodeName)
        {
            try
            {
                for (int i = 0; i < target.ChildNodes.Count; i++)
                    if (target.ChildNodes[i].Name == nodeName)
                        return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an attribute with the name defined by <paramref name="attributeName"/> exists in the attrbute collection
        /// </summary>
        /// <param name="target">The attribute collection to check</param>
        /// <param name="attributeName">The name of the attribute to find</param>
        /// <returns>Returns true if found; otherwise false</returns>
        public static bool Contains(this XmlAttributeCollection target, string attributeName)
        {
            for (int i = 0; i < target.Count; i++)
                if (target[i].Name == attributeName)
                    return true;

            return false;
        }

        /// <summary>
        /// Counts the number of childs that have the type defined by <paramref name="type"/>
        /// </summary>
        /// <param name="target">The starting node</param>
        /// <param name="type">The type of child to count</param>
        /// <returns>Returns the number of children</returns>
        public static int CountOf(this XmlNodeList target, XmlNodeType type)
        {
            int count = 0;

            for (int i = 0; i < target.Count; i++)
                if (target[i].NodeType == type)
                    count++;

            return count;
        }

        /// <summary>
        /// Gets a node defined by <paramref name="nodePath"/>
        /// </summary>
        /// <param name="target">The starting node</param>
        /// <param name="nodePath">A node path separated by /</param>
        /// <param name="attributeName">The name of the attribute to retrieve</param>
        /// <param name="attributeValue">The value that the attribute <paramref name="attributeName"/> should have</param>
        /// <returns>Returns the node if successfull; otherwise null</returns>
        public static XmlNode GetNode(this XmlNode target, string nodePath, string attributeName, string attributeValue)
        {
            XmlNode element = target;

            try
            {
                var nodeNames = nodePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < nodeNames.Length; i++)
                {
                    var name = nodeNames[i];
                    var elements = element.ChildNodes.Cast<XmlElement>().ToArray().Where(x => x.Name == name);

                    if (elements.Count() > 1)
                        element = elements.FirstOrDefault(x => x.Attributes.Contains(attributeName) && x.Attributes[attributeName].InnerText == attributeValue);
                    else
                        element = elements.First();
                }
            }
            catch
            {
                return null;
            }

            return element;
        }

        /// <summary>
        /// Gets the concatenated values of the node and all its child nodes.
        /// </summary>
        /// <param name="target">The xml document</param>
        /// <param name="nodePath">A node path separated by /</param>
        /// <returns>The concatenated values of the node and all its child nodes.</returns>
        public static string GetNodeInnerText(this XmlDocument target, string nodePath)
        {
            var val = target.SelectSingleNode(nodePath);

            if (val == null)
                return "";
            else
                return val.InnerText;
        }
    }
}