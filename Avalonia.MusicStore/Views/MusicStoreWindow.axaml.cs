using System;
using Avalonia.Controls;
using Avalonia.MusicStore.ViewModels;

namespace Avalonia.MusicStore.Views
{
    public partial class MusicStoreWindow : Window
    {
        public MusicStoreWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (DataContext is MusicStoreViewModel vm)
            {
                vm.AlbumPurchased += (album) => { Close(album); };
            }
        }
    }
}