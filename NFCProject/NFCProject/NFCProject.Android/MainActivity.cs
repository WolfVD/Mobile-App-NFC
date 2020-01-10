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
using NFCProject.Pages;

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

        public uint calculateChksum(string NetID, string NetChan, string NodeConfig, string OperMode, string EncKey, string AuthKey, bool NetIDBool, bool NetChanBool, bool NodeConfigBool, bool OperModeBool, bool EncKeyBool, bool AuthKeyBool)
        {
            int chksum = 0;

            chksum += Convert.ToInt32(NetID);
            chksum += Convert.ToInt32(NetChan);
            chksum += Convert.ToInt32(NodeConfig);
            chksum += Convert.ToInt32(OperMode);
            for (int i = 0; i < 16; i++)
            {
                chksum += hexToByte(EncKey)[i] & 0xff;
            }
            for (int i = 0; i < 16; i++)
            {
                chksum += hexToByte(AuthKey)[i] & 0xff;
            }

            chksum += NetIDBool ? 1 : 0;
            chksum += NetChanBool ? 1 : 0;
            chksum += NodeConfigBool ? 1 : 0;
            chksum += OperModeBool ? 1 : 0;
            chksum += EncKeyBool ? 1 : 0;
            chksum += AuthKeyBool ? 1 : 0;

            return Convert.ToUInt32(chksum);
        }

        protected void HandleNFC(Intent intent)
        {
            var currentPage = this.GetType().Name;
            Console.WriteLine(currentPage);

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

                    if (currentPage == "ReadFromNode")
                    {
                        Console.WriteLine("Read");
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
                    else {
                        Console.WriteLine("Write");
                        NodeConfiguration nodeConfiguration;
                        NodeOperatingMode operatingMode;

                        WriteToNode writeNodePage = new WriteToNode();

                        string NetID = writeNodePage.NetID;
                        string NetChan = writeNodePage.NetChan;
                        string NodeConfig = writeNodePage.NodeConfig;
                        string OperMode = writeNodePage.OperMode;
                        string EncKey = writeNodePage.EncKey;
                        string AuthKey = writeNodePage.AuthKey;
                        string UpdateRate = writeNodePage.UpdateRate;

                        bool NetIDBool = writeNodePage.NetIDBox.IsChecked;
                        bool NetChanBool = writeNodePage.NetChanBox.IsChecked;
                        bool NodeConfigBool = writeNodePage.NodeConfigBox.IsChecked;
                        bool OperModeBool = writeNodePage.OperModeBox.IsChecked;
                        bool EncKeyBool = writeNodePage.EncKeyBox.IsChecked;
                        bool AuthKeyBool = writeNodePage.AuthKeyBox.IsChecked;
                        bool UpdateRateBool = writeNodePage.UpdateRateBox.IsChecked;

                        #region fix garbage later
                        if (NodeConfig == "0")
                        {
                            nodeConfiguration = NodeConfiguration.Desk1M;
                        }
                        else if (NodeConfig == "1")
                        {
                            nodeConfiguration = NodeConfiguration.Desk2M;
                        }
                        else if (NodeConfig == "2")
                        {
                            nodeConfiguration = NodeConfiguration.Ceiling1M;
                        }
                        else
                        {
                            nodeConfiguration = NodeConfiguration.Ceiling2M;
                        }

                        if (OperMode == "0")
                        {
                            operatingMode = NodeOperatingMode.Run;
                        }
                        else
                        {
                            operatingMode = NodeOperatingMode.Inventory;
                        }
                        #endregion fix garbage later

                        RX1_NFC_Request nfcRequest = new RX1_NFC_Request
                        {
                            RequestType = RX1_NFC_Request.Types.NFCRequestType.SetNodeConfig,
                            EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                            NodeConfig = new RX1_NFC_Config
                            {
                                NetworkID = (Convert.ToUInt32(NetID)),
                                HasNetworkID = NetIDBool,
                                NetworkChannel = (Convert.ToUInt32(NetChan)),
                                HasNetworkChannel = NetChanBool,
                                NodeConfiguration = nodeConfiguration,
                                HasNodeConfiguration = NodeConfigBool,
                                OperatingMode = operatingMode,
                                HasOperatingMode = OperModeBool,
                                EncryptionKey = (ByteString.CopyFrom(hexToByte(EncKey))),
                                HasEncryptionKey = EncKeyBool,
                                AuthenticationKey = (ByteString.CopyFrom(hexToByte(AuthKey))),
                                HasAuthenticationKey = AuthKeyBool

                            },
                            Chksum = calculateChksum(NetID, NetChan, NodeConfig, OperMode, EncKey, AuthKey, NetIDBool, NetChanBool, NodeConfigBool, OperModeBool, EncKeyBool, AuthKeyBool)

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
                        Console.WriteLine(secondRecords[0].GetPayload());

                        RX1_NFC_Reply nfcSecondReply = RX1_NFC_Reply.Parser.ParseFrom(secondRecords[0].GetPayload());
                        Console.WriteLine(nfcSecondReply);
                    }

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