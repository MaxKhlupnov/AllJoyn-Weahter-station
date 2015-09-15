using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;


using SensorClient.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensorClient
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private WeatherShieldViewModel ShieldViewModel = new WeatherShieldViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = ShieldViewModel;
            ((SensorClient.App)Application.Current).OnBridgeInitialized += OnBridgeInitialized;
        }

        private void Status_TextChanged(object sender, TextChangedEventArgs e)
        {
        }


        private void OnBridgeInitialized(IAsyncAction asyncAction, AsyncStatus asyncStatus)
        {
            //TODO: Add status information
        }
    }
}
