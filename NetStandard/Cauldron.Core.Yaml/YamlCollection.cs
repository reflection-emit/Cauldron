using System.Collections.ObjectModel;

namespace Cauldron.Core.Yaml
{
    internal class YamlCollection : Collection<Yaml>
    {
        private Yaml parent;

        public YamlCollection(Yaml parent) => this.parent = parent;

        protected override void InsertItem(int index, Yaml item)
        {
            item.Parent = this.parent;
            base.InsertItem(index, item);
        }
    }
}