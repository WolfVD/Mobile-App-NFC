using Xamarin.Forms;
using System;
using System.IO;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {
        private string classId;

        private string netID = "N/A";
        private string netIDFinal = null;
        private bool netIDOn = false;

        private string netChan = "N/A";
        private string netChanFinal = null;
        private bool netChanOn = false;

        private string NodeConfig = "N/A";
        private string NodeConfigFinal = null;
        private bool NodeConfigOn = false;

        private string OperMode = "N/A";
        private string OperModeFinal = null;
        private bool OperModeOn = false;

        private string EncKey = "N/A";
        private string EncKeyFinal = null;
        private bool EncKeyOn = false;

        private string AuthKey = "N/A";
        private string AuthKeyFinal = null;
        private bool AuthKeyOn = false;

        private string UpdateRate = "N/A";
        private string UpdateRateFinal = null;
        private bool UpdateRateOn = false;
        public WriteToNode()
        {
            InitializeComponent();
        }

        void Entry_CheckBox(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            string classId = checkbox.ClassId;


            if (classId == "NetID")
            {
                if (netIDOn == false)
                {
                    netIDOn = true;
                }
                else
                {
                    netIDOn = false;
                }
            }
            else if (classId == "NetChan")
            {
                if (netChanOn == false)
                {
                    netChanOn = true;
                }
                else
                {
                    netChanOn = false;
                }
            }
            else if (classId == "NodeConfig")
            {
                if (NodeConfigOn == false)
                {
                    NodeConfigOn = true;
                }
                else
                {
                    NodeConfigOn = false;
                }
            }
            else if (classId == "OperMode")
            {
                if (OperModeOn == false)
                {
                    OperModeOn = true;
                }
                else
                {
                    OperModeOn = false;
                }
            }
            else if (classId == "EncKey")
            {
                if (EncKeyOn == false)
                {
                    EncKeyOn = true;
                }
                else
                {
                    EncKeyOn = false;
                }
            }
            else if (classId == "UpdateRate")
            {
                if (UpdateRateOn == false)
                {
                    UpdateRateOn = true;
                }
                else
                {
                    UpdateRateOn = false;
                }
            }
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            classId = entry.ClassId;

            if (classId == "NetID")
            {
                netID = entry.Text;
            }
            else if (classId == "NetChan")
            {
                netChan = entry.Text;
            }
            else if (classId == "NodeConfig")
            {
                NodeConfig = entry.Text;
            }
            else if (classId == "OperMode")
            {
                OperMode = entry.Text;
            }
            else if (classId == "EncKey")
            {
                EncKey = entry.Text;
            }
            else if (classId == "UpdateRate")
            {
                UpdateRate = entry.Text;
            }
        }

        void SaveValues(object sender, System.EventArgs e)
        {
            if (netIDOn == true) {
                netIDFinal = netID; 
            } if (netChanOn == true) {
                netChanFinal = netChan;
            } if (NodeConfigOn == true) {
                NodeConfigFinal = NodeConfig;
            } if (OperModeOn == true) {
                OperModeFinal = OperMode;
            } if (EncKeyOn == true) {
                EncKeyFinal = EncKey;
            } if (UpdateRateOn == true)
            {
                UpdateRateFinal = UpdateRate;
            }
        }
        async void iosScan(object sender, System.EventArgs e)
        {
            IWriteScan service = DependencyService.Get<IWriteScan>(DependencyFetchTarget.NewInstance);
            await service.StartWriteScan(netIDFinal, netChanFinal, NodeConfigFinal, OperModeFinal, EncKeyFinal, UpdateRateFinal);
        }
    }
}