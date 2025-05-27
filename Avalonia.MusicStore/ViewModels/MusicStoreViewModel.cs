using Avalonia.MusicStore.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.MusicStore.ViewModels
{
    public partial class MusicStoreViewModel : ViewModelBase
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _searchDebounceCts;
        public event Action<AlbumViewModel>? AlbumPurchased;

        [ObservableProperty] public partial string? SearchText { get; set; }
        
        [ObservableProperty] public partial bool IsBusy { get; private set; }
        
        [ObservableProperty] public partial AlbumViewModel? SelectedAlbum { get; set; }

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();


        public MusicStoreViewModel()
        {
            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText))
                {
                    await SearchWithDelayAsync(SearchText);
                }
            };
        }

        private async Task SearchWithDelayAsync(string? term)
        {
            _searchDebounceCts?.Cancel();
            _searchDebounceCts = new CancellationTokenSource();
            var token = _searchDebounceCts.Token;

            try
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested && !string.IsNullOrWhiteSpace(term))
                {
                    DoSearch(term);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task DoSearch(string term)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsBusy = true;
            SearchResults.Clear();

            var albums = await Album.SearchAsync(term);

            foreach (var album in albums)
            {
                var vm = new AlbumViewModel(album);
                SearchResults.Add(vm);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                LoadCovers(cancellationToken);
            }

            IsBusy = false;
        }


        private async void LoadCovers(CancellationToken cancellationToken)
        {
            foreach (var album in SearchResults.ToList())
            {
                await album.LoadCover();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        [RelayCommand]
        private void BuyMusic()
        {
            if (SelectedAlbum != null)
            {
                AlbumPurchased?.Invoke(SelectedAlbum);
            }
        }
    }
}