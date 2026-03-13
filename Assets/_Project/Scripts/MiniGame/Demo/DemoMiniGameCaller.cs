using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using _Project.MiniGame.Core;

namespace _Project.Demo
{
    public sealed class DemoMiniGameCaller : MonoBehaviour
    {
        [SerializeField] private Button _playMiniGameButton;
        [SerializeField] private Transform _miniGameParentTransform;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private IMiniGameLauncher _miniGameLauncher;

        [Inject]
        private void Construct(IMiniGameLauncher miniGameLauncher)
        {
            _miniGameLauncher = miniGameLauncher;
        }

        private void Awake()
        {
            _playMiniGameButton
                .OnClickAsObservable()
                .Subscribe(_ => RunMiniGame())
                .AddTo(_disposables);

            _miniGameLauncher.OnMiniGameClosed.Subscribe(_ => ShowPlayMiniGameButton()).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void RunMiniGame()
        {
            HidePlayMiniGameButton();

            _miniGameLauncher
                .Run(_miniGameParentTransform)
                .Subscribe(_ => { }, OnMiniGameLaunchFailed)
                .AddTo(_disposables);
        }

        private void OnMiniGameLaunchFailed(Exception exception)
        {
            ShowPlayMiniGameButton();
        }

        private void HidePlayMiniGameButton()
        {
            _playMiniGameButton.gameObject.SetActive(false);
        }

        private void ShowPlayMiniGameButton()
        {
            _playMiniGameButton.gameObject.SetActive(true);
        }
    }
}