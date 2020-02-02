using Google.Protobuf;
using NFCProject.Pages;
using NFCProject.Services;
using System;

namespace NFCProject
{
    public class ReadNFC
    {
        public static void DisplayValues(byte[] input) // Get values from node config and display them
        {
            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c"); //Convert the hex string to a byte array
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f"); //Convert the hex string to a byte array

            CryptoHandler cryptoHandler = new CryptoHandler();
            byte[] data = cryptoHandler.Decrypt(input, Key, IV);

            int lastIndex = Array.FindLastIndex(data, b => b != 0);

            Array.Resize(ref data, lastIndex + 1);

            RLConfigPayload payload = RLConfigPayload.Parser.ParseFrom(data);

            string NodeID = "Node ID (SN): " + payload.NodeID.ToString();
            string NetworkID = "Network ID: " + payload.NetworkID.ToString();
            string NetworkChannel = "Network Channel: " + payload.NetworkChannel.ToString();
            string AppAreaID = "Application Area ID: " + payload.ApplicationAreaID.ToString("X");
            string HardVer = "Hardware Version: " + payload.HardwareVersion.ToString();
            string Softver = "Software Version: " + payload.SoftwareVersion.Major.ToString() + "." + payload.SoftwareVersion.Devel.ToString() + "." + payload.SoftwareVersion.Maint.ToString() + "." + payload.SoftwareVersion.Minor.ToString();
            string MeshVer = "Wirepas Version: " + payload.WirepasVersion.Major.ToString() + "." + payload.WirepasVersion.Devel.ToString() + "." + payload.WirepasVersion.Maint.ToString() + "." + payload.WirepasVersion.Minor.ToString();
            string NodeConfig = payload.NodeConfiguration.ToString();

            string OperatingMode = "Operating Mode: ";
            switch (payload.OperatingMode)
            {
                case NodeOperatingMode.Run:
                    OperatingMode = "Operating Mode: Run";
                    break;
                case NodeOperatingMode.Inventory:
                    OperatingMode = "Operating Mode: Inventory";
                    break;
            }            
            string HeadNodeRSSI = "Normalized Head Node RSSI: " + (Convert.ToDouble((payload.HeadNodeRSSI)/254.0f)*100.0f).ToString() + "%";
            string BatVoltage = "Battery Voltage: " + (Convert.ToDouble(payload.BatteryVoltage) / 1000.0f).ToString() + " V";
            string GateConnect = "Gateway Connected: " + (payload.GatewayConnected ? "Yes" : "No").ToString();
            string UplinkRate = "Uplink Rate: " + payload.UplinkRate.ToString() + " Seconds";

            string DeviceRole = "Device Role: ";
            switch (payload.NodeRole)
            {
                case NodeRole.HeadnodeAnchor:
                    DeviceRole = "Device Role: Router";
                    break;
                case NodeRole.SubnodeTracked:
                    DeviceRole = "Device Role: Leaf";
                    break;
            }

            string AssetTrack = "Asset Tracking: " + (payload.AssetTrackingEnabled ? "Enabled" : "Disabled").ToString();

            string[] valueList = new string[] { NodeID, NetworkID, NetworkChannel, AppAreaID, HardVer, Softver, MeshVer, NodeConfig, OperatingMode, HeadNodeRSSI, BatVoltage, GateConnect, UplinkRate, DeviceRole, AssetTrack};

            ReadFromNode readValues = new ReadFromNode();
            readValues.DisplayValues(valueList); //Display the values

        }

        public static byte[] hexToByte(String s)
        {
            int length = s.Length;
            byte[] data = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }

            return data;
        }
    }
}
