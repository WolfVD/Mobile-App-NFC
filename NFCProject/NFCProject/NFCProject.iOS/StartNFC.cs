using CoreFoundation;
using CoreNFC;
using Foundation;
using Google.Protobuf;
using NFCProject.Services;
using System;
using System.Text;
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

            Action<NFCNdefMessage, NSError> readValues;
            readValues = ReadValues;

            tag.ReadNdef(readValues); //Read NDEF tag and then encrypt the nonce
            

        }

        public void ReadValues(NFCNdefMessage message, NSError error)
        {
            try
            {
                
                Console.WriteLine(error);
                Console.WriteLine(message);
                NFCNdefPayload messageRecord = message.Records[0];

                if (MainPage.currentPage == "Read From Node") //If current page is read page
                {
                    
                    ReadNFC.DisplayValues(messageRecord.Payload.ToArray()); //Encrypt nonce and create reply
                    Session.InvalidateSession();
                }
                else // If current page is write page
                {
                    byte[] bytes = WriteNFC.CreateRequest();

                    NFCNdefPayload writePayload = new NFCNdefPayload(NFCTypeNameFormat.Unknown, NSData.FromArray(new byte[0]), NSData.FromArray(new byte[0]), NSData.FromArray(bytes));
                    NFCNdefMessage writeMessage = new NFCNdefMessage(new NFCNdefPayload[] { writePayload });
                    tag.WriteNdef(writeMessage, delegate { Console.WriteLine("Write"); });
                    Session.InvalidateSession();

                }
            }
            catch
            {
                Console.WriteLine("Did not contain a message");
            }

        }

        public void StartScan() //Start an NFC Session
        {
            Session = new NFCNdefReaderSession(this, DispatchQueue.CurrentQueue, false);
            Session.BeginSession();
        }
    }
}