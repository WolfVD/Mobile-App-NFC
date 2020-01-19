using Google.Protobuf;
using NFCProject.Pages;
using NFCProject.Services;
using System;

namespace NFCProject
{
    public class ReadNFC
    {
        public static byte[] EncryptNonceRead(byte[] input) // Encrypt the input (nonce) and create a request back to get node config
        {
            RX1_NFC_Reply nfcReply = RX1_NFC_Reply.Parser.ParseFrom(input);
            byte[] nonce = nfcReply.Nonce.ToByteArray();

            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c"); //Convert the hex string to a byte array
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f"); //Convert the hex string to a byte array

            CryptoHandler cryptoHandler = new CryptoHandler();

            byte[] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

            byte[] trimmedResult = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                trimmedResult[i] = encryptedNonce[i];
            }

            //Create a request to get the node config
            RX1_NFC_Request nfcRequest = new RX1_NFC_Request
            {
                RequestType = RX1_NFC_Request.Types.NFCRequestType.GetNodeConfig,
                EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                NullPayload = true
            };

            byte[] bytes = nfcRequest.ToByteArray(); //Convert request to a byte array so it can be written

            return bytes;
        }

        public static void GetValues(byte[] input) // Get values from node config and display them
        {
            RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(input);

            string NodeID = "Node ID (SN): " + nfcSecondReply.NodeConfig.NodeID.ToString();
            string NetworkID = "Network ID: " + nfcSecondReply.NodeConfig.NetworkID.ToString();
            string NetworkChannel = "Network Channel: " + nfcSecondReply.NodeConfig.NetworkChannel.ToString();
            string Softver = "Software Version: " + nfcSecondReply.NodeConfig.SoftwareVersion.ToString();
            string WireVer = "Wirepas Version: " + nfcSecondReply.NodeConfig.WirepasVersion.Major.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Devel.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Maint.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Minor.ToString();
            string NodeConfig = "Configuration ID: " + nfcSecondReply.NodeConfig.NodeConfiguration.ToString();
            string AppAreaID = "Application Area ID: " + nfcSecondReply.NodeConfig.ApplicationAreaID.ToString();
            string HeadNodeRSSI = "Head Node RSSI: " + nfcSecondReply.NodeConfig.HeadNodeRSSI.ToString();
            string BatVoltage = "Battery Voltage: " + (Convert.ToDouble(nfcSecondReply.NodeConfig.BatteryVoltage) / 1000).ToString() + " V";

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
