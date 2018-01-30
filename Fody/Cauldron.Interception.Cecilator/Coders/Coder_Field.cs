namespace Cauldron.Interception.Cecilator.Coders
{
    public partial class Coder
    {
        public FieldAssignCoder Load(Field field) => new FieldAssignCoder(this, field);
    }
}