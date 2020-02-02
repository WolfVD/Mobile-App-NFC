using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace NFCProject.Pages
{
    public class SensorView
    {

        static bool writePage;
        public static int _config;

        public static Grid GetView(bool checkBox1Checked, bool checkBox2Checked, bool checkBox3Checked, bool checkBox4Checked, bool checkBox5Checked)
        {
            if (MainPage.currentPage == "Read From Node")
            {
                writePage = false;
            }
            else
            {
                writePage = true;
            }

            Grid grid = new Grid
            {
                Padding = 15,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = GridLength.Auto },
                    new ColumnDefinition {Width = GridLength.Auto },
                    new ColumnDefinition {Width = GridLength.Auto },

                }

            };

            var checkBox1 = new CheckBox() { IsEnabled = writePage, IsChecked = checkBox1Checked, Scale = 1.2, HeightRequest = 50 };
            checkBox1.CheckedChanged += (sender, e) =>
            {
                CheckBox checkBox = (CheckBox)sender;
                ChangeConfig(0, checkBox.IsChecked);
            };
            var label1 = new Label() { Text = "Temp, Hum, Air", FontSize = 20, HeightRequest = 50, TranslationY = 7, HorizontalTextAlignment = TextAlignment.Start };

            var checkBox2 = new CheckBox() { IsEnabled = writePage, IsChecked = checkBox2Checked, Scale = 1.2, HeightRequest = 50 };
            checkBox2.CheckedChanged += (sender, e) =>
            {
                CheckBox checkBox = (CheckBox)sender;
                ChangeConfig(1, checkBox.IsChecked);
            };
            var label2 = new Label() { Text = "Magnetic", FontSize = 20, HeightRequest = 50, TranslationY = 7, HorizontalTextAlignment = TextAlignment.Start };

            var checkBox3 = new CheckBox() { IsEnabled = writePage, IsChecked = checkBox3Checked, Scale = 1.2, HeightRequest = 50 };
            checkBox3.CheckedChanged += (sender, e) =>
            {
                CheckBox checkBox = (CheckBox)sender;
                ChangeConfig(2, checkBox.IsChecked);
            };
            var label3 = new Label() { Text = "Audio", FontSize = 20, HeightRequest = 50, TranslationY = 7, HorizontalTextAlignment = TextAlignment.Start };

            var checkBox4 = new CheckBox() { IsEnabled = writePage, IsChecked = checkBox4Checked, Scale = 1.2, HeightRequest = 50 };
            checkBox4.CheckedChanged += (sender, e) =>
            {
                CheckBox checkBox = (CheckBox)sender;
                ChangeConfig(3, checkBox.IsChecked);
            };
            var label4 = new Label() { Text = "Accelerometer", FontSize = 20, HeightRequest = 50, TranslationY = 7, HorizontalTextAlignment = TextAlignment.Start };

            var checkBox5 = new CheckBox() { IsEnabled = writePage, IsChecked = checkBox5Checked, Scale = 1.2, HeightRequest = 50 };
            checkBox5.CheckedChanged += (sender, e) =>
            {
                CheckBox checkBox = (CheckBox)sender;
                ChangeConfig(4, checkBox.IsChecked);
            };
            var label5 = new Label() { Text = "Light", FontSize = 20, HeightRequest = 50, TranslationY = 7, HorizontalTextAlignment = TextAlignment.Start };

            grid.Children.Add(checkBox1, 0, 0);
            grid.Children.Add(label1, 1, 0);

            grid.Children.Add(checkBox2, 0, 1);
            grid.Children.Add(label2, 1, 1);

            grid.Children.Add(checkBox3, 0, 2);
            grid.Children.Add(label3, 1, 2);

            grid.Children.Add(checkBox4, 0, 3);
            grid.Children.Add(label4, 1, 3);

            grid.Children.Add(checkBox5, 0, 4);
            grid.Children.Add(label5, 1, 4);

            return grid;
        }
        public static void ChangeConfig(int row, bool isChecked)
        {
            if (writePage == true)
            {
                switch (row)
                {
                    case 0:
                        WriteToNode.checkBox1Checked = isChecked;
                        if (isChecked)
                            _config |= 0x0001;
                        else
                            _config &= ~0x0001;
                        break;
                    case 1:
                        WriteToNode.checkBox2Checked = isChecked;
                        if (isChecked)
                            _config |= 0x0004;
                        else
                            _config &= ~0x0004;
                        break;
                    case 2:
                        WriteToNode.checkBox3Checked = isChecked;
                        if (isChecked)
                            _config |= 0x0008;
                        else
                            _config &= ~0x0008;
                        break;
                    case 3:
                        WriteToNode.checkBox4Checked = isChecked;
                        if (isChecked)
                            _config |= 0x0020;
                        else
                            _config &= ~0x0020;
                        break;
                    case 4:
                        WriteToNode.checkBox5Checked = isChecked;
                        if (isChecked)
                            _config |= 0x0040;
                        else
                            _config &= ~0x0040;
                        break;
                }
            }
        }
    }
}
