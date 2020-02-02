using Xamarin.Forms;
using NFCProject.Services;
using System;

namespace NFCProject.Pages
{
    public partial class ReadFromNode : ContentPage
    {

        static bool checkBox1Checked = false;
        static bool checkBox2Checked = false;
        static bool checkBox3Checked = false;
        static bool checkBox4Checked = false;
        static bool checkBox5Checked = false;

        public static ReadViewModel ViewModel => App.ViewModel;

        public ReadFromNode ()
        {
            InitializeComponent ();
            BindingContext = ViewModel;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                DisplaySensorView();
            };
            NodeConfig.GestureRecognizers.Add(tapGestureRecognizer);
        }

        async void DisplaySensorView()
        {
            Grid grid = SensorView.GetView(checkBox1Checked, checkBox2Checked, checkBox3Checked, checkBox4Checked, checkBox5Checked);
            StackLayout stackLayout = new StackLayout();

            Label title = new Label { Text = "Current Node Configuration", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Start, FontSize = 20, VerticalTextAlignment = TextAlignment.Center, TranslationX = 10, TranslationY = 10 };
            Label okLabel = new Label { Text = "OK", FontSize = 20, HorizontalTextAlignment = TextAlignment.Start, TranslationX = 10, TextColor = Color.HotPink, VerticalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Start };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Navigation.PopModalAsync();
            };
            okLabel.GestureRecognizers.Add(tapGestureRecognizer);

            grid.Children.Add(okLabel, 0, 5);

            stackLayout.Children.Add(title);
            stackLayout.Children.Add(grid);

            ContentPage contentPage = new ContentPage
            {
                Content = stackLayout,
            };

            await Navigation.PushModalAsync(contentPage);
        }

        // A scan button on IOS is required because: 1. Only certain phones support background tag reading (Iphone XS and above with exceptions); 2. Background tag reading does not support the payload type that is used by the node
        async void StartScan(object sender, System.EventArgs e) 
        {
            IStartNFC service = DependencyService.Get<IStartNFC>(DependencyFetchTarget.NewInstance);
            service.StartScan();

        }

        public void DisplayValues(string[] valueList)
        {
            ViewModel.NodeIDString = valueList[0];
            ViewModel.NetworkIDString = valueList[1];
            ViewModel.NetChanString = valueList[2];
            ViewModel.AppAreaIDString = valueList[3];
            ViewModel.HardVerString = valueList[4];
            ViewModel.SoftVerString = valueList[5];
            ViewModel.MeshVerString = valueList[6];
            SetCurrentConfig(Convert.ToInt32(valueList[7]));
            ViewModel.NodeConfigString = "Sensors - View";
            ViewModel.NodeConfigColor = Color.Blue;
            ViewModel.OperModeString = valueList[8];
            ViewModel.HeadNodeRSSIString = valueList[9];
            ViewModel.BatVoltageString = valueList[10];
            ViewModel.GateConnectString = valueList[11];
            ViewModel.UplinkRateString = valueList[12];
            ViewModel.DeviceRoleString = valueList[13];
            ViewModel.AssetTrackString = valueList[14];

        }

        private static void SetCurrentConfig(int config)
        {
            checkBox1Checked = (config & 0x0001) != 0;
            checkBox2Checked = (config & 0x0004) != 0;
            checkBox3Checked = (config & 0x0008) != 0;
            checkBox4Checked = (config & 0x0020) != 0;
            checkBox5Checked = (config & 0x0040) != 0;
        }


    }
}