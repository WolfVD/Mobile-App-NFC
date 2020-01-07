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


namespace NFCProject.iOS
{
    class WriteScan : NFCNdefReaderSessionDelegate, IWriteScan
    {
        string netIDWrite; 
        string netChanWrite;
        string nodeConfigWrite;
        string operModeWrite;
        string encKeyWrite;
        string authKeyWrite;
        string updateRateWrite;

        bool netIDBool;
        bool netChanBool;
        bool NodeConfigBool;
        bool OperModeBool;
        bool EncKeyBool;
        bool AuthKeyBool;
        bool UpdateRateBool;

        byte[] trimmedResult;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            //This is left empty on purpose because it will never be called
        }

        public uint calculateChksum() 
        {
            int chksum = 0;

            chksum += Convert.ToInt32(netIDWrite);
            chksum += Convert.ToInt32(netChanWrite);
            chksum += Convert.ToInt32(nodeConfigWrite);
            chksum += Convert.ToInt32(operModeWrite);
            for (int i = 0; i < 16; i++) {
                chksum += hexToByte(encKeyWrite)[i]&0xff;
            }
            for (int i = 0; i < 16; i++)
            {
                chksum += hexToByte(authKeyWrite)[i] & 0xff;
            }

            chksum += netIDBool ? 1 : 0;
            chksum += netChanBool ? 1 : 0;
            chksum += NodeConfigBool ? 1 : 0;
            chksum += OperModeBool ? 1 : 0;
            chksum += EncKeyBool ? 1 : 0;
            chksum += AuthKeyBool ? 1 : 0;

            return Convert.ToUInt32(chksum);
        }

        [Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            INFCNdefTag tag = tags[0];
            session.ConnectToTag(tag, delegate { });

            Action<NFCNdefMessage, NSError> readNonce;
            readNonce = EncryptNonce;

            tag.ReadNdef(readNonce);

            NodeConfiguration nodeConfiguration;
            NodeOperatingMode operatingMode;

            #region fix garbage later
            if (nodeConfigWrite == "0")
            {
                nodeConfiguration = NodeConfiguration.Desk1M;
            }
            else if (nodeConfigWrite == "1")
            {
                nodeConfiguration = NodeConfiguration.Desk2M;
            }
            else if (nodeConfigWrite == "2")
            {
                nodeConfiguration = NodeConfiguration.Ceiling1M;
            }
            else {
                nodeConfiguration = NodeConfiguration.Ceiling2M;
            }

            if (operModeWrite == "0")
            {
                operatingMode = NodeOperatingMode.Run;
            }
            else {
                operatingMode = NodeOperatingMode.Inventory;
            }
            #endregion fix garbage later

            RX1_NFC_Request nfcRequest = new RX1_NFC_Request
            {
                RequestType = RX1_NFC_Request.Types.NFCRequestType.SetNodeConfig,
                EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                NodeConfig = new RX1_NFC_Config {
                    NetworkID = (Convert.ToUInt32(netIDWrite)),
                    HasNetworkID = netIDBool,
                    NetworkChannel = (Convert.ToUInt32(netChanWrite)),
                    HasNetworkChannel = netChanBool,
                    NodeConfiguration = nodeConfiguration,
                    HasNodeConfiguration = NodeConfigBool,
                    OperatingMode = operatingMode,
                    HasOperatingMode = OperModeBool,
                    EncryptionKey = (ByteString.CopyFrom(hexToByte(encKeyWrite))),
                    HasEncryptionKey = EncKeyBool,
                    AuthenticationKey = (ByteString.CopyFrom(hexToByte(authKeyWrite))),
                    HasAuthenticationKey = AuthKeyBool

                },
                Chksum = calculateChksum()
                
            };

            string nfcReplyPayload = nfcRequest.ToString(); //Convert to a request to a string so it can be written

            //Create a payload/message and write it.
            NFCNdefPayload writePayload = NFCNdefPayload.CreateWellKnownTypePayload(nfcReplyPayload, NSLocale.CurrentLocale);
            NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
            tag.WriteNdef(writeMessage, delegate { Console.WriteLine("Write succesful"); });

            System.Threading.Thread.Sleep(1000);

            Action<NFCNdefMessage, NSError> readNodeReply;
            readNodeReply = ValidateWrite;
            tag.ReadNdef(ValidateWrite);

            session.InvalidateSession();



        }

        public void ValidateWrite(NFCNdefMessage message, NSError error) {
            NSData readPayload = message.Records[0].Payload;
            byte[] bytes = readPayload.ToArray();

            RX1_NFC_Reply nfcSecondReply;
            nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(bytes);

            if (nfcSecondReply.SetNodeConfigAcknowledge) {
                Console.WriteLine("Data Written to Node Successfully.  Node will apply settings in 5 seconds and subsequently reset");
            }
        }

        private void EncryptNonce(NFCNdefMessage message, NSError error)
        {
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

            byte[] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

            //Trim the encrypted nonce to a length of 16 bytes
            trimmedResult = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                trimmedResult[i] = encryptedNonce[i];
            }
        }

        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            //Purposefuly left empty
        }

        public async Task StartWriteScan(string netID, string netChan, string nodeConfig, string operMode, string encKey, string authKey, string updateRate, bool netIDOn, bool netChanOn, bool NodeConfigOn, bool OperModeOn, bool EncKeyOn, bool AuthKeyOn, bool UpdateRateOn)
        {
            netIDWrite = netID;
            netChanWrite = netChan;
            nodeConfigWrite = nodeConfig;
            operModeWrite = operMode;
            encKeyWrite = encKey;
            authKeyWrite = authKey;
            updateRateWrite = updateRate;

            netIDBool = netIDOn;
            netChanBool = netChanOn;
            NodeConfigBool = NodeConfigOn;
            OperModeBool = OperModeOn;
            EncKeyBool = EncKeyOn;
            AuthKeyBool = AuthKeyOn;
            UpdateRateBool = UpdateRateOn;

            NFCNdefReaderSession Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
            Session.BeginSession();
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