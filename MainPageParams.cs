using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GeetApp
{
    public class MainPageParams
    {
        public Frame CentreFrame { get; set; }
        public NavigationView NavigationControlView { get; set; }
        
        public Collection collection { get; set; }

        //public List<string> ListofPlaylistName { get; set; }
    }
}
