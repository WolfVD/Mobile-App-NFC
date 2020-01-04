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
        string updateRateWrite;

        byte[] trimmedResult;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            //This is left empty on purpose because it will never be called
        }

        public int calculateChksum() 
        {
            int chksum = 0;

            return chksum;
        }

        [Foundation.Export("readerSession:didDetectTags:")]
        public override void DidDetectTags(NFCNdefReaderSession session, INFCNdefTag[] tags)
        {
            INFCNdefTag tag = tags[0];
            session.ConnectToTag(tag, delegate { });

            Action<NFCNdefMessage, NSError> readNonce;
            readNonce = EncryptNonce;

            tag.ReadNdef(readNonce);


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

        public async Task StartWriteScan(string netID, string netChan, string NodeConfig, string OperMode, string EncKey, string UpdateRate)
        {
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