using Xamarin.Forms;
using System;
using System.IO;
using NFCProject.Services;
using System.Collections.Generic;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {

        public string NetID = "N/A";
        public string NetChan = "N/A";
        public string NodeConfig = "N/A";
        public string OperMode = "N/A";
        public string EncKey = "N/A";
        public string AuthKey = "N/A";
        public string UpdateRate = "N/A";

        public WriteToNode()
        {
            InitializeComponent();
        }

        async void SaveValues(object sender, System.EventArgs e)
        {
            bool answer = await DisplayAlert("Save Values", "Are you sure you want to write the given values to the node?", "Yes", "No");

            if (answer == true) {

                if (NetIDBox.IsChecked) {
                    NetID = NetIDEntry.Text;
                }
                if (NetChanBox.IsChecked)
                {
                    NetChan = NetChanEntry.Text;
                }
                if (NodeConfigBox.IsChecked)
                {
                    NodeConfig = NodeConfigEntry.Text;
                }
                if (OperModeBox.IsChecked)
                {
                    OperMode = OperModeEntry.Text;
                }
                if (EncKeyBox.IsChecked)
                {
                    EncKey = EncKeyEntry.Text;
                }
                if (AuthKeyBox.IsChecked)
                {
                    AuthKey = AuthKeyEntry.Text;
                }
                if (UpdateRateBox.IsChecked)
                {
                    UpdateRate = UpdateRateEntry.Text;
                }
            }

        }
        async void iosScan(object sender, System.EventArgs e)
        {
            try
            {
                IWriteScan service = DependencyService.Get<IWriteScan>(DependencyFetchTarget.NewInstance);
                Console.WriteLine("test");
                service.StartWriteScan();
            }
            catch {
                Console.WriteLine("yeet");
            }
        }
    }
}