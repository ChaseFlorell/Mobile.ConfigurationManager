using System.IO;
using Android.Content;

namespace Mobile.ConfigurationManager
{
    public partial class ConfigurationManager
    {
        public static void Init(string config = "App.config")
        {
            using (var stream = new StreamReader(Android.App.Application.Context.Assets.Open(config)))
                InitInternal(stream);
        }

        public static void Init(Context context, string config = "App.config")
        {
            using (var stream = new StreamReader(context.Assets.Open(config)))
                InitInternal(stream);
        }
    }
}