using Zenject;
using UnityEngine;
using _Project.MiniGame.Core;
using _Project.MiniGame.Gameplay;

namespace _Project.MiniGame.Installers
{
    public sealed class MiniGameInstaller : MonoInstaller
    {
        [SerializeField] private TicTacToeModuleLauncher _miniGameLauncher;

        public override void InstallBindings()
        {
            Container.Bind<IMiniGameLauncher>().FromInstance(_miniGameLauncher).AsSingle();
            Container.Bind<RewardService>().AsSingle();
            Container.Bind<TicTacToeBotMoveSelector>().AsSingle();
            Container.Bind<TicTacToeMatchFactory>().AsSingle();
        }
    }
}