using Couldron.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#else

using System.Windows;
using System.Windows.Data;

#endif

namespace Couldron.Behaviours
{
    public sealed partial class ControlTemplateBinding : Behaviour<FrameworkElement>
    {
        #region Dependency Property SourceType

        /// <summary>
        /// Identifies the <see cref="SourceType" /> dependency property
        /// </summary>
        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register(nameof(SourceType), typeof(Type), typeof(ControlTemplateBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="SourceType" /> Property
        /// </summary>
        public Type SourceType
        {
            get { return (Type)this.GetValue(SourceTypeProperty); }
            set { this.SetValue(SourceTypeProperty, value); }
        }

        #endregion Dependency Property SourceType

        #region Dependency Property TargetProperty

        /// <summary>
        /// Identifies the <see cref="TargetProperty" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TargetPropertyProperty = DependencyProperty.Register(nameof(TargetProperty), typeof(DependencyProperty), typeof(ControlTemplateBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="TargetProperty" /> Property
        /// </summary>
        public DependencyProperty TargetProperty
        {
            get { return (DependencyProperty)this.GetValue(TargetPropertyProperty); }
            set { this.SetValue(TargetPropertyProperty, value); }
        }

        #endregion Dependency Property TargetProperty

        #region Dependency Property TargetBehaviourName

        /// <summary>
        /// Identifies the <see cref="TargetBehaviourName" /> dependency property
        /// </summary>
        public static readonly DependencyProperty TargetBehaviourNameProperty = DependencyProperty.Register(nameof(TargetBehaviourName), typeof(string), typeof(ControlTemplateBinding), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="TargetBehaviourName" /> Property
        /// </summary>
        public string TargetBehaviourName
        {
            get { return (string)this.GetValue(TargetBehaviourNameProperty); }
            set { this.SetValue(TargetBehaviourNameProperty, value); }
        }

        #endregion Dependency Property TargetBehaviourName

        #region Dependency Property Converter

        /// <summary>
        /// Identifies the <see cref="Converter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register(nameof(Converter), typeof(IValueConverter), typeof(ControlTemplateBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Converter" /> Property
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)this.GetValue(ConverterProperty); }
            set { this.SetValue(ConverterProperty, value); }
        }

        #endregion Dependency Property Converter

        #region Dependency Property ConverterParameter

        /// <summary>
        /// Identifies the <see cref="ConverterParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty = DependencyProperty.Register(nameof(ConverterParameter), typeof(object), typeof(ControlTemplateBinding), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ConverterParameter" /> Property
        /// </summary>
        public object ConverterParameter
        {
            get { return (object)this.GetValue(ConverterParameterProperty); }
            set { this.SetValue(ConverterParameterProperty, value); }
        }

        #endregion Dependency Property ConverterParameter

        #region Dependency Property Mode

        /// <summary>
        /// Identifies the <see cref="Mode" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(BindingMode), typeof(ControlTemplateBinding), new PropertyMetadata(BindingMode.OneWay));

        /// <summary>
        /// Gets or sets the <see cref="Mode" /> Property
        /// </summary>
        public BindingMode Mode
        {
            get { return (BindingMode)this.GetValue(ModeProperty); }
            set { this.SetValue(ModeProperty, value); }
        }

        #endregion Dependency Property Mode

        protected override void OnAttach()
        {
        }

        protected override void OnDataContextChanged()
        {
            if (this.SourceProperty == null || this.TargetProperty == null || this.SourceType == null)
                return;

            var source = this.AssociatedObject.FindParent(this.SourceType);

            if (source == null)
                return;

            if (string.IsNullOrEmpty(this.TargetBehaviourName))
            {
                BindingOperations.SetBinding(this.AssociatedObject, this.TargetProperty, new Binding
                {
                    Source = source,
#if NETFX_CORE
                    Path = new PropertyPath(this.SourceProperty),
#else
                    Path = new PropertyPath(this.SourceProperty.Name),
#endif
                    Converter = this.Converter,
                    ConverterParameter = this.ConverterParameter,
                    Mode = this.Mode,
                });
            }
            else
            {
                var behaviour = this.GetBehaviour(Interaction.GetBehaviours(this.AssociatedObject));

                if (behaviour == null)
                    return;

                BindingOperations.SetBinding(behaviour as DependencyObject, this.TargetProperty, new Binding
                {
                    Source = source,
#if NETFX_CORE
                    Path = new PropertyPath(this.SourceProperty),
#else
                    Path = new PropertyPath(this.SourceProperty.Name),
#endif
                    Converter = this.Converter,
                    ConverterParameter = this.ConverterParameter,
                    Mode = this.Mode,
                });
            }
        }

        protected override void OnDetach()
        {
        }

        private IBehaviour GetBehaviour(BehaviourCollection behaviourCollection)
        {
            foreach (var behaviour in behaviourCollection)
            {
                if (behaviour.Name == this.TargetBehaviourName)
                    return behaviour;
                else if (behaviour.GetType() == typeof(EventTrigger))
                {
                    foreach (var action in behaviour.CastTo<EventTrigger>().Events)
                        if (action.Name == this.TargetBehaviourName)
                            return action;
                }
            }

            return null;
        }
    }
}