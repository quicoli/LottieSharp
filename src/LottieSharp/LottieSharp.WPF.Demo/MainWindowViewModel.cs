using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;

namespace LottieSharp.WPF.Demo
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand CloseSplashScreenCommand { get; set; }


        private bool displaySplashScreen;
        public bool DisplaySplashScreen
        {
            get => displaySplashScreen;
            set => SetProperty(ref displaySplashScreen, value);
        }

        private void SetupCommands()
        {
            CloseSplashScreenCommand = new DelegateCommand(CloseSplashScreen);
        }

        private void CloseSplashScreen()
        {
            DisplaySplashScreen = false;
        }

        public MainWindowViewModel()
        {
            DisplaySplashScreen = true;
            SetupCommands();
            LoadAssets();
        }

        private void LoadAssets()
        {
            Assets = new ObservableCollection<AssetFile>();
            foreach (string? item in Directory.EnumerateFiles(@".\Assets"))
            {
                Assets.Add(new AssetFile(Path.GetFileName(item), item));
            }
            SelectedAsset = Assets[0];
        }

        private ObservableCollection<AssetFile> assets;
        public ObservableCollection<AssetFile> Assets
        {
            get => assets;
            set => SetProperty(ref assets, value);
        }

        private AssetFile selectedAsset;
        public AssetFile SelectedAsset
        {
            get => selectedAsset;
            set => SetProperty(ref selectedAsset, value);
        }

    }

    public class AssetFile : BindableBase
    {
        private string filename;
        public string Filename
        {
            get => filename;
            set => SetProperty(ref filename, value);
        }

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set => SetProperty(ref filePath, value);
        }

        public AssetFile(string filename, string filePath)
        {
            Filename = filename;
            FilePath = filePath;
        }
    }
}
