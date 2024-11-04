
using UnityEngine;
using Zenject;

namespace Application
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IRouter>()
                .To<Router>()
                .AsSingle();
            
            Container
                .Bind<IGameManager>()
                .To<GameManager>()
                .AsSingle();
            
            Container
                .Bind<Helper.IPlayerPrefsManager>()
                .To<Helper.PlayerPrefsManager>()
                .AsSingle();
            
            Container
                .Bind<Helper.IResourceLoader>()
                .To<Helper.ResourceLoader>()
                .AsSingle();
        }
    }
}