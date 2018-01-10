using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        /// <summary>
        /// Checks all Methods that returns Task or Task`1 if their naming is according to the convention.
        /// </summary>
        /// <param name="builder"></param>
        public void CheckAsyncMthodsNomenclature(Builder builder)
        {
            var task = new __Task();
            var taskGeneric = new __Task_1();
            var methods = builder
                .GetTypes(SearchContext.Module)
                .SelectMany(x => x.Methods)
                .Where(x =>
                {
                    if (x.Fullname.IndexOf('<') >= 0 || x.Fullname.IndexOf('>') >= 0)
                        return false;

                    if (x.Name.EndsWith("Action"))
                        return false;

                    if (x.Name.EndsWith("Async"))
                        return false;

                    if (x.ReturnType == __Task.Type || x.ReturnType == __Task_1.Type)
                        return true;

                    return false;
                });

            foreach (var item in methods)
                this.Log(LogTypes.Warning, item, $"The method '{item.Name}' in '{item.OriginType.Fullname}' is async, but does not have an 'Async' suffix.");
        }
    }
}