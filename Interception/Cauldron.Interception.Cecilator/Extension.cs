namespace Cauldron.Interception.Cecilator
{
    public static class Extension
    {
        public static Builder CreateBuilder(this IWeaver weaver) => new Builder(weaver);
    }
}