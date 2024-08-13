using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.LowLevel;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Framework
{
    public class DemoAsyncClass
    {

        public async UniTask<string> DemoAsync()
        {
            // yield return new WaitForSeconds/WaitForSecondsRealtime 的替代方案
            var task1 = UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false);
            var task2= UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            var task3 = UniTask.Delay(TimeSpan.FromSeconds(2), ignoreTimeScale: false);
            List<UniTask> taskList = new List<UniTask>();
            taskList.Add(task1);
            taskList.Add(task2);
            taskList.Add(task3);
            /*
            // 你可以等待一个Unity异步对象
            var asset = await Resources.LoadAsync<TextAsset>("foo");
            var txt = (await UnityWebRequest.Get("https://...").SendWebRequest()).downloadHandler.text;
            await SceneManager.LoadSceneAsync("scene2");
            */

            await UniTask.Yield();

            // 构造一个async-wait，并通过元组语义轻松获取所有结果
            await UniTask.WhenAny(taskList);
            // await UniTask.WhenAll(taskList);
            
            /*
            // 多线程示例，在此行代码后的内容都运行在一个线程池上
            await UniTask.SwitchToThreadPool();

            // 工作在线程池上的代码 //

            // 转回主线程
            await UniTask.SwitchToMainThread();
            */
            
            
            /*
            // 获取异步的 webrequest
            async UniTask<string> GetTextAsync(UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.text;
            }

            var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
            var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
            var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));
            */

            return "DemoAsyncClass";
        }
        
        /// <summary>
        /// UniTask.ToCoroutine将 async/await 桥接到协程
        /// </summary>
        /// <returns></returns>
        public IEnumerator DelayIgnore() => UniTask.ToCoroutine(async () =>
        {
            var time = Time.realtimeSinceStartup;

            Time.timeScale = 0.5f;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(3), ignoreTimeScale: true);

                var elapsed = Time.realtimeSinceStartup - time;
                Assert.AreEqual(3, (int)Math.Round(TimeSpan.FromSeconds(elapsed).TotalSeconds, MidpointRounding.ToEven));
            }
            finally
            {
                Time.timeScale = 1.0f;
            }
        });


        public Button cancelButton;
        /// <summary>
        /// 取消task
        /// </summary>

        public async void CancelDemo()
        {
            var cts = new CancellationTokenSource();
            

            cancelButton.onClick.AddListener(() =>
            {
                cts.Cancel();
            });

            await UnityWebRequest.Get("http://google.co.jp").SendWebRequest().WithCancellation(cts.Token);

            await UniTask.DelayFrame(1000, cancellationToken: cts.Token);
            
            
        }
        
        public static async UniTask MoveTo(Transform obj, Transform startObj, Transform stopObj, float timeToMove)
        {        
            //【1】变量定义
            float startPoint;       //开始运动时间点
            float elapsedTime;      //运行耗用的时间
            bool isFlying = false;  //当为true的时候，Update里面每帧插值计算位置，设置物体的位置

            //【2】移动前
            startPoint = Time.realtimeSinceStartup;
            obj.position = startObj.position;   //初始方位
            obj.rotation = startObj.rotation;        
    
            //【3】移动中
            // Update的内容：Unity 2020.2, C# 8.0 
            Func<UniTask> UpdateLoop = async () =>
            {
                //绑定到Update中去执行
                await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate()) 
                {
                    if (!isFlying) break;

                    //用时间进度来插值计算中间的方位
                    elapsedTime = Time.realtimeSinceStartup - startPoint;
                    //Debug.Log($" {Time.realtimeSinceStartup}  {elapsedTime}/{timeToMove} = {elapsedTime / timeToMove}");
                    obj.transform.position = Vector3.Lerp(startObj.position, stopObj.position, elapsedTime / timeToMove);
                    obj.transform.rotation = Quaternion.Lerp(startObj.rotation, stopObj.rotation, elapsedTime / timeToMove);              
                }
                return;
            };
            isFlying = true;
            await  UpdateLoop();                                           //分叉并行执行：移动物体      

            //【4】移动后
            await UniTask.Delay(TimeSpan.FromSeconds(timeToMove));  //等待给定的时间完成
            isFlying = false;                                       //停止updateLoop里面的循环
            await UniTask.DelayFrame(1);                            //等待一帧，用于渲染
            obj.position = stopObj.position;                        //结束方位
            obj.rotation = stopObj.rotation;
        }

        private void InjectFunction()
        {
           
        }

        private async UniTask<string> Timeout(string url,float timeout)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            var (cancleorfailed, result) = 
                await UnityWebRequest.Get(url).SendWebRequest().WithCancellation(cts.Token).SuppressCancellationThrow();

            if (!cancleorfailed)
            {
                return result.downloadHandler.text;
            }

            return "http time out!";


        }
        

    }

    public class DemoAsync : MonoBehaviour
    {
        private void Start()
        {
                        
            // 这个CancellationTokenSource和this GameObject生命周期相同，当this GameObject Destroy的时候，就会执行Cancel
            //await UniTask.DelayFrame(1000, cancellationToken: this.GetCancellationTokenOnDestroy());

            //string str = await UniTaskCallBack();
            
           // Debug.Log(str);

        }

        //一些Unity的异步操作具有ToUniTask(IProgress<float> progress = null, ...)扩展方法。

        public ProgressBar progressBar;
        private async void Profgress()
        {
            //var progress = Progress.Create<float>(x => Debug.Log(x));
            var progress = Progress.Create<float>(f =>
            {

                progressBar.value = f * 100;


            });

            var request = await UnityWebRequest.Get("http://google.co.jp")
                .SendWebRequest()
                .ToUniTask(progress: progress);
        }


        /// <summary>
        /// 基于UniTask的CallBack
        /// </summary>
        /// <returns></returns>
        private async UniTask<string> UniTaskCallBack()
        {
            var utcs = new UniTaskCompletionSource<string>();

            /*
             * 这里放一些异步处理，比如加载AssetBundle
             * 完成之后 utcs.TrySetResult()
             */
            


            return await utcs.Task;
        }

        private async UniTask DelayCallBack(UniTaskCompletionSource<string> utcs)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5),ignoreTimeScale:false);
            utcs.TrySetResult("UniTaskCallBack");
        }

        /*
            private async void DoAnimation()
            {
                
            
                // 动画序列
                await transform.DOMoveX(2, 10);
                await transform.DOMoveZ(5, 20);
    
                // 并行，并传递cancellation用于取消
                var ct = this.GetCancellationTokenOnDestroy();
    
                await UniTask.WhenAll(
                    transform.DOMoveX(10, 3).WithCancellation(ct),
                    transform.DOScale(10, 3).WithCancellation(ct));
                
                
            }
        */
    }
}