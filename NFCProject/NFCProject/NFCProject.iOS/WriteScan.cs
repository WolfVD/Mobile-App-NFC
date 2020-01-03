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
        private NFCNdefReaderSession Session;
        private TaskCompletionSource<string> _tcs;
        string nfcReplyPayload;
        INFCNdefTag tag;
        bool writeNonce = false;

        public override void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            throw new NotImplementedException();
        }

        public override void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            throw new NotImplementedException();
        }

        public Task StartWriteScan(string netID, string netChan, string configID, string encKeyCom, string authKeyCom, string encKeyOTAP, string authKeyOTAP, string operMode)
        {
            throw new NotImplementedException();
        }
    }
}