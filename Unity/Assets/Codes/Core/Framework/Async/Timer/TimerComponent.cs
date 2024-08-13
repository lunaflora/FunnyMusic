using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 通过Timer中的ServerNow可以模拟时间暂停效果
    /// 在ServerNow中用当前时间减去游戏总计的暂停时间
    /// </summary>
    
    public enum TimerType
    {
        None,
        OnceTimer,  //一段时间后执行一次的计时器，回调写法
        OnceWaitTimer, //一段时间后执行一次的计时器，连续写法
        RepeatTimer, //重读执行的计时器
        
    }
    
    [ObjectSystem]
    public class TimerActionAwakeSystem: AwakeSystem<TimerAction, TimerType, long, int, object>
    {
        public override void Awake(TimerAction self, TimerType timerType, long time, int type, object obj)
        {
            self.TimerType = timerType;
            //这个是事件的自定义参数
            self.Object = obj;
            //注意，在OnceTimer中这里是触发时间，不是间隔时间
            self.Time = time;
            self.Type = type;
        }
    }
    
    [ObjectSystem]
    public class TimerActionDestroySystem: DestroySystem<TimerAction>
    {
        public override void Destroy(TimerAction self)
        {
            self.Object = null;
            self.Time = 0;
            self.TimerType = TimerType.None;
            self.Type = 0;
        }
    }

    
    /// <summary>
    /// 每个timer时间的行为数据，作为一个Child加入TimerComponent
    /// </summary>
    public class TimerAction: Entity, IAwake, IAwake<TimerType, long, int, object>, IDestroy
    {
        public TimerType TimerType;

        public object Object;

        public long Time;

        public int Type;
    }

    [FriendClass(typeof(TimerComponent))]
    [FriendClass(typeof(TimerAction))]
    public static class TimeComponentSystem
    {
        [ObjectSystem]
        public class TimeComponentAwakeSystem : AwakeSystem<TimerComponent>
        {
            public override void Awake(TimerComponent self)
            {
                TimerComponent.Instance = self;
                self.Init();
            }
        }
        
        /// <summary>
        /// 这个在重载的时候使用
        /// </summary>
        [ObjectSystem]
        public class TimeComponentLoadSystem : LoadSystem<TimerComponent>
        {
            public override void Load(TimerComponent self)
            {
                self.Init();
            }
        }
        [ObjectSystem]
        public class TimeComponenDestroySystem : DestroySystem<TimerComponent>
        {
            public override void Destroy(TimerComponent self)
            {
                TimerComponent.Instance = null;
                CustomLogger.Log(LoggerLevel.Log, $"<color=blue>### 框架逻辑 </color> 删除计时器ID {self.Id}");
            }
        }
        
        [ObjectSystem]
        public class TimeComponenUpdateSystem : UpdateSystem<TimerComponent>
        {
            public override void Update(TimerComponent self)
            {
                
                #region 每帧执行的timer，不用foreach TimeId，减少GC

                int count = self.everyFrameTimer.Count;
                for (int i = 0; i < count; ++i)
                {
                    long timerId = self.everyFrameTimer.Dequeue();
                    TimerAction timerAction = self.GetChild<TimerAction>(timerId);
                    if (timerAction == null)
                    {
                        continue;
                    }
                    self.Run(timerAction);
                }

                #endregion
                
                if (self.TimeId.Count == 0)
                {
                    return;
                }

                self.timeNow = TimeHelper.ServerNow();

                if (self.timeNow < self.minTime)
                {
                    return;
                }

                self.TimeId.ForEachFunc(self.foreachFunc);

                while (self.timeOutTime.Count > 0)
                {
                    long time = self.timeOutTime.Dequeue();
                    var list = self.TimeId[time];
                    for (int i = 0; i < list.Count; ++i)
                    {
                        long timerId = list[i];
                        self.timeOutTimerIds.Enqueue(timerId);
                    }

                    self.TimeId.Remove(time);
                }

                while (self.timeOutTimerIds.Count > 0)
                {
                    long timerId = self.timeOutTimerIds.Dequeue();

                    TimerAction timerAction = self.GetChild<TimerAction>(timerId);
                    if (timerAction == null)
                    {
                        continue;
                    }
                    self.Run(timerAction);
                }
               
            }
        }
        



        public static void Init(this TimerComponent self)
        {
            //加入已经到时的事件
            //由于是按时间先后顺序加入的
            //如果第一个还未到时间，就不再循环
            //并且更新最小时间
            self.foreachFunc = (k, v) =>
            {
                if (k > self.timeNow)
                {
                    self.minTime = k;
                    return false;
                }

                self.timeOutTime.Enqueue(k);
                return true;
            };
            self.timerActions = new ITimer[TimerComponent.TimeTypeMax];

            //这里建立一个Timer和TimerType的映射
            List<Type> types = GameWorld.WorldSystem.GetTypes(typeof (TimerAttribute));

            foreach (Type type in types)
            {
                ITimer iTimer = Activator.CreateInstance(type) as ITimer;
                if (iTimer == null)
                {
                    CustomLogger.Log(LoggerLevel.Error,$"Timer Action {type.Name} 需要继承 ITimer");
                    continue;
                }
                
                object[] attrs = type.GetCustomAttributes(typeof(TimerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                foreach (object attr in attrs)
                {
                    TimerAttribute timerAttribute = attr as TimerAttribute;
                    self.timerActions[timerAttribute.Type] = iTimer;
                }
            }
            

        }

        public static void Run(this TimerComponent self,TimerAction timerAction)
        {
            switch (timerAction.TimerType)
            {
                //这个是基于UniTask写法的Timer,不用处理
                case TimerType.OnceWaitTimer:
                   
                    break;
                case TimerType.OnceTimer:
                {
                    int type = timerAction.Type;
                    ITimer timer = self.timerActions[type];
                    if (timer == null)
                    {
                        CustomLogger.Log(LoggerLevel.Error,$"not found timer action: {type}");
                        return;
                    }
                    timer.Handle(timerAction.Object);
                    //删除掉已经触发的一次性计数器
                    self.Remove(timerAction.Id);  
                    break;
                }
                case TimerType.RepeatTimer:
                {
                    int type = timerAction.Type;
                    long tillTime = TimeHelper.ServerNow() + timerAction.Time;
                    self.AddTimer(tillTime, timerAction);

                    ITimer timer = self.timerActions[type];
                    if (timer == null)
                    {
                        CustomLogger.Log(LoggerLevel.Error, $"not found timer action: {type}");
                        return;
                    }

                    timer.Handle(timerAction.Object);
                    break;
                }
                default:
                    break;
                
            }
            
        } 
        
        private static void AddTimer(this TimerComponent self, long tillTime, TimerAction timer)
        {
            if (timer.TimerType == TimerType.RepeatTimer && timer.Time == 0)
            {
                self.everyFrameTimer.Enqueue(timer.Id);
                return;
            }
            self.TimeId.Add(tillTime, timer.Id);
            if (tillTime < self.minTime)
            {
                self.minTime = tillTime;
            }
        }

        public static bool Remove(this TimerComponent self, ref long id)
        {
            long i = id;
            id = 0;
            return self.Remove(i);
        }
        
        
        private static bool Remove(this TimerComponent self, long id)
        {
            if (id == 0)
            {
                return false;
            }

            TimerAction timerAction = self.GetChild<TimerAction>(id);
            if (timerAction == null)
            {
                return false;
            }
            timerAction.Dispose();
            return true;
        }

        #region AddTimer

        /// <summary>
        /// time是毫秒
        /// 基于UniTask的连续写法
        /// </summary>
        /// <param name="self"></param>
        /// <param name="time"></param>
        /// <param name="cancellationToken"></param>
        public static async UniTask<bool> WaitAsync(this TimerComponent self, long time, CancellationTokenSource cancellationToken = null)
        {
            if (time == 0)
            {
                return true;
            }

            bool ret = true;
            try
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(time),false,PlayerLoopTiming.Update,
                    cancellationToken != null ? cancellationToken.Token : default(CancellationToken));

            }
            catch
            {
                if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                {
                    CustomLogger.Log(LoggerLevel.Log ,$"WaitAsync cancelled {time}");
                    return false;
                }

            }

            return ret;
        }
        
        // 用这个优点是可以热更，缺点是回调式的写法，逻辑不连贯。WaitTillAsync不能热更，优点是逻辑连贯。
        // wait时间短并且逻辑需要连贯的建议WaitTillAsync
        // wait时间长不需要逻辑连贯的建议用NewOnceTimer
        public static long NewOnceTimer(this TimerComponent self, long time, int type, object args)
        {
            long tillTime = TimeHelper.ServerNow() + time;
            if (tillTime < TimeHelper.ServerNow())
            {
                CustomLogger.Log(LoggerLevel.Warning,$"new once time too small: {tillTime}");
            }
            TimerAction timer = self.AddChild<TimerAction, TimerType, long, int, object>(TimerType.OnceTimer, tillTime, type, args, true);
            self.AddTimer(tillTime, timer);
            return timer.Id;
        }

        public static long NewFrameTimer(this TimerComponent self, int type, object args)
        {

            return self.NewRepeatedTimerInner(0, type, args);
        }

        #endregion
        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        private static long NewRepeatedTimerInner(this TimerComponent self, long time, int type, object args)
        {

            long tillTime = TimeHelper.ServerNow() + time;
            TimerAction timer = self.AddChild<TimerAction, TimerType, long, int, object>(TimerType.RepeatTimer, time, type, args, true);

            // 每帧执行的不用加到timerId中，防止遍历
            self.AddTimer(tillTime, timer);
            return timer.Id;
        }

        public static long NewRepeatedTimer(this TimerComponent self, long time, int type, object args)
        {
            if (time < 100)
            {
                CustomLogger.Log(LoggerLevel.Error,$"time too small: {time}");
                return 0;
            }
            return self.NewRepeatedTimerInner(time, type, args);
        }
        

        
    }


    [ComponentOf(typeof(World))]
    public class TimerComponent : Entity, IAwake,IUpdate,ILoad,IDestroy
    {
        public static TimerComponent Instance
        {
            get;

            set;
        }

        public long timeNow;
        public Func<long, List<long>, bool> foreachFunc;
        
        /// <summary>
        /// key: time, value: timer id
        /// 在对应的system里面进行逻辑处理
        /// </summary>
        public readonly MultiMap<long, long> TimeId = new MultiMap<long, long>();

        /// <summary>
        /// 到时应该Excute的时间队列
        /// </summary>
        public readonly Queue<long> timeOutTime = new Queue<long>();

        public readonly Queue<long> timeOutTimerIds = new Queue<long>();
        /// <summary>
        /// 用于重复性定时器队列
        /// </summary>
        public readonly Queue<long> everyFrameTimer = new Queue<long>();

        // 记录最小时间，不用每次都去MultiMap取第一个值
        public long minTime;

        public const int TimeTypeMax = 10000;

        public ITimer[] timerActions;

    }
}