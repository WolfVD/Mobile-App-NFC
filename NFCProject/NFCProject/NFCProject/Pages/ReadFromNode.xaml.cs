using Xamarin.Forms;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class ReadFromNode : ContentPage
    {

        public static ReadViewModel ViewModel => App.ViewModel;

        public ReadFromNode ()
        {
            InitializeComponent ();
            BindingContext = ViewModel;
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
            ViewModel.SoftVerString = valueList[3];
            ViewModel.WireVerString = valueList[4];
            ViewModel.NodeConfigString = valueList[5];
            ViewModel.AppAreaIDString = valueList[6];
            ViewModel.HeadNodeRSSIString = valueList[7];
            ViewModel.BatVoltageString = valueList[8];

        }


    }
}