using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Windows;
using ProjectEye.Models.Enums;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 音效Service
    /// 处理休息结束提示音的加载和播放
    /// </summary>
    public class SoundService : IService
    {
        private readonly Dictionary<SoundType, SoundPlayer> players = [];

        private void Player_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            (sender as SoundPlayer).Dispose();
        }

        public void Init()
        {
            players.Add(SoundType.RestOverSound, new SoundPlayer());
            players.Add(SoundType.Other, new SoundPlayer());

            players[SoundType.RestOverSound].LoadCompleted += Player_LoadCompleted;
            players[SoundType.Other].LoadCompleted += Player_LoadCompleted;

            LoadConfigSound();
        }

        /// <summary>
        /// 加载用户配置的音效
        /// </summary>
        private void LoadConfigSound()
        {
            //加载休息结束提示音
            LoadSound(SoundType.RestOverSound);
        }
        #region 播放音效
        /// <summary>
        /// 播放音效,默认休息结束音效
        /// </summary>
        public bool Play(SoundType soundType = SoundType.RestOverSound)
        {
            var player = players[soundType];
            if (player.IsLoadCompleted)
            {
                try
                {
                    player.Play();
                    return true;
                }
                catch (Exception ec)
                {
                    //播放声音失败，可能是加载了不支持或损坏的文件
                    LogHelper.Warning(ec.ToString());
                    //切换到默认音效
                    LoadSound();
                }
            }
            return false;
        }

        #endregion

        #region 加载指定音效文件
        /// <summary>
        /// 从路径加载音效
        /// </summary>
        /// <param name="file">路径</param>
        /// <param name="resource">指示是否是系统资源</param>
        /// <returns></returns>
        public bool Load(SoundType soundType, string file, bool resource = true)
        {
            try
            {
                var player = players[soundType];
                if (resource)
                {
                    var soundUri = new Uri(file, UriKind.RelativeOrAbsolute);
                    var info = Application.GetResourceStream(soundUri);
                    player.Stream = info.Stream;
                }
                else
                {
                    player.SoundLocation = file;
                }
                player.LoadAsync();

                return true;
            }
            catch (Exception ec)
            {
                LogHelper.Warning($"Failed to load music: {ec}");
                return false;
            }
        }
        /// <summary>
        /// 加载指定音效文件
        /// </summary>
        /// <param name="path">音效文件路径，为空时加载默认音效</param>
        public void LoadSound(SoundType soundType = SoundType.RestOverSound)
        {
            var isDefault = true;
            var path = "/ProjectEye;component/Resources/relentless.wav";
            var loadResult = Load(soundType, path, isDefault);
            //加载音效失败
            if (!loadResult && !isDefault)
            {
                //加载自定义音效失败
                //尝试加载默认音效
                LoadSound();
            }
        }
        #endregion

        #region 测试外部音效
        /// <summary>
        /// 测试外部音效
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Test(string file)
        {

            if (Load(SoundType.Other, file, false))
            {
                if (Play(SoundType.Other))
                {
                    return true;
                }
            }
            return false;

        }
        #endregion
    }
}
