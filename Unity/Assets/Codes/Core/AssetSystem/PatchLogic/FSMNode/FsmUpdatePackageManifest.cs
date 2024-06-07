using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Machine;
using YooAsset;

namespace Core
{
    /// <summary>
    /// 更新资源清单
    /// </summary>
    internal class FsmUpdatePackageManifest : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }
        void IStateNode.OnEnter()
        {
            PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
            UpdateManifest().Forget();
        }
        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }

        private async UniTask UpdateManifest()
        {
            await UniTask.Delay(500);
            var packageName = (string)_machine.GetBlackboardValue("PackageName");
            var packageVersion = (string)_machine.GetBlackboardValue("PackageVersion");
            var package = YooAssets.GetPackage(packageName);
            bool savePackageVersion = true;
            var operation = package.UpdatePackageManifestAsync(packageVersion, savePackageVersion);
            await operation.ToUniTask();
            
            if (operation.Status != EOperationStatus.Succeed)
            {
                FDebug.Warn(operation.Error);
                PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
              
            }
            else
            {
                _machine.ChangeState<FsmCreatePackageDownloader>();
            }


        }
    }
}