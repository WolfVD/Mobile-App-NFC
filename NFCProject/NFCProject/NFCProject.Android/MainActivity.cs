using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using NFCProject.Pages;
using Google.Protobuf;
using System;

namespace NFCProject.Droid
{
    [Activity(Label = "NFCProject", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
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

        byte[] trimmedResult;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //Create an NFC Adapter that can detect
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            nfcAdapter = NfcAdapter.GetDefaultAdapter(Application.Context);
            nfcPendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
            var nfcIntentFiltersArrays = new[] { techDetected };

            if (NfcAdapter.ActionNdefDiscovered.Equals(Intent.Action) == true)
            {
                OnNewIntent(Intent);
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

        protected override void OnNewIntent(Intent intent) //When a new NFC tag is discovered
        {
            try
            {
                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

                IParcelable[] rawMsgs = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
                if (rawMsgs != null)
                {
                    NdefMessage message = (NdefMessage)rawMsgs[0];
                    NdefRecord[] records = message.GetRecords();
                    if (records != null)
                    {
                        if (MainPage.currentPage == "Read From Node") //If current page is read page
                        {
                            ReadNFC.DisplayValues(records[0].GetPayload()); //Encrypt nonce and create reply
                        }
                        else //If current page is write page
                        {

                            Ndef ndef = Ndef.Get(tag);
                            ndef.Connect();

                            byte[] bytes = WriteNFC.CreateRequest();

                            NdefRecord newRecord = new NdefRecord(NdefRecord.TnfUnknown, new byte[0], new byte[0], bytes);
                            NdefMessage newMessage = new NdefMessage(new NdefRecord[] { newRecord });
                            ndef.WriteNdefMessage(newMessage);
                            ndef.Close();
                            Toast.MakeText(ApplicationContext, "Write Succesful", ToastLength.Long).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "The Tag did not contain a message.", ToastLength.Long).Show();
                }
            }
            catch
            {
                Toast.MakeText(ApplicationContext, "Something went wrong, please try again.", ToastLength.Long).Show();
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