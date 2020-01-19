using CoreFoundation;
using CoreNFC;
using Foundation;
using NFCProject.Services;
using System;
using UIKit;

namespace NFCProject.iOS
{
    public class StartNFC : NFCNdefReaderSessionDelegate, IStartNFC
    {

        NFCNdefReaderSession Session;
        INFCNdefTag tag;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            //This will never be entered
        }

        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            //Fired when nfc session is invalidated.
        }

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

        public void EncryptNonce(NFCNdefMessage message, NSError error)
        {
            NFCNdefPayload messageRecord = message.Records[0];

            if (MainPage.currentPage == "Read From Node") //If current page is read page
            {
                byte[] bytes = ReadNFC.EncryptNonceRead(messageRecord.Payload.ToArray()); //Encrypt nonce and create reply

                //Write reply
                NFCNdefPayload writePayload = new NFCNdefPayload(NFCTypeNameFormat.Unknown, NSData.FromArray(new byte[0]), NSData.FromArray(new byte[0]), NSData.FromArray(bytes));
                NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
                tag.WriteNdef(writeMessage, delegate { });

                System.Threading.Thread.Sleep(1000); //Wait 1 second

                //Read node config
                Action<NFCNdefMessage, NSError> readNodeConfig;
                readNodeConfig = GetNodeConfig;
                tag.ReadNdef(readNodeConfig);

            }
            else // If current page is write page
            {
                byte[] bytes = WriteNFC.EncryptNonceWrite(messageRecord.Payload.ToArray()); //Encrypt nonce and create reply

                //Write reply
                NFCNdefPayload writePayload = new NFCNdefPayload(NFCTypeNameFormat.Unknown, NSData.FromArray(new byte[0]), NSData.FromArray(new byte[0]), NSData.FromArray(bytes));
                NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
                tag.WriteNdef(writeMessage, delegate { });

                System.Threading.Thread.Sleep(1000); //Wait 1 second

                //Read write confirmation
                Action<NFCNdefMessage, NSError> writeNodeConfig;
                writeNodeConfig = GetNodeWrite;
                tag.ReadNdef(writeNodeConfig);
            }

        }

        public void GetNodeConfig(NFCNdefMessage message, NSError error) //Get and display values from node config
        {
            NFCNdefPayload messageRecord = message.Records[0];

            ReadNFC.GetValues(messageRecord.Payload.ToArray());

            Session.InvalidateSession();
        }

        public void GetNodeWrite(NFCNdefMessage message, NSError error) //Check if write was succesful
        {
            NFCNdefPayload messageRecord = message.Records[0];

            RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(messageRecord.Payload.ToArray());

            if (nfcSecondReply.SetNodeConfigAcknowledge)
            {
                Console.WriteLine("Write succesful");
            }
            Session.InvalidateSession();
        }

        public void StartScan() //Start an NFC Session
        {
            Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
            Session.BeginSession();
        }
    }
}