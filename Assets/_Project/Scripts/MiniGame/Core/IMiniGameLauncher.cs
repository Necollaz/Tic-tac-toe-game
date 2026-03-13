using System;
using UniRx;
using UnityEngine;

namespace _Project.MiniGame.Core
{
    public interface IMiniGameLauncher
    {
        public IObservable<MiniGameResult> Run(Transform parentTransform);
        public IObservable<Unit> OnMiniGameClosed { get; }
    }
}