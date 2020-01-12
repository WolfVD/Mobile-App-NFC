using System.ComponentModel;
using Xamarin.Forms;
using System;

namespace NFCProject.Pages
{
    public class ReadViewModel : BaseBind
    {
        public ReadViewModel() {
            NodeIDString = "Node ID: ";
            NetworkIDString = "Network ID: ";
            NetChanString = "Network Channel: ";
            SoftVerString = "Software Version: ";
            WireVerString = "Wirepas Version: ";
            NodeConfigString = "Node Configuration: ";
            AppAreaIDString = "Application Area ID: ";
            HeadNodeRSSIString = "Head Node RSSI: ";
            BatVoltageString = "Battery Voltage: ";
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


    }
}
