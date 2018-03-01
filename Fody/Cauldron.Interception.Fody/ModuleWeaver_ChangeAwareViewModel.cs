using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Cecilator.Extensions;
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
                    method.NewCoder()
                        .If(x => x.Load(CodeBlocks.GetParameter(0)).Is("IsChanged"), then =>

                                  then.If(z => z.Load(getIsChangeChangedEvent).IsNotNull(), thenInner =>
                                      thenInner.Load(getIsChangeChangedEvent).Call(eventHandler.Invoke, CodeBlocks.This, thenInner.NewCoder().NewObj(eventHandler.EventArgs.Ctor)))
                                          .Call(viewModelInterface.RaisePropertyChanged, CodeBlocks.GetParameter(0))
                                          .Return()
                             )
                        .If(x => x.Load(getIsChangeEvent).IsNotNull(), then =>

                                 then.Call(viewModelInterface.RaisePropertyChanged, CodeBlocks.GetParameter(0))
                                     .Call(eventHandlerGeneric.Invoke.MakeGeneric(changeAwareInterface.PropertyIsChangedEventArgs.ToBuilderType),
                                     x => x.Load(getIsChangeEvent),
                                     x => x.NewObj(changeAwareInterface.PropertyIsChangedEventArgs.Ctor, CodeBlocks.This, CodeBlocks.GetParameter(0), CodeBlocks.GetParameter(1), CodeBlocks.GetParameter(2)))

                            )
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
                        .NewCoder()
                        .Call(method, CodeBlocks.GetParameter(0), CodeBlocks.GetParameter(1), CodeBlocks.GetParameter(2))
                        .End
                        .Insert(InsertionPosition.Beginning);

                // Repair IsChanged
                if (!vm.Implements(changeAwareInterface.ToBuilderType, false))
                    continue;

                var isChangedSetter = vm.GetMethod("set_IsChanged", 1, false);
                if (isChangedSetter != null)
                {
                    isChangedSetter.NewCoder()
                        .If(x => x.Call(viewModelInterface.IsLoading).Is(true),
                            then => then.Return())
                        .Insert(InsertionPosition.Beginning);
                }
            }
        }
    }
}