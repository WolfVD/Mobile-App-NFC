 using NFCProject.Pages;
using Xamarin.Forms;

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
