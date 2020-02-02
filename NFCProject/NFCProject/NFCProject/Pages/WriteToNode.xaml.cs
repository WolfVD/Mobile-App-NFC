using Xamarin.Forms;
using System;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {

        static string NetID = "1";
        static string NetChan = "1";
        static string OperMode = "0";
        static string EncKey = "6650208e3aac4f4043a9ae5a9a8761c1";
        static string AuthKey = "22de24ea7356c76e79126bff3491333f";
        static string UpdateRate = "12";
        static string DeviceRole = "0";
        static string AssetTrack = "0";
        static string FeatLock = "0";

        static string NetIDFinal = "1";
        static string NetChanFinal = "1";
        static string OperModeFinal = "0";
        static string EncKeyFinal = "6650208e3aac4f4043a9ae5a9a8761c1";
        static string AuthKeyFinal = "22de24ea7356c76e79126bff3491333f";
        static string UpdateRateFinal = "12";
        static string DeviceRoleFinal = "0";
        static string AssetTrackFinal = "0";
        static string FeatLockFinal = "0";

        static bool NetIDOn;
        static bool NetChanOn;
        static bool NodeConfigOn;
        static bool OperModeOn;
        static bool EncKeyOn;
        static bool AuthKeyOn;
        static bool UpdateRateOn;
        static bool DeviceRoleOn;
        static bool AssetTrackOn;
        static bool FeatLockOn;

        public static bool checkBox1Checked = false;
        public static bool checkBox2Checked = false;
        public static bool checkBox3Checked = false;
        public static bool checkBox4Checked = false;
        public static bool checkBox5Checked = false;

        public WriteToNode()
        {
            InitializeComponent();

            OperModePicker.Items.Add("Run");
            OperModePicker.Items.Add("Inventory");

            DeviceRolePicker.Items.Add("Router");
            DeviceRolePicker.Items.Add("Leaf");

            AssetTrackPicker.Items.Add("Enabled");
            AssetTrackPicker.Items.Add("Disabled");

            FeatLockPicker.Items.Add("Enabled");
            FeatLockPicker.Items.Add("Disabled");

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                DisplaySensorView();
            };
            NodeConfigClick.GestureRecognizers.Add(tapGestureRecognizer);
        }

        async void DisplaySensorView()
        {
            Grid grid = SensorView.GetView(checkBox1Checked, checkBox2Checked, checkBox3Checked, checkBox4Checked, checkBox5Checked);
            StackLayout stackLayout = new StackLayout();

            Label title = new Label { Text = "Current Node Configuration", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Start, FontSize=20, VerticalTextAlignment=TextAlignment.Center, TranslationX=10, TranslationY=10 };
            Label okLabel = new Label { Text = "OK", FontSize = 20, HorizontalTextAlignment = TextAlignment.Start, TranslationX = 10, TextColor=Color.HotPink, VerticalTextAlignment=TextAlignment.Start, VerticalOptions=LayoutOptions.Start };
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

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;
            
            if (entry == NetIDEntry)
            {
                NetID = entry.Text;
            }
            else if (entry == NetChanEntry)
            {
                NetChan = entry.Text;
            }
            else if (entry == EncKeyEntry)
            {
                EncKey = entry.Text;
            }
            else if (entry == AuthKeyEntry)
            {
                AuthKey = entry.Text;
            }
            else if (entry == UpdateRateEntry)
            {
                UpdateRate = entry.Text;
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            if (picker == OperModePicker)
            {
                OperMode = picker.SelectedIndex.ToString();
            }
            else if (picker == DeviceRolePicker)
            {
                DeviceRole = picker.SelectedIndex.ToString();
            }
            else if (picker == AssetTrackPicker)
            {
                AssetTrack = picker.SelectedIndex.ToString();
            }
            else if (picker == FeatLockPicker)
            {
                FeatLock = picker.SelectedIndex.ToString();
            }
        }

        private void CheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            bool value = Convert.ToBoolean(e.Value.ToString().ToLower());

            if (sender == NetIDBox)
            {
                NetIDOn = value;
            } 
            else if (sender == NetChanBox)
            {
                NetChanOn = value;
            }
            else if (sender == NodeConfigBox)
            {
                NodeConfigOn = value;
            }
            else if (sender == OperModeBox)
            {
                OperModeOn = value;
            }
            else if (sender == EncKeyBox)
            {
                EncKeyOn = value;
            }
            else if (sender == AuthKeyBox)
            {
                AuthKeyOn = value;
            }
            else if (sender == UpdateRateBox)
            {
                UpdateRateOn = value;
            }
            else if (sender == DeviceRoleBox)
            {
                DeviceRoleOn = value;
            }
            else if (sender == AssetTrackBox)
            {
                AssetTrackOn = value;
            }
            else if (sender == FeatLockBox)
            {
                FeatLockOn = value;
            }
        }

        public string[] ReturnValues()
        {

            if (NetIDOn)
            {
                NetIDFinal = NetID;
            }
            if (NetChanOn)
            {
                NetChanFinal = NetChan;
            }
            if (OperModeOn)
            {
                OperModeFinal = OperMode;
            }
            if (EncKeyOn)
            {
                EncKeyFinal = EncKey;
            }
            if (AuthKeyOn)
            {
                AuthKeyFinal = AuthKey;
            }
            if (UpdateRateOn)
            {
                UpdateRateFinal = UpdateRate;
            }
            if (DeviceRoleOn)
            {
                DeviceRoleFinal = DeviceRole;
            }
            if (AssetTrackOn)
            {
                AssetTrackFinal = AssetTrack;
            }
            if (FeatLockOn)
            {
                FeatLockFinal = FeatLock;
            }

            string[] valueList = new string[] { NetIDFinal, NetChanFinal, null, OperModeFinal, EncKeyFinal, AuthKeyFinal, UpdateRateFinal, DeviceRoleFinal, AssetTrackFinal, FeatLockFinal };

            return valueList;
        }

        public bool[] ReturnChecked()
        {
            bool[] checkedList = new bool[] { NetIDOn, NetChanOn, NodeConfigOn, OperModeOn, EncKeyOn, AuthKeyOn, UpdateRateOn, DeviceRoleOn, AssetTrackOn, FeatLockOn };

            return checkedList;
        }

        // A scan button on IOS is required because: 1. Only certain phones support background tag reading (Iphone XS and above with exceptions); 2. Background tag reading does not support the payload type that is used by the node
        async void StartScan(object sender, System.EventArgs e) 
        {
            IStartNFC service = DependencyService.Get<IStartNFC>(DependencyFetchTarget.NewInstance);
            service.StartScan();
        }
    }
}