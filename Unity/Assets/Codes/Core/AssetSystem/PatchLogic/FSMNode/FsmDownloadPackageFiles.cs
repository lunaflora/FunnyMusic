using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Machine;
using YooAsset;

namespace Core
{
    /// <summary>
    /// 下载更新文件
    /// </summary>
    internal class FsmDownloadPackageFiles : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }
        void IStateNode.OnEnter()
        {
            FDebug.Print("开始下载补丁文件！");
            BeginDownload().Forget();
        }
        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }

        private async UniTask BeginDownload()
        {
            var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
            downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
            downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
            downloader.BeginDownload();
            await downloader.ToUniTask();

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
            {
                
            }
            

            _machine.ChangeState<FsmDownloadPackageOver>();
        }
    }
}