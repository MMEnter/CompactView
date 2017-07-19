using CompactView.Views;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;

namespace CompactView.Models
{
    public class Website
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public Uri URL { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar { get { return (char)Symbol; } }
    }

    public class UseWebsite
    {
        static List<Website> websites = new List<Website>
        {
            new Website() {Name="Netflix", URL= new Uri("https://www.netflix.com/"), Symbol= Symbol.Globe},
            new Website() {Name="HNA", URL=new Uri("https://www.hna.de/"), Symbol= Symbol.Globe}
        };

        public static List<Website> GetList()
        {
            return UseWebsite.websites;
        }

        public static void AddNewAsync(string name, Uri uRL)
        {
            long iD = Convert.ToInt64(DateTime.Now.Ticks);
            Symbol symbol = Symbol.Globe;

            Website newSite = new Website() { ID = iD, Name = name, URL = uRL, Symbol = symbol };

            websites.Add(newSite);
            SaveAsync(websites);
        }

        public void Delete(int iD)
        {
            Website deleteSite = websites.Find(
                delegate (Website site)
                {
                    return site.ID == iD;
                }
                );
            websites.Remove(deleteSite);
        }

        public static async Task SaveAsync(List<Website> keyLargeObject)
        {
            RoamingObjectStorageHelper helper = new RoamingObjectStorageHelper();
            await helper.SaveFileAsync("keyWebsites", websites);
        }
        public static async Task LoadAsync()
        {
            RoamingObjectStorageHelper helper = new RoamingObjectStorageHelper();
            websites = await helper.ReadFileAsync("keyWebsites", websites);
        }
    }
}
