using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Represents an action that opens a <see cref="Microsoft.Win32.OpenFileDialog"/>
    /// </summary>
    public class OpenFileDialogAction : ActionBase
    {
        #region Dependency Property Filter

        /// <summary>
        /// Identifies the <see cref="Filter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string), typeof(OpenFileDialogAction), new PropertyMetadata("*.*|All Files"));

        /// <summary>
        /// Gets or sets the <see cref="Filter" /> Property
        /// </summary>
        public string Filter
        {
            get { return (string)this.GetValue(FilterProperty); }
            set { this.SetValue(FilterProperty, value); }
        }

        #endregion Dependency Property Filter

        #region Dependency Property Multiselect

        /// <summary>
        /// Identifies the <see cref="Multiselect" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MultiselectProperty = DependencyProperty.Register(nameof(Multiselect), typeof(bool), typeof(OpenFileDialogAction), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="Multiselect" /> Property
        /// </summary>
        public bool Multiselect
        {
            get { return (bool)this.GetValue(MultiselectProperty); }
            set { this.SetValue(MultiselectProperty, value); }
        }

        #endregion Dependency Property Multiselect

        #region Dependency Property CheckFileExists

        /// <summary>
        /// Identifies the <see cref="CheckFileExists" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CheckFileExistsProperty = DependencyProperty.Register(nameof(CheckFileExists), typeof(bool), typeof(OpenFileDialogAction), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the <see cref="CheckFileExists" /> Property
        /// </summary>
        public bool CheckFileExists
        {
            get { return (bool)this.GetValue(CheckFileExistsProperty); }
            set { this.SetValue(CheckFileExistsProperty, value); }
        }

        #endregion Dependency Property CheckFileExists

        #region Dependency Property Filename

        /// <summary>
        /// Identifies the <see cref="Filename" /> dependency property
        /// </summary>
        public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register(nameof(Filename), typeof(string), typeof(OpenFileDialogAction), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the <see cref="Filename" /> Property
        /// </summary>
        public string Filename
        {
            get { return (string)this.GetValue(FilenameProperty); }
            set { this.SetValue(FilenameProperty, value); }
        }

        #endregion Dependency Property Filename

        /// <summary>
        /// Occures when the action is invoked by an event
        /// </summary>
        /// <param name="parameter">The parameter passed by the event</param>
        public override void Invoke(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = this.Filter;
            openFileDialog.Multiselect = this.Multiselect;
            openFileDialog.CheckFileExists = this.CheckFileExists;
            openFileDialog.InitialDirectory = string.IsNullOrEmpty(this.Filename) ?
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) : Path.GetDirectoryName(this.Filename);

            var result = openFileDialog.ShowDialog(this.AssociatedObject.FindVisualParent<Window>());

            if (result.HasValue && result.Value)
                this.Filename = openFileDialog.FileName;
        }

        /// <summary>
        /// Occures when the behavior is attached to the object
        /// </summary>
        protected override void OnAttach()
        {
        }

        /// <summary>
        /// Occures when the behaviour is detached from the object
        /// </summary>
        protected override void OnDetach()
        {
        }
    }
}