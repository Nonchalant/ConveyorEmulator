using UnityEngine;
using Zenject;

namespace Game
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private GameObject MinePrefab;

        public override void InstallBindings()
        {
            Container
                .Bind<IViewModel>()
                .To<ViewModel>()
                .AsSingle();
        
            Container
                .Bind<IForwardManipulator>()
                .To<ForwardManipulator>()
                .AsTransient();

            Container
                .BindFactory<Model.Mine, Model.MineFactory>()
                .FromComponentInNewPrefab(MinePrefab);
        }
    }
}