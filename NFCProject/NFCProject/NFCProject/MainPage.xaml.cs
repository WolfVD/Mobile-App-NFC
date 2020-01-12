using NFCProject.Pages;
using System;
using Xamarin.Forms;
using NFCProject.Services;

namespace NFCProject
{
    public partial class MainPage : TabbedPage
    {

        public MainPage()
        {
            InitializeComponent();

            this.Children.Add(new ReadFromNode());
            this.Children.Add(new WriteToNode());

        }

    }
}
