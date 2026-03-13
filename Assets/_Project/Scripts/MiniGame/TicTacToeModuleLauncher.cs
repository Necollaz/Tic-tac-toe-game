using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UniRx;
using Zenject;
using _Project.MiniGame.Core;
using _Project.MiniGame.Gameplay;
using _Project.MiniGame.UI;

namespace _Project.MiniGame
{
    public sealed class TicTacToeModuleLauncher : MonoBehaviour, IMiniGameLauncher
    {
        [SerializeField] private AssetReferenceGameObject _gamePanelAssetReference;

        private readonly Subject<Unit> _miniGameClosedSubject = new Subject<Unit>();

        private TicTacToeMatchFactory _matchFactory;
        private TicTacToeGamePanel _activeGamePanel;
        private Transform _activeParentTransform;
        private AsyncSubject<MiniGameResult> _activeRunSubject;
        
        private AsyncOperationHandle<GameObject> _loadHandle;
        
        private bool _isLoadHandleValid;

        [Inject]
        private void Construct(TicTacToeMatchFactory matchFactory)
        {
            _matchFactory = matchFactory;
        }

        public IObservable<Unit> OnMiniGameClosed => _miniGameClosedSubject;

        public IObservable<MiniGameResult> Run(Transform parentTransform)
        {
            if (_activeGamePanel != null || _activeRunSubject != null)
            {
                return Observable.Throw<MiniGameResult>(
                    new InvalidOperationException("Mini game is already running."));
            }

            AsyncSubject<MiniGameResult> runSubject = new AsyncSubject<MiniGameResult>();

            _activeRunSubject = runSubject;
            _activeParentTransform = parentTransform;

            LoadAndCreatePanel(runSubject, parentTransform);

            return runSubject;
        }

        private void LoadAndCreatePanel(AsyncSubject<MiniGameResult> runSubject, Transform parentTransform)
        {
            if (!_gamePanelAssetReference.RuntimeKeyIsValid())
            {
                runSubject.OnError(new InvalidOperationException("Mini game panel AssetReference is not valid."));
                
                ResetRunState();
                
                return;
            }

            _loadHandle = _gamePanelAssetReference.LoadAssetAsync<GameObject>();
            _isLoadHandleValid = true;
            _loadHandle.Completed += handle => OnGamePanelLoaded(handle, parentTransform, runSubject);
        }

        private void OnGamePanelLoaded(
            AsyncOperationHandle<GameObject> loadHandle,
            Transform parentTransform,
            AsyncSubject<MiniGameResult> runSubject)
        {
            if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
            {
                ReleaseLoadedAsset();
                
                runSubject.OnError(new InvalidOperationException("Failed to load mini game panel prefab."));
                
                ResetRunState();
                
                return;
            }

            GameObject gamePanelPrefab = loadHandle.Result;
            GameObject gamePanelInstance = Instantiate(gamePanelPrefab, parentTransform, false);
            TicTacToeGamePanel gamePanel = gamePanelInstance.GetComponent<TicTacToeGamePanel>();

            if (gamePanel == null)
            {
                Destroy(gamePanelInstance);
                ReleaseLoadedAsset();
                runSubject.OnError(
                    new InvalidOperationException("TicTacToeGamePanel component was not found on loaded prefab."));
                ResetRunState();
                
                return;
            }

            _activeGamePanel = gamePanel;

            TicTacToeMatchController matchController = _matchFactory.Create();
            gamePanel.Initialize(matchController, CloseMiniGamePanel, RestartMiniGamePanel);

            matchController.OnMatchFinished
                .Subscribe(
                    result =>
                    {
                        if (_activeRunSubject == null)
                            return;

                        _activeRunSubject.OnNext(result);
                        _activeRunSubject.OnCompleted();
                    },
                    exception =>
                    {
                        if (_activeRunSubject == null)
                            return;

                        _activeRunSubject.OnError(exception);
                        
                        ResetRunState();
                    });
        }

        private void RestartMiniGamePanel()
        {
            if (_activeGamePanel == null || _activeRunSubject == null || _activeParentTransform == null)
                return;

            Destroy(_activeGamePanel.gameObject);
            
            _activeGamePanel = null;

            ReleaseLoadedAsset();
            LoadAndCreatePanel(_activeRunSubject, _activeParentTransform);
        }

        private void CloseMiniGamePanel()
        {
            if (_activeGamePanel == null)
                return;

            Destroy(_activeGamePanel.gameObject);
            
            _activeGamePanel = null;

            ReleaseLoadedAsset();
            ResetRunState();
            
            _miniGameClosedSubject.OnNext(Unit.Default);
        }

        private void ReleaseLoadedAsset()
        {
            if (!_isLoadHandleValid)
                return;

            _gamePanelAssetReference.ReleaseAsset();
            
            _loadHandle = default;
            _isLoadHandleValid = false;
        }

        private void ResetRunState()
        {
            _activeRunSubject = null;
            _activeParentTransform = null;
        }
    }
}