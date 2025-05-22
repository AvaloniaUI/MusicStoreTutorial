using Avalonia;
using Avalonia.Controls;
using Avalonia.MusicStore.ViewModels;

namespace Avalonia.MusicStore.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Design.IsDesignMode) return;

            this.Opened += (_, _) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.OnShowDialog = async (musicStoreVm) =>
                    {
                        var dialog = new MusicStoreWindow
                        {
                            DataContext = musicStoreVm
                        };

                        return await dialog.ShowDialog<AlbumViewModel?>(this);
                    };
                }
            };
        }
    }
}