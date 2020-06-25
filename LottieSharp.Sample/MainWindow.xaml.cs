using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace LottieSharp.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LottieAnimationView.UseHardwareAcceleration(true);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LottieAnimationView.PauseAnimation();
            LottieAnimationView.Progress = (float)(e.NewValue / 1000);
        }

        private void LoadAnimation_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Json files|*.json|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                LottieAnimationView.PauseAnimation();
                LottieAnimationView.FileName = openFileDialog.FileName;
                LottieAnimationView.PlayAnimation();
            }
        }

        private void StartAnimation_Click(object sender, RoutedEventArgs e)
        {
            LottieAnimationView.PlayAnimation();
        }

        private void PauseAnimation_Click(object sender, RoutedEventArgs e)
        {
            LottieAnimationView.PauseAnimation();
        }

        private void LoadImageAssetsFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ImageAssetsFolderTextBox.Text = dialog.SelectedPath;
            }
        }

        private void DeleteImageAssetsFolder_Click(object sender, RoutedEventArgs e)
        {
            ImageAssetsFolderTextBox.Text = "";
        }

        private void ImageAssetsFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LottieAnimationView.PauseAnimation();
            LottieAnimationView.ImageAssetsFolder = ImageAssetsFolderTextBox.Text;
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!double.IsNaN(e.NewValue))
                LottieAnimationView.Scale = (float)e.NewValue;
        }
    }
}
