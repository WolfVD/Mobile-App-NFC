using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreNFC;
using UIKit;
using Foundation;
using CoreFoundation;
using Google.Protobuf;
using NFCProject.Services;
using Xamarin;
using System.Text;

namespace NFCProject.iOS
{
    public class ReadScan : NFCNdefReaderSessionDelegate, IReadScan
    {
        uint nodeID;
        uint netChan;
        uint netID;
        uint hardVersion;
        uint softVersion;
        string wirepasVersion;
        string operMode;
        string appAreaID;
        string nodeConfigValue;
        uint headNodeRSSI;
        uint batteryVolt;
        bool gatewayConnectBool;
        string gatewayConnect;

        byte[] trimmedResult;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            //This is left empty on purpose because it will never be called
        }

        public void EncryptNonce(NFCNdefMessage message, NSError error) {
            
            NSData readPayload = message.Records[0].Payload;
            byte[] bytes = readPayload.ToArray();

            RX1_NFC_Reply nfcReply;
            nfcReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
            byte[] nonce = nfcReply.Nonce.ToByteArray();
            Console.WriteLine("Nonce: " + nonce);

            //Generate the key and IV for encryption
            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c");
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f");

            CryptoHandler cryptoHandler = new CryptoHandler();

            byte [] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

            //Trim the encrypted nonce to a length of 16 bytes
            trimmedResult = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                trimmedResult[i] = encryptedNonce[i];
            }
        }

        public void GetNodeConfig(NFCNdefMessage message, NSError error) 
        {
            NSData readPayload = message.Records[0].Payload;
            byte[] bytes = readPayload.ToArray();

            RX1_NFC_Reply nfcSecondReply;
            nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
            RX1_Uplink_Config nodeConfig = nfcSecondReply.NodeConfig;
            Console.WriteLine(nodeConfig);

            nodeID = nodeConfig.NodeID;
            netChan = nodeConfig.NetworkChannel;
            netID = nodeConfig.NetworkID;
            hardVersion = nodeConfig.HardwareVersion;
            softVersion = nodeConfig.SoftwareVersion;
            wirepasVersion = nodeConfig.WirepasVersion.Major.ToString() + "." + nodeConfig.WirepasVersion.Devel.ToString() + "." + nodeConfig.WirepasVersion.Maint.ToString() + "." + nodeConfig.WirepasVersion.Minor.ToString();
            nodeConfigValue = nodeConfig.NodeConfiguration.ToString();
            operMode = nodeConfig.OperatingMode.ToString();
            appAreaID = Convert.ToString(nodeConfig.ApplicationAreaID, 16);
            headNodeRSSI = nodeConfig.HeadNodeRSSI;
            batteryVolt = nodeConfig.BatteryVoltage;
            bool gatewayConnectBool = nodeConfig.GatewayConnected;
            if (gatewayConnectBool == true)
            {
                gatewayConnect = "Yes";
            }
            else
            {
                gatewayConnect = "No";
            }

            Console.WriteLine("nodeID: " + nodeID);
            Console.WriteLine("netChan: " + netChan);
            Console.WriteLine("netID: " + netID);
            Console.WriteLine("hardVersion: " + hardVersion);
            Console.WriteLine("softVersion: " + softVersion);
            Console.WriteLine("wirepasVersion: " + wirepasVersion);
            Console.WriteLine("nodeConfigValue: " + nodeConfigValue);
            Console.WriteLine("operMode: " + operMode);
            Console.WriteLine("appAreaID: " + appAreaID);
            Console.WriteLine("headNodeRSSI: " + headNodeRSSI);
            Console.WriteLine("batteryVolt: " + batteryVolt);
            Console.WriteLine("gatewayConnect: " + gatewayConnect);
        }

        [Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            //Connect with the NDEF Tag
            INFCNdefTag tag = tags[0];
            session.ConnectToTag(tag, delegate { });

            Action<NFCNdefMessage, NSError> readNonce;
            readNonce = EncryptNonce;

            tag.ReadNdef(readNonce); //Read NDEF tag and then encrypt the nonce

            //Create a request to write back to the RX1 Node
            RX1_NFC_Request nfcRequest = new RX1_NFC_Request
            {
                RequestType = RX1_NFC_Request.Types.NFCRequestType.GetNodeConfig,
                EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                NullPayload = true
            };

            string nfcReplyPayload = nfcRequest.ToString(); //Convert to a request to a string so it can be written

            //Create a payload/message and write it.
            NFCNdefPayload writePayload = NFCNdefPayload.CreateWellKnownTypePayload(nfcReplyPayload, NSLocale.CurrentLocale);
            NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
            tag.WriteNdef(writeMessage, delegate { Console.WriteLine("Write succesful"); } );

            System.Threading.Thread.Sleep(1000); //Wait 1 second

            //Read tag again to get node config
            Action<NFCNdefMessage, NSError> readNodeConfig;
            readNodeConfig = GetNodeConfig;
            tag.ReadNdef(readNodeConfig);

            session.InvalidateSession(); //Invalidate session (automatically goes to DidInvalidate)

        }
        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            //Pass variables through to ReadFromNode.cs page
        }

        public async Task StartReadScan()
        {
            NFCNdefReaderSession Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
            Session.BeginSession();
        }

        public static byte[] hexToByte(String s) {
            int length = s.Length;
            byte[] data = new byte[length / 2];
            for (int i = 0; i < length; i += 2) { 
                data[i/2] = Convert.ToByte(s.Substring(i, 2), 16);
            }

            return data;
        }
    }
}
