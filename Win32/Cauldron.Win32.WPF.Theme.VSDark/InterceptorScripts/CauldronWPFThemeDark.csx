using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

public static class CauldronWPFThemeDark
{
    public const string Name = "Cauldron WPF Theme";
    public const int Priority = int.MaxValue;

    [Display("Adding WPF Theme Reference")]
    public static void Implement(Builder builder)
    {
        builder
            .GetType("<Module>", SearchContext.Module)
            .CreateStaticConstructor()
            .NewCoder()
            .Load(builder.GetType("Cauldron.XAML.Theme.CauldronTheme").Import()).Pop()
            .Load(builder.GetType("Cauldron.XAML.Theme.VSDark.ColorsAndBrushes").Import()).Pop()
            .Insert(InsertionPosition.End);
    }
}