using System;
using NPSMLib;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// Service for controlling media playback (pause/resume) during rest periods
    /// </summary>
    public class MediaControlService : IService
    {
        private readonly object _lock = new();
        
        private NowPlayingSessionManager _sessionManager;
        private MediaPlaybackDataSource _mediaPlaybackDataSource;
        private bool _wasPlayingBeforePause;

        public MediaControlService()
        {
            _wasPlayingBeforePause = false;
        }

        public void Init()
        {
            try
            {
                _sessionManager = new NowPlayingSessionManager();
                LogHelper.Info("MediaControlService initialized");
            }
            catch (Exception ex)
            {
                LogHelper.Error("Failed to initialize MediaControlService: " + ex.Message);
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                DisposeResource(_mediaPlaybackDataSource, "MediaPlaybackDataSource");
                _mediaPlaybackDataSource = null;

                DisposeResource(_sessionManager, "NowPlayingSessionManager");
                _sessionManager = null;
            }
        }

        /// <summary>
        /// Helper method to safely dispose resources
        /// </summary>
        private void DisposeResource(object resource, string resourceName)
        {
            if (resource is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"Error disposing {resourceName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Pause currently playing media
        /// </summary>
        public void PauseMedia()
        {
            lock (_lock)
            {
                try
                {
                    if (_sessionManager?.CurrentSession is null)
                    {
                        _wasPlayingBeforePause = false;
                        return;
                    }

                    // Dispose old media playback data source if it exists
                    if (_mediaPlaybackDataSource is not null)
                    {
                        DisposeResource(_mediaPlaybackDataSource, "MediaPlaybackDataSource");
                    }

                    _mediaPlaybackDataSource = _sessionManager.CurrentSession.ActivateMediaPlaybackDataSource();
                    
                    if (_mediaPlaybackDataSource is null)
                    {
                        _wasPlayingBeforePause = false;
                        return;
                    }

                    var playbackInfo = _mediaPlaybackDataSource.GetMediaPlaybackInfo();
                    
                    // Only pause if media is currently playing
                    if (playbackInfo.PlaybackState == MediaPlaybackState.Playing)
                    {
                        _wasPlayingBeforePause = true;
                        _mediaPlaybackDataSource.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
                        LogHelper.Info("Media paused for rest period");
                    }
                    else
                    {
                        _wasPlayingBeforePause = false;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error while trying to pause media: " + ex.Message);
                    _wasPlayingBeforePause = false;
                }
            }
        }

        /// <summary>
        /// Resume media if it was playing before pause
        /// </summary>
        public void ResumeMedia()
        {
            lock (_lock)
            {
                try
                {
                    // Only resume if media was playing before we paused it
                    if (!_wasPlayingBeforePause)
                    {
                        return;
                    }

                    if (_mediaPlaybackDataSource is null)
                    {
                        _wasPlayingBeforePause = false;
                        return;
                    }

                    _mediaPlaybackDataSource.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);
                    LogHelper.Info("Media resumed after rest period");
                    _wasPlayingBeforePause = false;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Error while trying to resume media: " + ex.Message);
                    _wasPlayingBeforePause = false;
                }
            }
        }
    }
}
