namespace Cauldron.Consoles
{
    /// <summary>
    /// Represents an execution group
    /// </summary>
    public interface IExecutionGroup
    {
        /// <summary>
        /// Gets or sets a value indicating if the <see cref="ParameterParser"/> can execute this group
        /// </summary>
        bool CanExecute { get; set; }

        /// <summary>
        /// Starts the execution of the group
        /// </summary>
        /// <param name="parser">The <see cref="ParameterParser"/> instanced that executed the group</param>
        void Execute(ParameterParser parser);
    }
}