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
        private NFCNdefReaderSession Session;
        private TaskCompletionSource<string> _tcs;
        string nfcReplyPayload;
        INFCNdefTag tag;
        bool writeNonce = false;
        NFCNdefPayload readMessage;
        NSData readPayload;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            Console.WriteLine("DidDetect entered");
            readMessage = messages[0].Records[0];
            readPayload = readMessage.Payload;
            byte[] bytes = new byte[readPayload.Length];
            System.Runtime.InteropServices.Marshal.Copy(readPayload.Bytes, bytes, 0, Convert.ToInt32(readPayload.Length));


            if (writeNonce == false)
            {
                //Create a byte array called bytes that contains the payload of the NFC Message

                //Get the nonce using the protocol buffer and the bytes
                RX1_NFC_Reply nfcReply;
                nfcReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
                string nonce = nfcReply.Nonce.ToStringUtf8();


                //Generate the key and IV for encryption
                byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c");
                byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f");

                byte[] encryptedNonce = CryptoHandler.EncryptString(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

                //Trim the encrypted nonce to a length of 16 bytes
                byte[] trimmedResult = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    trimmedResult[i] = encryptedNonce[i];
                }

                //Create a request back to write back to the RX1 Node
                RX1_NFC_Request nfcRequest = new RX1_NFC_Request
                {
                    RequestType = RX1_NFC_Request.Types.NFCRequestType.GetNodeConfig,
                    EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                    NullPayload = true
                };

                nfcReplyPayload = nfcRequest.ToString(); //Convert to a request to a string so it can be written

                Console.WriteLine("Starting to write message");

                NFCNdefPayload writePayload = NFCNdefPayload.CreateWellKnownTypePayload(nfcReplyPayload, NSLocale.CurrentLocale);
                NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });

                tag.WriteNdef(writeMessage, delegate { Console.WriteLine("Write succesful"); writeNonce = true; });
            }
            else {
                RX1_NFC_Reply nfcSecondReply;
                nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);
                RX1_Uplink_Config nodeConfig = nfcSecondReply.NodeConfig;
                Console.WriteLine(nodeConfig);
                uint nodeID = nodeConfig.NodeID;
                uint netChan = nodeConfig.NetworkChannel;
                uint netID = nodeConfig.NetworkID;
                uint hardVersion = nodeConfig.HardwareVersion;
                uint softVersion = nodeConfig.SoftwareVersion;
                string wirepasVersion = nodeConfig.WirepasVersion.Major.ToString() + "." + nodeConfig.WirepasVersion.Devel.ToString() + "." + nodeConfig.WirepasVersion.Maint.ToString() + "." + nodeConfig.WirepasVersion.Minor.ToString();
                string nodeConfigValue = nodeConfig.NodeConfiguration.ToString();
                string operMode = nodeConfig.OperatingMode.ToString();
                string appAreaID = Convert.ToString(nodeConfig.ApplicationAreaID, 16);
                uint headNodeRSSI = nodeConfig.HeadNodeRSSI;
                uint batteryVolt = nodeConfig.BatteryVoltage;
                bool gatewayConnectBool = nodeConfig.GatewayConnected;
                string gatewayConnect;
                if (gatewayConnectBool == true)
                {
                    gatewayConnect = "Yes";
                }
                else {
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
        }

        [Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {

            Console.WriteLine("----------------");
            Console.WriteLine("DidDetectTags entered");

            tag = tags[0];
            session.ConnectToTag(tag, delegate { });
            bool isAvail = tag.Available;
            tag.ReadNdef(delegate { Console.WriteLine("Reading tag"); });
            NFCNdefPayload payload = NFCNdefPayload.CreateWellKnownTypePayload("iOSpari", NSLocale.CurrentLocale);
            NFCNdefMessage nFCNdefMessage = new NFCNdefMessage(new NFCNdefPayload[] { payload });
            tag.WriteNdef(nFCNdefMessage, delegate
            {
                Console.WriteLine("Write Successful");
            });
            session.InvalidateSession();

        }
        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            //Session.InvalidateSession();
            Console.WriteLine("DidInvalidate entered");
            //Do the thing

        }

        public async Task StartReadScan()
        {
            _tcs = new TaskCompletionSource<string>();
            Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, true);
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
