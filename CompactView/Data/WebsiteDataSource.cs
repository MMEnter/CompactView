using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CompactView.Data
{
    class WebsiteDataSource
    {
            static List<Website> _websites = new List<Website>
            {
            };

            static Website _tempWebsite = new Website();

            //Returns the list with all the saved Websites.
            public static List<Website> GetList()
            {
                return _websites;
            }

        public static Website GetTempSite()
        {
            return _tempWebsite;
        }

        public static void SetTempSite(string name, Uri uRL)
        {
            long iD = Convert.ToInt64(DateTime.Now.Ticks);
            Symbol symbol = Symbol.Globe;

            _tempWebsite = new Website() { ID = iD, Name = name, URL = uRL, Symbol = symbol };
        }

            //Returns either the first element in the list or a default page when the list is empty.
            public static Website GetDefault()
            {
                if (_websites.Count == 0)
                {
                    return new Website() { Name = "Bing", URL = new Uri("https://www.bing.com/"), Symbol = Symbol.Globe };
                }
                else
                {
                    return _websites.ElementAt(0);
                }

            }

            //Returns the first website with the matching iD.
            public static Website GetWebsite(long iD)
            {
                Website website = _websites.Find(
                delegate (Website site)
                {
                    return site.ID == iD;
                }
                );
                return website;
            }

            public static void AddNewAsync(string name, Uri uRL)
            {
                long iD = Convert.ToInt64(DateTime.Now.Ticks);
                Symbol symbol = Symbol.Globe;

                Website newSite = new Website() { ID = iD, Name = name, URL = uRL, Symbol = symbol };

                _websites.Add(newSite);
                SaveAsync(_websites);
            }

            public static void Delete(long iD)
            {
                _websites.Remove(GetWebsite(iD));
                SaveAsync(_websites);
            }

            public static void Rename(long iD, string newName)
            {
                GetWebsite(iD).Name = newName;
                SaveAsync(_websites);
            }

        public static void ChangeUrl(long iD, Uri newURL)
        {
            GetWebsite(iD).URL = newURL;
            SaveAsync(_websites);
        }

        public static void ChangeSymbol(long iD, Symbol newSymbol)
        {
            GetWebsite(iD).Symbol = newSymbol;
            SaveAsync(_websites);
        }

        //Saves the Website List in Roaming Storage to allow cross device sync.
        //TODO: At a prompt when file gets over 100kb.
        public static async Task SaveAsync(List<Website> keyLargeObject)
            {
                RoamingObjectStorageHelper helper = new RoamingObjectStorageHelper();
                await helper.SaveFileAsync("keyWebsites", _websites);
            }

            public static async Task LoadAsync()
            {
                RoamingObjectStorageHelper helper = new RoamingObjectStorageHelper();
                _websites = await helper.ReadFileAsync("keyWebsites", _websites);
            }
        }
    }
