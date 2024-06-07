using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Machine;
using YooAsset;

namespace Core
{
    /// <summary>
    /// 更新资源版本号
    /// </summary>
    internal class FsmUpdatePackageVersion : IStateNode
    {
        private StateMachine _machine;
        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }
        void IStateNode.OnEnter()
        {
            FDebug.Print("获取最新的资源版本 !");
            UpdatePackageVersion().Forget();
        }
        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }

        private async UniTask UpdatePackageVersion()
        {
            await UniTask.Delay(500);
            
            var packageName = (string)_machine.GetBlackboardValue("PackageName");
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageVersionAsync();
            await operation.ToUniTask();
            
            if (operation.Status != EOperationStatus.Succeed)
            {
                FDebug.Warn(operation.Error);
                PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
            }
            else
            {
                _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
                _machine.ChangeState<FsmUpdatePackageManifest>();
            }
        }
    }
}