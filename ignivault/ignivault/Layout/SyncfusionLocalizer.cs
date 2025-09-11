using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor;
namespace ignivault.Layout
{
    public class SyncfusionLocalizer : ISyncfusionStringLocalizer
    {
        // To get the locale key from mapped resources file
        public string? GetText(string key)
        {
            string value = this.ResourceManager.GetString(key);

            if(string.IsNullOrEmpty(value))
            {
                Console.WriteLine($"Missing localization for key: {key}");
                return key;
            }

            return value;
        }

        // To access the resource file and get the exact value for locale key

        public System.Resources.ResourceManager ResourceManager
        {
            get
            {
                return ignivault.Resources.SfResources.ResourceManager;
            }
        }
    }
}
