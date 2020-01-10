using System;
using System.IO;
using Xamarin.Forms;
using NFCProject.Services;

namespace NFCProject.Pages
{
    public partial class ReadFromNode : ContentPage
    {
        public ReadFromNode ()
        {

            InitializeComponent ();

        }

        async void iosScan(object sender, System.EventArgs e)
        {
            IReadScan service = DependencyService.Get<IReadScan>(DependencyFetchTarget.NewInstance);
            service.StartReadScan();

        }
    }
}