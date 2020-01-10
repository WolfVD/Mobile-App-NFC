using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Nfc;
using Google.Protobuf;
using NFCProject.Services;

using Android.Nfc.Tech;
using Android.Content;

namespace NFCProject.Droid
{
    [Activity(Label = "NFCProject", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { NfcAdapter.ActionTechDiscovered },
      Categories = new[] { Intent.CategoryDefault },
      DataScheme = "vnd.android.nfc",
      DataPathPrefix = "/com.nfcproject:RX1Type",
      DataHost = "ext")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        PendingIntent nfcPendingIntent; 
        IntentFilter[] nfcIntentFiltersArray;
        private NfcAdapter nfcAdapter;

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
        string gatewayConnect;


        byte[] trimmedResult;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            nfcAdapter = NfcAdapter.GetDefaultAdapter(Application.Context);
            nfcPendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
            var nfcIntentFiltersArrays = new[] { techDetected };

            if (NfcAdapter.ActionNdefDiscovered.Equals(Intent.Action) == true)
            {
                HandleNFC(Intent);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            if (nfcAdapter != null)
            {
                nfcAdapter.EnableForegroundDispatch(this, nfcPendingIntent, nfcIntentFiltersArray, null);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            HandleNFC(intent);
        }

        protected void HandleNFC(Intent intent)
        {
            var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

            IParcelable[] rawMsgs = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            if (rawMsgs != null)
            {
                NdefMessage message = (NdefMessage)rawMsgs[0];
                NdefRecord[] records = message.GetRecords();
                if (records != null)
                {
                        RX1_NFC_Reply nfcReply = RX1_NFC_Reply.Parser.ParseFrom(records[0].GetPayload());
                        byte[] nonce = nfcReply.Nonce.ToByteArray();

                        byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c");
                        byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f");

                        CryptoHandler cryptoHandler = new CryptoHandler();

                        byte[] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

                        trimmedResult = new byte[16];

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

                        Ndef ndef = Ndef.Get(tag);
                        ndef.Connect();

                        byte[] bytes = nfcRequest.ToByteArray();
                        NdefRecord newRecord = new NdefRecord(NdefRecord.TnfUnknown, new byte[0], new byte[0], bytes);
                        NdefMessage newMessage = new NdefMessage(new NdefRecord[] { newRecord });
                        ndef.WriteNdefMessage(newMessage);

                        System.Threading.Thread.Sleep(1000); //Wait 1 second

                        NdefMessage secondMessage = ndef.NdefMessage; 
                        NdefRecord[] secondRecords = secondMessage.GetRecords();

                        RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(secondRecords[0].GetPayload());
                        Console.WriteLine(nfcSecondReply);
                    
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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