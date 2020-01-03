using Xamarin.Forms;
using System;
using System.IO;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class WriteToNode : ContentPage
    {
        private string classId;

        private string netID = "N/A";
        private string netIDFinal = null;
        private bool netIDOn = false;

        private string netChan = "N/A";
        private string netChanFinal = null;
        private bool netChanOn = false;

        private string configID = "N/A";
        private string configIDFinal = null;
        private bool configIDOn = false;

        private string encKeyCom = "N/A";
        private string encKeyComFinal = null;
        private bool encKeyComOn = false;

        private string authKeyCom = "N/A";
        private string authKeyComFinal = null;
        private bool authKeyComOn = false;

        private string encKeyOTAP = "N/A";
        private string encKeyOTAPFinal = null;
        private bool encKeyOTAPOn = false;

        private string authKeyOTAP = "N/A";
        private string authKeyOTAPFinal = null;
        private bool authKeyOTAPOn = false;

        private string operMode = "N/A";
        private string operModeFinal = null;
        private bool operModeOn = false;
        public WriteToNode()
        {
            InitializeComponent();
        }

        void Entry_CheckBox(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            string classId = checkbox.ClassId;


            if (classId == "NetID")
            {
                if (netIDOn == false) {
                    netIDOn = true; }
                else { 
                    netIDOn = false; }
            }
            else if (classId == "NetChan")
            {
                if (netChanOn == false) { 
                    netChanOn = true; }
                else { 
                    netChanOn = false; }
            }
            else if (classId == "ConfigID")
            {
                if (configIDOn == false) { 
                    configIDOn = true; }
                else { 
                    configIDOn = false; }
            }
            else if (classId == "EncKeyCom")
            {
                if (encKeyComOn == false) { 
                    encKeyComOn = true; }
                else { 
                    encKeyComOn = false; }
            }
            else if (classId == "AuthKeyCom")
            {
                if (authKeyComOn == false) { 
                    authKeyComOn = true; }
                else { 
                    authKeyComOn = false; }
            }
            else if (classId == "EncKeyOTAP")
            {
                if (encKeyOTAPOn == false) { 
                    encKeyOTAPOn = true; }
                else { 
                    encKeyOTAPOn = false; }
            }
            else if (classId == "AuthKeyOTAP")
            {
                if (authKeyOTAPOn == false) { 
                    authKeyOTAPOn = true; }
                else { 
                    authKeyOTAPOn = false; }
            }
            else if (classId == "OperMode")
            {
                if (operModeOn == false) { 
                    operModeOn = true; }
                else { 
                    operModeOn = false; }
            }
        }

        void OnTextChanged(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            classId = entry.ClassId;

            if (classId == "NetID") { 
                netID = entry.Text; }
            else if (classId == "NetChan") { 
                netChan = entry.Text; }
            else if (classId == "ConfigID") { 
                configID = entry.Text; }
            else if (classId == "EncKeyCom") { 
                encKeyCom = entry.Text; }
            else if (classId == "AuthKeyCom") { 
                authKeyCom = entry.Text; }
            else if (classId == "EncKeyOTAP") { 
                encKeyOTAP = entry.Text; }
            else if (classId == "AuthKeyOTAP") { 
                authKeyOTAP = entry.Text; }
            else if (classId == "OperMode") { 
                operMode = entry.Text; }
        }

        void SaveValues(object sender, System.EventArgs e)
        {
            if (netIDOn == true) {
                netIDFinal = netID; 
            } if (netChanOn == true) {
                netChanFinal = netChan;
            } if (configIDOn == true) {
                configIDFinal = configID;
            } if (encKeyComOn == true) {
                encKeyComFinal = encKeyCom;
            } if (authKeyComOn == true) {
                authKeyComFinal = authKeyCom;
            } if (encKeyOTAPOn == true) {
                encKeyOTAPFinal = encKeyOTAP;
            }  if (authKeyOTAPOn == true) {
                authKeyOTAPFinal = authKeyOTAP;
            } if (operModeOn == true) {
                operModeFinal = operMode;
            }

            DisplayAlert("List of values:", $"Network ID = {netIDFinal}; Network Channel = {netChanFinal}; Configuration ID = {configIDFinal}; Encryption Key for Communication = {encKeyComFinal};" +
                $" Authentication Key for Communication = {authKeyComFinal}; Encryption Key for OTAP = {encKeyOTAPFinal}; Authentication Key for OTAP = {authKeyOTAPFinal}; Operating Mode = {operModeFinal}" , "Ok");
        }
        async void iosScan(object sender, System.EventArgs e)
        {
            IWriteScan service = DependencyService.Get<IWriteScan>(DependencyFetchTarget.NewInstance);
            await service.StartWriteScan(netIDFinal, netChanFinal, configIDFinal, encKeyComFinal, authKeyComFinal, encKeyOTAPFinal, authKeyOTAPFinal, operModeFinal);
        }
    }
}