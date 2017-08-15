using CompactView.Data;
using CompactView.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CompactView.Models
{
    public class WebsiteViewModel
    {
        private long _iD;

            public long ID
        {
            get
            {
                return _iD;
            }
        }
            public string Name { get; set; }
            public Uri URL { get; set; }
            public Symbol Symbol { get; set; }
            public char SymbolAsChar { get { return (char)Symbol; }
        }

        public WebsiteViewModel()
        {

        }
        public static WebsiteViewModel FromWebsite(Website website)
        {
            var viewModel = new WebsiteViewModel();

            viewModel._iD = website.ID;
            viewModel.Name = website.Name;
            viewModel.URL = website.URL;
            viewModel.Symbol = website.Symbol;
            //viewModel.SymbolAsChar = website.SymbolAsChar;

            return viewModel;
        }
     }

}
