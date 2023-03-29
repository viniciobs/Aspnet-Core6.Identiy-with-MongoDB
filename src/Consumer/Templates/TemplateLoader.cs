namespace Consumer.Templates
{
    internal static class TemplateLoader
    {
        public static string LoadByName(string name)
        {
            var path = Directory.GetCurrentDirectory() + "\\Templates\\" + name;
            
            return File.ReadAllText(path);
        }
    }
}
