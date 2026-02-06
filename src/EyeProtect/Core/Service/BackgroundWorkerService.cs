using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EyeProtect.Core.Service
{
    /// <summary>
    /// 管理整个程序的后台工作任务
    /// </summary>
    public class BackgroundWorkerService : IService
    {
        /// <summary>
        /// 获取当前是否正在执行后台任务
        /// </summary>
        public bool IsBusy => backgroundWorker != null && backgroundWorker.IsBusy;
        public delegate void EventHandler();
        public event EventHandler OnCompleted, DoWork;

        private readonly BackgroundWorker backgroundWorker;
        private readonly List<Action> actions;

        public BackgroundWorkerService()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            actions = new List<Action>();
        }
        public void Init()
        {
            DoWork?.Invoke();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCompleted?.Invoke();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (var i = 0; i < actions.Count; i++)
            {
                actions[i].Invoke();
                actions.Remove(actions[i]);
            }
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

        public void Run()
        {
            if (!backgroundWorker.IsBusy)
            {
                DoWork?.Invoke();
                backgroundWorker.RunWorkerAsync();
            }
        }

        public void Dispose()
        {
            backgroundWorker.Dispose();
        }
    }
}
