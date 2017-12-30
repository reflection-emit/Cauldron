using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private void ImplementPropertyChangedEvent(Builder builder)
        {
            var changeAwareInterface = new __IChangeAwareViewModel();
            var eventHandlerGeneric = new __EventHandler_1();
            var eventHandler = new __EventHandler();
            var viewModelInterface = new __IViewModel();

            // Get all viewmodels with implemented change aware interface
            var viewModels = builder.FindTypesByInterface(__IChangeAwareViewModel.Type)
                .OrderBy(x =>
                {
                    if (x.Implements(__IChangeAwareViewModel.Type, false))
                        return 0;

                    return 1;
                });

            foreach (var vm in viewModels)
            {
                if (vm.IsInterface)
                    continue;

                var method = vm.GetMethod("<>RaisePropertyChangedEventRaise", false, typeof(string), typeof(object), typeof(object));
                var getIsChangeChangedEvent = __IChangeAwareViewModel.GetIsChangedChanged(vm);
                var getIsChangeEvent = __IChangeAwareViewModel.GetChanged(vm);

                if (method == null && getIsChangeChangedEvent != null && getIsChangeEvent != null)
                {
                    method = vm.CreateMethod(Modifiers.Protected, "<>RaisePropertyChangedEventRaise", typeof(string), typeof(object), typeof(object));
                    method.NewCode()
                            .Load(Crumb.GetParameter(0)).EqualTo("IsChanged").Then(x =>
                            {
                                x.Load(getIsChangeChangedEvent).IsNotNull().Then(y =>
                                    y.Callvirt(getIsChangeChangedEvent, eventHandler.Invoke, Crumb.This, y.NewCode().NewObj(eventHandler.EventArgs.Ctor)))
                                        .Call(Crumb.This, viewModelInterface.RaisePropertyChanged, Crumb.GetParameter(0))
                                        .Return();
                            })
                            .Load(getIsChangeEvent)
                            .IsNotNull()
                            .Then(x =>
                            {
                                x.Call(Crumb.This, viewModelInterface.RaisePropertyChanged, Crumb.GetParameter(0));
                                x.Callvirt(eventHandlerGeneric.Invoke.MakeGeneric(changeAwareInterface.PropertyIsChangedEventArgs.ToBuilderType),
                                    x.NewCode().Load(getIsChangeEvent), x.NewCode().NewObj(changeAwareInterface.PropertyIsChangedEventArgs.Ctor, Crumb.This, Crumb.GetParameter(0), Crumb.GetParameter(1), Crumb.GetParameter(2)));
                            })
                            .Return()
                            .Replace();
                    method.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
                }

                if (method == null)
                    continue;

                this.Log($"Implementing RaisePropertyChanged Raise Event in '{vm.Fullname}'");
                var raisePropertyChanged = vm.GetMethod("RaisePropertyChanged", false, typeof(string), typeof(object), typeof(object));

                if (raisePropertyChanged == null)
                    continue;

                if (!raisePropertyChanged.IsAbstract && !raisePropertyChanged.HasMethodBaseCall())
                    raisePropertyChanged
                        .NewCode()
                        .Context(x => x.Call(Crumb.This, method, Crumb.GetParameter(0), Crumb.GetParameter(1), Crumb.GetParameter(2)))
                        .Insert(InsertionPosition.Beginning);

                // Repair IsChanged
                if (!vm.Implements(changeAwareInterface.ToBuilderType, false))
                    continue;

                var isChangedSetter = vm.GetMethod("set_IsChanged", 1, false);
                if (isChangedSetter != null)
                {
                    isChangedSetter.NewCode()
                        .Call(Crumb.This, viewModelInterface.IsLoading)
                        .IsTrue()
                        .Then(x => x.Return())
                        .Insert(InsertionPosition.Beginning);
                }
            }
        }
    }
}