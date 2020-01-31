using Xamarin.Forms;
using System;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {

        static string NetID = "1";
        static string NetChan = "1";
        static string NodeConfig = "0";
        static string OperMode = "0";
        static string EncKey = "6650208e3aac4f4043a9ae5a9a8761c1";
        static string AuthKey = "22de24ea7356c76e79126bff3491333f";

        static string NetIDFinal = "1";
        static string NetChanFinal = "1";
        static string NodeConfigFinal = "0";
        static string OperModeFinal = "0";
        static string EncKeyFinal = "6650208e3aac4f4043a9ae5a9a8761c1";
        static string AuthKeyFinal = "22de24ea7356c76e79126bff3491333f";

        static bool NetIDOn;
        static bool NetChanOn;
        static bool NodeConfigOn;
        static bool OperModeOn;
        static bool EncKeyOn;
        static bool AuthKeyOn;

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

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                DisplaySensorView();
            };
            NodeConfigClick.GestureRecognizers.Add(tapGestureRecognizer);
        }

        async void DisplaySensorView()
        {
            Grid grid = SensorView.GetView();
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

        public static void ChangeConfig(int row, bool isChecked)
        {
            Console.WriteLine("Test");
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

        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            if (picker == OperModePicker)
            {
                OperMode = picker.SelectedIndex.ToString();
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
            if (NodeConfigOn)
            {
                NodeConfigFinal = NodeConfig;
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

            string[] valueList = new string[] { NetIDFinal, NetChanFinal, NodeConfigFinal, OperModeFinal, EncKeyFinal, AuthKeyFinal };

            return valueList;
        }

        public bool[] ReturnChecked()
        {
            bool[] checkedList = new bool[] { NetIDOn, NetChanOn, NodeConfigOn, OperModeOn, EncKeyOn, AuthKeyOn };

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