using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Nfc;

using Google.Protobuf;
using NFCProject.Services;
using Android.Nfc.Tech;

namespace NFCProject.Droid
{
    [Activity(Label = "NfcActivity")]
    public class NfcActivity : Activity
    {

        private NfcAdapter _nfcAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            Console.WriteLine("YEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEET");
        }

        protected override void OnResume()
        {
            base.OnResume();

            var tagDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            var filters = new[] { tagDetected };

            var intent = new Intent(this, this.GetType()).AddFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        protected override void OnNewIntent(Intent intent)
        {
            var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
            if (tag != null)
            {
                var rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                if (rawMessages != null)
                {
                    NdefMessage msg = (NdefMessage)rawMessages[0];

                    NdefRecord[] record = msg.GetRecords();

                        if (record != null)
                        {
                            RX1_NFC_Reply nfcReply = RX1_NFC_Reply.Parser.ParseFrom(record[0].GetPayload());

                            byte[] nonce = nfcReply.Nonce.ToByteArray();
                            Console.WriteLine("Nonce: " + nfcReply);

                            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c");
                            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f");

                            CryptoHandler cryptoHandler = new CryptoHandler();

                            byte[] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV);

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

                            byte[] payload = nfcRequest.ToByteArray();

                            Ndef ndef = Ndef.Get(tag);
                            if (ndef != null && ndef.IsWritable)
                            {
                                NdefRecord replyRecord = new NdefRecord(NdefRecord.TnfUnknown, new byte[0], new byte[0], payload);
                                NdefMessage ndefMessage = new NdefMessage(new[] { replyRecord });

                                ndef.Connect();
                                ndef.WriteNdefMessage(ndefMessage);

                                System.Threading.Thread.Sleep(1000);

                                NdefMessage configMessage = ndef.NdefMessage;

                                NdefRecord[] configRecord = configMessage.GetRecords();
                                if (configRecord != null)
                                {
                                    RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(configRecord[0].GetPayload());
                                    RX1_Uplink_Config nodeConfig = nfcSecondReply.NodeConfig;
                                    Console.WriteLine(nodeConfig);
                                }

                            }


                        }

                }
                }
            
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