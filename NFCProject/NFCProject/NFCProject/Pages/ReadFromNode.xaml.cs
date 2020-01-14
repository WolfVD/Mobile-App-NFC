using System;
using System.IO;
using Xamarin.Forms;
using NFCProject.Services;
using System.ComponentModel;
using System.Windows.Input;

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

        async void iosScan(object sender, System.EventArgs e)
        {
            IReadScan service = DependencyService.Get<IReadScan>(DependencyFetchTarget.NewInstance);
            service.StartReadScan();

        }

        public void DisplayValues(string[] valueList)
        {
            ViewModel.NodeIDString = valueList[0];
            ViewModel.NetworkIDString = valueList[1];
            ViewModel.NetChanString = valueList[2];
            //ViewModel.HardVerString = valueList[3];
            ViewModel.SoftVerString = valueList[3];
            ViewModel.WireVerString = valueList[4];
            ViewModel.NodeConfigString = valueList[5];
            //ViewModel.PowerTestString = valueList[6];
            ViewModel.AppAreaIDString = valueList[6];
            ViewModel.HeadNodeRSSIString = valueList[7];
            ViewModel.BatVoltageString = valueList[8];
            //ViewModel.GatewaySNString = valueList[10];

        }


    }
}