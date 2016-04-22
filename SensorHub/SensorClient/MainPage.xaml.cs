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
using WinRTXamlToolkit.Debugging;
using WinRTXamlToolkit.Debugging.Views;
using WinRTXamlToolkit.Controls.Extensions;
using WinRTXamlToolkit.Controls;


using SensorClient.DataModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensorClient
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        public Popup DebugPopup { get; set; }


        public MainPage()
        {
            DC.ShowLog();
            this.InitializeComponent();
          
            this.DataContext = new WeatherShieldViewModel();

            ((SensorClient.App)Application.Current).OnBridgeInitialized += OnBridgeInitialized;
            this.Loaded += MainPage_Loaded;            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {            
            DC.Hide();

        }
        
        private void OnBridgeInitialized(IAsyncAction asyncAction, AsyncStatus asyncStatus)
        {
            Debug.WriteLine("AllJoyn bridge successfully activated");
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            /*var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((ControlInfoDataGroup)group).UniqueId);*/
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void SensorsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            /*var itemId = ((ControlInfoDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);*/
        }

        private void chkShowTracePanel_Unchecked(object sender, RoutedEventArgs e)
        {
            DC.Hide();
        }

        private void chkShowTracePanel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
               
                if (DebugPopup == null)
                {
                    DC.ShowLog();
                    DebugPopup = Window.Current.Content.GetFirstDescendantOfType<Popup>();

                    if (DebugPopup != null)
                    {
                        var debugPopupTitle = ((DebugConsoleView)DebugPopup.Child).Content.GetFirstDescendantOfType<ToolWindow>();
                        if (debugPopupTitle != null)
                        {
                            debugPopupTitle.Title = "Debug messages trace";
                        }

                        DebugPopup.HorizontalOffset = 500;
                        DebugPopup.Closed += DebugPopup_Closed;
                    }
                }
                else
                {
                    DC.Show();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't load debug view: " + ex.Message);
            }
        }

        private void DebugPopup_Closed(object sender, object e)
        {
            if (chkShowTracePanel != null)
                chkShowTracePanel.IsChecked = false;
        }

        CheckBox chkShowTracePanel = null;
        private void chkShowTracePanel_Loaded(object sender, RoutedEventArgs e)
        {
            chkShowTracePanel = sender as CheckBox;
        }
    }


}
