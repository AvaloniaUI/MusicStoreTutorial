using Avalonia.MusicStore.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<AlbumViewModel> Albums { get; } = new();

        public Func<MusicStoreViewModel, Task<AlbumViewModel?>>? OnShowDialog { get; set; }

        public MainWindowViewModel()
        {
            Task.Run(LoadAlbums);
        }

        [RelayCommand]
        private async Task AddAlbumAsync()
        {
            var store = new MusicStoreViewModel();

            if (OnShowDialog is not null)
            {
                var result = await OnShowDialog(store);
                if (result != null)
                {
                    Albums.Add(result);
                    await result.SaveToDiskAsync();
                }
            }
        }

        private async void LoadAlbums()
        {
            var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

            foreach (var album in albums)
            {
                Albums.Add(album);
            }

            foreach (var album in Albums.ToList())
            {
                await album.LoadCover();
            }
        }
    }
}