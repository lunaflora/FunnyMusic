using UnityEngine;

namespace Framework
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
        
        public Transform Global { get; set; }
        
    }

    [ObjectSystem]
    public class GlobalGameObjectComponentAwakeSystem : AwakeSystem<GlobalGameObjectComponent>
    {
        public override void Awake(GlobalGameObjectComponent self)
        {
            GlobalGameObjectComponent.Instance = self;
            
            if (self.Global == null)
            {
                self.Global = GameObjectUtil.CreateTransform("Global", null, true);
            }

            if (self.UIRoot == null)
            {
                self.UIRoot = GameObjectUtil.CreateTransform("UIRoot", self.Global);
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
            
            if (self.Global != null)
            {
                GameObject.Destroy(self.Global);
            }
        }
    }



}