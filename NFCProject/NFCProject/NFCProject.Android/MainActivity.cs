using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Google.Protobuf;
using NFCProject.Pages;
using NFCProject.Services;
using System;

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

        static string currentPage;

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
                    try
                    {
                        if (WriteToNode.onSaved == false)
                        {

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
                            ndef.Close();

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
                            Toast.MakeText(ApplicationContext, "Read Succesful", ToastLength.Long).Show();

                            ReadFromNode readValues = new ReadFromNode();
                            readValues.DisplayValues(valueList);

                        }
                        else
                        {
                            WriteToNode.onSaved = false;

                            NodeConfiguration nodeConfiguration;
                            NodeOperatingMode operatingMode;

                            WriteToNode writeNodePage = new WriteToNode();

                            bool[] checkedList = writeNodePage.ReturnChecked();

                            string[] valueList = writeNodePage.ReturnValues();

                            uint NetID = Convert.ToUInt32(valueList[0]);
                            uint NetChan = Convert.ToUInt32(valueList[1]);
                            string NodeConfig = valueList[2];
                            string OperMode = valueList[3];
                            ByteString EncKey = ByteString.CopyFrom(hexToByte(valueList[4]));
                            ByteString AuthKey = ByteString.CopyFrom(hexToByte(valueList[5]));
                            string UpdateRate = valueList[6];

                            bool NetIDBool = checkedList[0];
                            bool NetChanBool = checkedList[1];
                            bool NodeConfigBool = checkedList[2];
                            bool OperModeBool = checkedList[3];
                            bool EncKeyBool = checkedList[4];
                            bool AuthKeyBool = checkedList[5];
                            bool UpdateRateBool = checkedList[6];

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
                                    NetworkID = NetID,
                                    HasNetworkID = NetIDBool,
                                    NetworkChannel = NetChan,
                                    HasNetworkChannel = NetChanBool,
                                    NodeConfiguration = nodeConfiguration,
                                    HasNodeConfiguration = NodeConfigBool,
                                    OperatingMode = operatingMode,
                                    HasOperatingMode = OperModeBool,
                                    EncryptionKey = EncKey,
                                    HasEncryptionKey = EncKeyBool,
                                    AuthenticationKey = AuthKey,
                                    HasAuthenticationKey = AuthKeyBool

                                },
                                Chksum = calculateChksum(valueList[0], valueList[1], valueList[2], valueList[3], valueList[4], valueList[5], checkedList[0], checkedList[1], checkedList[2], checkedList[3], checkedList[4], checkedList[5])

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

                            if (nfcSecondReply.SetNodeConfigAcknowledge)
                            {
                                Toast.MakeText(ApplicationContext, "Write Succesful", ToastLength.Long).Show();
                            }
                        }
                    }
                    catch
                    {
                        Toast.MakeText(ApplicationContext, "Could not communicate with NDEF Tag, please try again", ToastLength.Long).Show();
                        Console.WriteLine("Yeet");
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