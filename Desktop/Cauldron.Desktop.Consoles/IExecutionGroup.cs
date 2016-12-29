namespace Cauldron.Consoles
{
    public interface IExecutionGroup
    {
        bool CanExecute { get; set; }

        void Execute(ParameterParser parser);
    }
}