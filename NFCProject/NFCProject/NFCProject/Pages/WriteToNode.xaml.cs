using Xamarin.Forms;
using System;
using System.IO;
using NFCProject.Services;
using System.Collections.Generic;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {

        string NetID = "N/A";
        string NetChan = "N/A";
        string NodeConfig = "N/A";
        string OperMode = "N/A";
        string EncKey = "N/A";
        string AuthKey = "N/A";
        string UpdateRate = "N/A";

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
                service.StartWriteScan(NetID, NetChan, NodeConfig, OperMode, EncKey, AuthKey, UpdateRate, NetIDBox.IsChecked, NetChanBox.IsChecked, NodeConfigBox.IsChecked, OperModeBox.IsChecked, EncKeyBox.IsChecked, AuthKeyBox.IsChecked, UpdateRateBox.IsChecked);
            }
            catch {
                Console.WriteLine("yeet");
            }
        }
    }
}