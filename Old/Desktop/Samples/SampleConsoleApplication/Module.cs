public static class Module
{
    // This method will be weaved in the <Module> cctor and will be called everytime the module is loaded
    public static void OnLoad(string[] modules)
    {
    }
}