using System.IO;

namespace Mobile.ConfigurationManager
{
    public partial class ConfigurationManagerLight
    {
        public static void Init(string config = "App.config")
        {
            using (var stream = new StreamReader(Android.App.Application.Context.Assets.Open(config)))
                Init(stream);
        }
    }
}