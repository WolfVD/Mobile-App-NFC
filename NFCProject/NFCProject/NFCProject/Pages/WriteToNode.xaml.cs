using Xamarin.Forms;
using System;
using System.IO;
using NFCProject.Services;
using System.Collections.Generic;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {

        static string NetID = "1";
        static string NetChan = "1";
        static string NodeConfig = "1";
        static string OperMode = "1";
        static string EncKey = "6650208e3aac4f4043a9ae5a9a8761c1";
        static string AuthKey = "22de24ea7356c76e79126bff3491333f";
        static string UpdateRate = "60";

        static bool NetIDOn;
        static bool NetChanOn;
        static bool NodeConfigOn;
        static bool OperModeOn;
        static bool EncKeyOn;
        static bool AuthKeyOn;
        static bool UpdateRateOn;

        public static bool onSaved = false;

        public WriteToNode()
        {
            InitializeComponent();
        }

        async void SaveValues(object sender, System.EventArgs e)
        {
            onSaved = await DisplayAlert("Save Values", "Are you sure you want to write the given values to the node?", "Yes", "No");

            if (onSaved == true) {

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


        }

        public string[] ReturnValues()
        {
            string[] valueList = new string[] { NetID, NetChan, NodeConfig, OperMode, EncKey, AuthKey, UpdateRate };

            return valueList;
        }

        public bool[] ReturnChecked()
        {
            bool[] checkedList = new bool[] { NetIDOn, NetChanOn, NodeConfigOn, OperModeOn, EncKeyOn, AuthKeyOn, UpdateRateOn };

            return checkedList;
        }

        async void iosScan(object sender, System.EventArgs e)
        {
            IWriteScan service = DependencyService.Get<IWriteScan>(DependencyFetchTarget.NewInstance);
            service.StartWriteScan();
        }
    }
}