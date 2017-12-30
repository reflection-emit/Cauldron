﻿using Cauldron.Interception.Cecilator;
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
                .Where(x => (x.ReturnType == __Task.Type || x.ReturnType == __Task_1.Type) && !x.Name.EndsWith("Async") && !x.Name.EndsWith("Action"));

            foreach (var item in methods)
                this.Log(LogTypes.Warning, item, $"The method '{item.Name}' in '{item.OriginType.Fullname}' is async, but does not have an 'Async' suffix.");
        }
    }
}