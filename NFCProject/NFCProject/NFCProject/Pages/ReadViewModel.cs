using System.ComponentModel;
using Xamarin.Forms;
using System;

namespace NFCProject.Pages
{
    public class ReadViewModel : BaseBind
    {
        public ReadViewModel()
        {
            NodeIDString = "Node ID (SN): ";
            NetworkIDString = "Network ID: ";
            NetChanString = "Network Channel: ";
            HardVerString = "Hardware Version: ";
            SoftVerString = "Software Version: ";
            WireVerString = "Wirepas Version: ";
            NodeConfigString = "Configuration ID: ";
            PowerTestString = "Power-On-Self-Test Result: ";
            AppAreaIDString = "Application Area ID: ";
            HeadNodeRSSIString = "Head Node RSSI: ";
            BatVoltageString = "Battery Voltage: ";
            GatewaySNString = "Gateway SN: ";
        }

        private string nodeIDString;
        public string NodeIDString
        {
            get { return nodeIDString; }
            set
            {
                SetProperty(ref nodeIDString, value);
            }
        }

        private string networkIDString;
        public string NetworkIDString
        {
            get { return networkIDString; }
            set
            {
                SetProperty(ref networkIDString, value);
            }
        }

        private string hardVerString;
        public string HardVerString
        {
            get { return hardVerString; }
            set
            {
                SetProperty(ref hardVerString, value);
            }
        }

        private string netChanString;
        public string NetChanString
        {
            get { return netChanString; }
            set
            {
                SetProperty(ref netChanString, value);
            }
        }

        private string softVerString;
        public string SoftVerString
        {
            get { return softVerString; }
            set
            {
                SetProperty(ref softVerString, value);
            }
        }

        private string wireVerString;
        public string WireVerString
        {
            get { return wireVerString; }
            set
            {
                SetProperty(ref wireVerString, value);
            }
        }

        private string nodeConfigString;
        public string NodeConfigString
        {
            get { return nodeConfigString; }
            set
            {
                SetProperty(ref nodeConfigString, value);
            }
        }

        private string powerTestString;
        public string PowerTestString
        {
            get { return powerTestString; }
            set
            {
                SetProperty(ref powerTestString, value);
            }
        }

        private string appAreaIDString;
        public string AppAreaIDString
        {
            get { return appAreaIDString; }
            set
            {
                SetProperty(ref appAreaIDString, value);
            }
        }

        private string headNodeRSSIString;
        public string HeadNodeRSSIString
        {
            get { return headNodeRSSIString; }
            set
            {
                SetProperty(ref headNodeRSSIString, value);
            }
        }

        private string batVoltageString;
        public string BatVoltageString
        {
            get { return batVoltageString; }
            set
            {
                SetProperty(ref batVoltageString, value);
            }
        }

        private string gatewaySNString;
        public string GatewaySNString
        {
            get { return gatewaySNString; }
            set
            {
                SetProperty(ref gatewaySNString, value);
            }
        }


    }
}
