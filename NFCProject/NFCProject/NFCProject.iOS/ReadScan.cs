using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreNFC;
using UIKit;
using Foundation;
using CoreFoundation;
using Google.Protobuf;
using NFCProject.Services;
using NFCProject.Pages;
using Xamarin;
using System.Text;

namespace NFCProject.iOS
{
    public class ReadScan : NFCNdefReaderSessionDelegate, IReadScan
    {
        NFCNdefReaderSession Session;
        INFCNdefTag tag;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            //This is left empty on purpose because it will never be called
        }

        [Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            //Connect with the NDEF Tag
            tag = tags[0];

            Session = session;

            session.ConnectToTag(tag, delegate { });

            Action<NFCNdefMessage, NSError> readNonce;
            readNonce = EncryptNonce;

            tag.ReadNdef(readNonce); //Read NDEF tag and then encrypt the nonce

            
        }

        public void EncryptNonce(NFCNdefMessage message, NSError error) {
            NFCNdefPayload messageRecord = message.Records[0];
            Console.WriteLine(message);

            byte[] bytes = messageRecord.Payload.ToArray();

            RX1_NFC_Reply nfcReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
            byte[] nonce = nfcReply.Nonce.ToByteArray();

            //Generate the key and IV for encryption
            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c");
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f");

            CryptoHandler cryptoHandler = new CryptoHandler();
            byte [] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

            //Trim the encrypted nonce to a length of 16 bytes
            byte[] trimmedResult = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                trimmedResult[i] = encryptedNonce[i];
            }


            RX1_NFC_Request nfcRequest = new RX1_NFC_Request
            {
                RequestType = RX1_NFC_Request.Types.NFCRequestType.GetNodeConfig,
                EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                NullPayload = true
            };

            string nfcReplyPayload = nfcRequest.ToString(); //Convert to a request to a string so it can be written
            Console.WriteLine(nfcReplyPayload);

            //Create a payload/message and write it.
            NFCNdefPayload writePayload = NFCNdefPayload.CreateWellKnownTypePayload(nfcReplyPayload, NSLocale.CurrentLocale);
            NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
            tag.WriteNdef(writeMessage, delegate { Console.WriteLine("Write succesful"); });

            System.Threading.Thread.Sleep(1000); //Wait 1 second

            //Read tag again to get node config
            Action<NFCNdefMessage, NSError> readNodeConfig;
            readNodeConfig = GetNodeConfig;
            tag.ReadNdef(readNodeConfig);

        }

        public void GetNodeConfig(NFCNdefMessage message, NSError error) 
        {
            NSData messageRecord = message.Records[0].Payload;

            byte[] bytes = messageRecord.ToArray();

            RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
            RX1_Uplink_Config nodeConfig = nfcSecondReply.NodeConfig;
            Console.WriteLine(nodeConfig);

            string NodeID = "Node ID: " + nfcSecondReply.NodeConfig.NodeID.ToString();
            string NetworkID = "Network ID: " + nfcSecondReply.NodeConfig.NetworkID.ToString();
            string NetworkChannel = "Network Channel: " + nfcSecondReply.NodeConfig.NetworkChannel.ToString();
            string Softver = "Software Version: " + nfcSecondReply.NodeConfig.SoftwareVersion.ToString();
            string WireVer = "Wirepas Version: " + nfcSecondReply.NodeConfig.WirepasVersion.Major.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Devel.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Maint.ToString() + "." + nfcSecondReply.NodeConfig.WirepasVersion.Minor.ToString();
            string NodeConfig = "Node Configuration: " + nfcSecondReply.NodeConfig.NodeConfiguration.ToString();
            string AppAreaID = "Application Area ID: " + nfcSecondReply.NodeConfig.ApplicationAreaID.ToString();
            string HeadNodeRSSI = "Head Node RSSI: " + nfcSecondReply.NodeConfig.HeadNodeRSSI.ToString();
            string BatVoltage = "Battery Voltage: " + nfcSecondReply.NodeConfig.BatteryVoltage.ToString();

            string[] valueList = new string[] { NodeID, NetworkID, NetworkChannel, Softver, WireVer, NodeConfig, AppAreaID, HeadNodeRSSI, BatVoltage };


            ReadFromNode readValues = new ReadFromNode();
            readValues.DisplayValues(valueList);

            Session.InvalidateSession();
        }

        
        
        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            //Pass variables through to ReadFromNode.cs page
            Console.WriteLine("DidInvalidate");
        }

        public void StartReadScan()
        {
            Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
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
