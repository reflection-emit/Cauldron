namespace Cauldron.Yaml
{
    internal class Yaml
    {
        public Yaml() => this.Children = new YamlCollection(this);

        public YamlCollection Children { get; private set; }

        public string Key { get; set; }

        public int Level { get; set; }

        public Yaml Parent { get; set; }

        public string Value { get; set; }

        public override string ToString() => this.Key + " : " + this.Value;
    }
}