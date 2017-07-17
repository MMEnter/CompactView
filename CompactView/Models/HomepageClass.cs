using CompactView.Views;
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
        public string Name { get; set; }
        public string URL { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar { get { return (char)Symbol; } }
    }

    public class UseWebsite
    {
        List<Website> websites = new List<Website>
        {
            new Website() {Name="Netflix", URL="https://www.netflix.com/", Symbol= Symbol.Globe},
            new Website() {Name="HNA", URL="https://www.hna.de/", Symbol= Symbol.Globe}
        };

        public List<Website> GetList()
        {
            return websites;
        }
    }
}
