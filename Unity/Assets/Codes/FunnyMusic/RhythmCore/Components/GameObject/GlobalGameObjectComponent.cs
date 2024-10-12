using Framework;
using UnityEngine;

namespace FunnyMusic
{
    public class GlobalGameObjectComponent : Entity ,IAwake,IDestroy
    {
        public static GlobalGameObjectComponent Instance;
        
        /// <summary>
        /// 各类UI的root
        /// </summary>
        public Transform UIRoot{ get; set; }
        public Transform NormalRoot{ get; set; }
        public Transform PopUpRoot{ get; set; }
        public Transform FixedRoot{ get; set; }
        public Transform OtherRoot{ get; set; }
        
        /// <summary>
        /// 核心游戏逻辑Root
        /// </summary>
        public Transform Controller { get; set; }
        
    }
    
    [ObjectSystem]
    public class GlobalGameObjectComponentAwakeSystem : AwakeSystem<GlobalGameObjectComponent>
    {
        public override void Awake(GlobalGameObjectComponent self)
        {
            GlobalGameObjectComponent.Instance = self;
            
            if (self.Controller == null)
            {
                self.Controller = GameObject.Find("Contorller").transform;
            }

            if (self.UIRoot == null)
            {
                self.UIRoot = GameObjectUtil.CreateTransform("UIRoot", self.Controller);
            }
            if (self.NormalRoot == null)
            {
                self.NormalRoot = GameObjectUtil.CreateTransform("NormalRoot", self.UIRoot);
            }
            if (self.FixedRoot == null)
            {
                self.FixedRoot = GameObjectUtil.CreateTransform("FixedRoot", self.UIRoot);
            }
            
            if (self.PopUpRoot == null)
            {
                self.PopUpRoot = GameObjectUtil.CreateTransform("PopUpRoot", self.UIRoot);
            }
            
            if (self.OtherRoot == null)
            {
                self.OtherRoot = GameObjectUtil.CreateTransform("OtherRoot", self.UIRoot);
            }

        }
    }

    [ObjectSystem]
    public class GlobalGameObjectComponentDestroySystem : DestroySystem<GlobalGameObjectComponent>
    {
        public override void Destroy(GlobalGameObjectComponent self)
        {
            
           
        }
    }
}