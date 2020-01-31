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
            Console.WriteLine(payload);

            string NodeID = "Node ID (SN): " + payload.NodeID.ToString();
            string NetworkID = "Network ID: " + payload.NetworkID.ToString();
            string NetworkChannel = "Network Channel: " + payload.NetworkChannel.ToString();
            string Softver = "Software Version: " + payload.SoftwareVersion.Major.ToString() + "." + payload.SoftwareVersion.Devel.ToString() + "." + payload.SoftwareVersion.Maint.ToString() + "." + payload.SoftwareVersion.Minor.ToString();
            string WireVer = "Wirepas Version: " + payload.WirepasVersion.Major.ToString() + "." + payload.WirepasVersion.Devel.ToString() + "." + payload.WirepasVersion.Maint.ToString() + "." + payload.WirepasVersion.Minor.ToString();
            string NodeConfig = "Configuration ID: " + payload.NodeConfiguration.ToString();
            string AppAreaID = "Application Area ID: " + payload.ApplicationAreaID.ToString();
            string HeadNodeRSSI = "Head Node RSSI: " + payload.HeadNodeRSSI.ToString();
            string BatVoltage = "Battery Voltage: " + (Convert.ToDouble(payload.BatteryVoltage) / 1000).ToString() + " V";

            string[] valueList = new string[] { NodeID, NetworkID, NetworkChannel, Softver, WireVer, NodeConfig, AppAreaID, HeadNodeRSSI, BatVoltage };

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
