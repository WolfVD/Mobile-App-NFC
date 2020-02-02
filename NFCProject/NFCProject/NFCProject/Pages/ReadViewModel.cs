using System.ComponentModel;
using Xamarin.Forms;
using System;

namespace NFCProject.Pages
{
    public class ReadViewModel : BaseBind
    {
        public ReadViewModel()
        {
            NodeIDString = "Serial Number: ";
            NetworkIDString = "Network ID: ";
            NetChanString = "Network Channel: ";
            AppAreaIDString = "App Area ID: ";
            HardVerString = "Hardware Version: ";
            SoftVerString = "Software Version: ";
            MeshVerString = "Mesh Version: ";
            NodeConfigString = "Sensors";
            NodeConfigColor = Color.Gray;
            OperModeString = "Operating Mode: ";
            HeadNodeRSSIString = "Head Node RSSI: ";
            BatVoltageString = "Battery Voltage: ";
            GateConnectString = "Gateway Connected: ";
            UplinkRateString = "Uplink Rate: ";
            DeviceRoleString = "Device Role: ";
            AssetTrackString = "Asset Tracking: ";
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

        private string appAreaIDString;
        public string AppAreaIDString
        {
            get { return appAreaIDString; }
            set
            {
                SetProperty(ref appAreaIDString, value);
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

        private string softVerString;
        public string SoftVerString
        {
            get { return softVerString; }
            set
            {
                SetProperty(ref softVerString, value);
            }
        }

        private string meshVerString;
        public string MeshVerString
        {
            get { return meshVerString; }
            set
            {
                SetProperty(ref meshVerString, value);
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

        private Color nodeConfigColor;
        public Color NodeConfigColor
        {
            get { return nodeConfigColor; }
            set
            {
                SetProperty(ref nodeConfigColor, value);
            }
        }

        private string operModeString;
        public string OperModeString
        {
            get { return operModeString; }
            set
            {
                SetProperty(ref operModeString, value);
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

        private string gateConnectString;
        public string GateConnectString
        {
            get { return gateConnectString; }
            set
            {
                SetProperty(ref gateConnectString, value);
            }
        }

        private string uplinkRateString;
        public string UplinkRateString
        {
            get { return uplinkRateString; }
            set
            {
                SetProperty(ref uplinkRateString, value);
            }
        }

        private string deviceRoleString;
        public string DeviceRoleString
        {
            get { return deviceRoleString; }
            set
            {
                SetProperty(ref deviceRoleString, value);
            }
        }

        private string assetTrackString;
        public string AssetTrackString
        {
            get { return assetTrackString; }
            set
            {
                SetProperty(ref assetTrackString, value);
            }
        }

    }
}
