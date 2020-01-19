using NFCProject.Pages;
using System;
using Xamarin.Forms;
using NFCProject.Services;

namespace NFCProject
{
    public partial class MainPage : TabbedPage
    {

        public static string currentPage = "Read From Node";

        public MainPage()
        {
            InitializeComponent();

            this.Children.Add(new ReadFromNode());
            this.Children.Add(new WriteToNode());

            CurrentPageChanged += CurrentPageHasChanged;

        }

        private void CurrentPageHasChanged(object sender, EventArgs e)
        {
            var tabbedPage = (TabbedPage)sender;
            Title = tabbedPage.CurrentPage.Title;
            currentPage = Title;
        }

    }
}
