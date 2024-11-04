using UnityEngine;
using Zenject;

namespace Title
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IViewModel>()
                .To<ViewModel>()
                .AsSingle();
        }
    }
}