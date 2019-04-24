using System.IO;

namespace Mobile.ConfigurationManager
{
    public partial class ConfigurationManagerLight
    {
        public static void Init(string config = "Assets/App.config")
        {
            using (var stream = new StreamReader(config))
                Init(stream);
        }
    }
}
