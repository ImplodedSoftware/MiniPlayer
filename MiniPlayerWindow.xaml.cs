﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NeonScripting.Models;

namespace MiniPlayer
{
    /// <summary>
    /// Interaction logic for MiniPlayerWindow.xaml
    /// </summary>
    public partial class MiniPlayerWindow : Window
    {
        private const string MpLeftPos = "MINIPLAYER_LEFT";
        private const string MpTopPos = "MINIPLAYER_TOP";
        private const string MpWidth = "MINIPLAYER_WIDTH";
        private const string MpHeight = "MINIPLAYER_HEIGHT";

        private bool _blockCalls;
        private bool _alwaysOnTop = true;
        private readonly MiniPlayerViewModel _vm = new MiniPlayerViewModel();
        public MiniPlayerWindow()
        {
            InitializeComponent();
            MouseDown += Window_MouseDown;
            _vm.UpdateUiAction += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    _vm.PreviousCommand.NotifyCanExecuteChanged();
                    _vm.NextCommand.NotifyCanExecuteChanged();
                });
            };
            DataContext = _vm;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        private void PlayerTimeBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _vm.ShowRemainingTime = !_vm.ShowRemainingTime;
        }

        public void OnEvent(NeonScriptEventTypes eventType)
        {
            switch (eventType)
            {
                case NeonScriptEventTypes.ActiveTrackChanged:
                    _vm.UpdateSongInfo();
                    break;
                case NeonScriptEventTypes.MusicStopped:
                    _vm.UpdateSongInfo();
                    break;
            }
        }


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _alwaysOnTop = !_alwaysOnTop;
            Topmost = _alwaysOnTop;
        }

        private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            PluginHostHandler.Instance.PluginCloseAction?.Invoke(PluginHostHandler.Instance.ThisPlugin);
            Close();
        }

        private void MiniPlayerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            var leftPos = remoteCalls.GetIntValue(MpLeftPos);
            if (leftPos == -1)
            {
                return;
            }
            var topPos = remoteCalls.GetIntValue(MpTopPos);
            var width = remoteCalls.GetIntValue(MpWidth);
            var height = remoteCalls.GetIntValue(MpHeight);
            Left = leftPos;
            Top = topPos;
            Width = width;
            Height = height;
        }

        private void MiniPlayerWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            remoteCalls.SetIntValue(MpLeftPos, (int)Left);
            remoteCalls.SetIntValue(MpTopPos, (int)Top);
            remoteCalls.SetIntValue(MpWidth, (int)Width);
            remoteCalls.SetIntValue(MpHeight, (int)Height);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_blockCalls) return;
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            remoteCalls.SetPlayerVolume((int)e.NewValue);
        }

        private void Rating_OnClick(object sender, RoutedEventArgs e)
        {
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            var tag = (string)(sender as MenuItem).Tag;
            if (int.TryParse(tag, out var rating))
            {
                remoteCalls.RateCurrentTrack(rating);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var remoteCalls = PluginHostHandler.Instance.ScriptHost.RemoteCalls;
            _blockCalls = true;
            VolSlider.Value = remoteCalls.PlayerVolume;
            var track = remoteCalls.ActiveTrack;
            var rating = track != null ? remoteCalls.DownsizeRating(track.Rating) : 0;
            var ratingString = rating.ToString();
            foreach(var item in CtxMenu.Items)
            {
                var menuItem = (item as MenuItem);
                if (menuItem != null && menuItem.IsCheckable)
                {
                    if (menuItem.Tag.Equals(ratingString))
                    {
                        menuItem.IsChecked = true;
                    }
                    else
                    {
                        menuItem.IsChecked = false;
                    }
                }
            }

            _blockCalls = false;
        }
    }
}
