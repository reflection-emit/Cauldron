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
            var task = new __Task(builder);
            var taskGeneric = new __Task_1(builder);
            var methods = builder
                .GetTypes(SearchContext.Module)
                .SelectMany(x => x.Methods)
                .Where(x => (x.ReturnType == task.Type || x.ReturnType == taskGeneric.Type) && !x.Name.EndsWith("Async") && !x.Name.EndsWith("Action"));

            foreach (var item in methods)
                this.Log(LogTypes.Warning, item, $"The method '{item.Name}' in '{item.DeclaringType.Fullname}' is async, but does not have an 'Async' suffix.");
        }
    }
}