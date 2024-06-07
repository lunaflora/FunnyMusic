using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Machine;
using YooAsset;

namespace Core
{
    internal class FsmCreatePackageDownloader : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }
        void IStateNode.OnEnter()
        {
            PatchEventDefine.PatchStatesChange.SendEventMessage("创建补丁下载器！");
            CreateDownloader().Forget();
            
        }
        void IStateNode.OnUpdate()
        {
            
        }
        void IStateNode.OnExit()
        {
        }

        private async UniTask CreateDownloader()
        {
            await UniTask.Delay(500);
            
            var packageName = (string)_machine.GetBlackboardValue("PackageName");
            var package = YooAssets.GetPackage(packageName);
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            _machine.SetBlackboardValue("Downloader", downloader);

            if (downloader.TotalDownloadCount == 0)
            {
                FDebug.Print("Not found any download files !");
                _machine.ChangeState<FsmUpdaterDone>();
            }
            else
            {
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);
                  
                _machine.ChangeState<FsmDownloadPackageFiles>();
            }
        }
    }
}