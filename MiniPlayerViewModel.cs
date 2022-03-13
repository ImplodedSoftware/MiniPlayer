using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MiniPlayer.Properties;
using NeonScripting.Models;

namespace MiniPlayer
{
    public class MiniPlayerViewModel : INotifyPropertyChanged
    {
        private bool BlockRatingChanges { get; set; }
        public Action UpdateUiAction { get; set; }
        private bool _isFirstInvocation = true;
        public RelayCommand PreviousCommand { get; }
        public RelayCommand PlayPauseCommand { get; }
        public RelayCommand StopCommand { get; }
        public RelayCommand NextCommand { get; }
        public RelayCommand SetFavouriteCommand { get; }

        private Timer _playerTimer;
        public MiniPlayerViewModel()
        {
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            PreviousCommand = new RelayCommand(o =>
            {
                remoteCalls.Previous(); 
            }, o =>
            {
                return remoteCalls.CanPlayPrevious;
            });
            PlayPauseCommand = new RelayCommand(o =>
            {
                remoteCalls.Pause();
            }, o =>
            {
                return true;
            });
            StopCommand = new RelayCommand(o =>
            {
                remoteCalls.Stop();
            }, o => { return true; });
            NextCommand = new RelayCommand(o =>
            {
                remoteCalls.Next();    
            }, o => { return true; });
            SetFavouriteCommand = new RelayCommand(o =>
            {
                IsFavourite = !IsFavourite;
                PluginHostHandler.Instance.ScriptHost.RemoteCalls.SetCurrentTrackAsFavourite(IsFavourite);
            }, o => { return true; });

            _playerTimer = new Timer(x =>
            {
                var playerState = remoteCalls.PlayerState;
                if (playerState != NeonPlayerState.Stopped)
                {
                    if (_showRemainingTime)
                    {
                        PlayerTimeTopLine = MiniPlayerHelpers.FormatTime(remoteCalls.CurrentPosition);
                    }
                    else
                    {
                        PlayerTimeTopLine =
                            $"-{MiniPlayerHelpers.FormatTime(remoteCalls.Duration - remoteCalls.CurrentPosition)}";
                    }
                    PlayerTimeBottomLine = MiniPlayerHelpers.FormatTime(remoteCalls.Duration);
                    if (_isFirstInvocation)
                    {
                        _isFirstInvocation = false;
                        UpdateSongInfo();
                    }
                }
                if (playerState == NeonPlayerState.Playing)
                {
                    PlayPauseImageSource = "Images/pause.png";
                }
                else 
                {
                    PlayPauseImageSource = "Images/play.png";
                }
            }, null, 0, 1000);
            PlayerImage = string.Empty;
            PlayPauseImageSource = "Images/play.png";
            PlayerInformationTopLine = "No track playing";
            PlayerTimeTopLine = "-";
            ShowRemainingTime = true;
            IsPlaying = false;
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        private bool _showRemainingTime;
        public bool ShowRemainingTime
        {
            get { return _showRemainingTime; }
            set { _showRemainingTime = value; OnPropertyChanged(); }
        }
        private string _playerImage;
        public string PlayerImage
        {
            get { return _playerImage; }
            set { _playerImage = value; OnPropertyChanged(); }
        }

        private string _playerTimeTopLine;
        public string PlayerTimeTopLine
        {
            get { return _playerTimeTopLine; }
            set { _playerTimeTopLine = value; OnPropertyChanged(); }
        }

        private string _playerTimeBottomLine;
        public string PlayerTimeBottomLine
        {
            get { return _playerTimeBottomLine; }
            set { _playerTimeBottomLine = value;OnPropertyChanged(); }
        }

        private string _playerInformationTopLine;
        public string PlayerInformationTopLine
        {
            get { return _playerInformationTopLine; }
            set { _playerInformationTopLine = value; OnPropertyChanged(); }
        }
        private string _playerInformationBottomLine;
        public string PlayerInformationBottomLine
        {
            get { return _playerInformationBottomLine; }
            set { _playerInformationBottomLine = value; OnPropertyChanged(); }
        }

        private string _playPauseImageSource;

        public string PlayPauseImageSource
        {
            get { return _playPauseImageSource; }
            set { _playPauseImageSource = value; OnPropertyChanged(); }
        }

        private bool _isFavourite;
        public bool IsFavourite
        {
            get { return _isFavourite; }
            set
            {
                _isFavourite = value;
                OnPropertyChanged();
                FavouriteImage = _isFavourite ? "Images/isfavr.png" : "Images/isnfav.png";
            }
        }

        private string _favouriteImage;
        public string FavouriteImage
        {
            get { return _favouriteImage; }
            set { _favouriteImage = value; OnPropertyChanged(); }
        }

        public void UpdateSongInfo()
        {
            var sb = new StringBuilder();
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            var track = remoteCalls.ActiveTrack;
            if (track != null)
            {
                sb.Append(string.IsNullOrEmpty(track.Artist) ? "?" : track.Artist);
                sb.Append(" - ");
                sb.Append(string.IsNullOrEmpty(track.Title) ? "?" : track.Title);

                if (!string.IsNullOrEmpty(track.Subtitle))
                {
                    sb.AppendFormat(" [{0}]", track.Subtitle);
                }
                if (!string.IsNullOrEmpty(track.Remix))
                {
                    sb.AppendFormat(" ({0})", track.Remix);
                }
                PlayerInformationTopLine = sb.ToString();
                sb.Clear();
                sb.Append(string.IsNullOrEmpty(track.Album) ? "?" : track.Album);
                if (track.ReleaseYear > 0)
                {
                    sb.AppendFormat(" - ({0})", track.ReleaseYear);
                }
                PlayerInformationBottomLine = sb.ToString();

                var album = remoteCalls.AlbumById(track.AlbumId);
                if (album != null)
                {
                    PlayerImage = album.AlbumPicturePath;
                }
                BlockRatingChanges = true;
                IsFavourite = track.IsFavourite;
                BlockRatingChanges = false;
            }
            if (remoteCalls.PlayerState == NeonPlayerState.Stopped || remoteCalls.PlayerState == NeonPlayerState.Undefined)
            {
                PlayerInformationTopLine = "No track playing";
                PlayerInformationBottomLine = string.Empty;
                PlayerTimeTopLine = "-";
                PlayerTimeBottomLine = string.Empty;
                IsPlaying = false;
            }
            else
            {
                IsPlaying = true;
            }
            UpdateUiAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
