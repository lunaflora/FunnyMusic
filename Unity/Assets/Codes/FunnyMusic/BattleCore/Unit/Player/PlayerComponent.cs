using Framework;
using UniFramework.Machine;

namespace FunnyMusic
{
    [ComponentOf(typeof(Unit))]
    public class PlayerComponent : Entity,IAwake,IDestroy,IUpdate
    {
        //状态机相关
        public  StateMachine PlayereMachine;
    }
    
     
    [ObjectSystem]
    public class PlayerComponentAwakeSystem : AwakeSystem<PlayerComponent>
    {
        public override void Awake(PlayerComponent self)
        {
            self.Awake();
        }
    }
	
    [ObjectSystem]
    public class PlayerComponentDestroySystem : DestroySystem<PlayerComponent>
    {
        public override void Destroy(PlayerComponent self)
        {
        }
    }
    
    [ObjectSystem]
    public class PlayerComponentUpdateSystem : UpdateSystem<PlayerComponent>
    {
        public override void Update(PlayerComponent self)
        {
        }
    }

    public static class PlayerComponentSystem
    {
        public static void Awake(this PlayerComponent self)
        {
            
            self.InitStateMachine();
       
        }

        private static void InitStateMachine(this PlayerComponent self)
        {
            self.PlayereMachine = new StateMachine(self);
            self.PlayereMachine.SetBlackboardValue("Unit", self.GetParent<Unit>());   
            
            //添加行为状态
            self.PlayereMachine.AddNode<PlayerIdleState>();
            
            //设置默认状态
            self.PlayereMachine.Run<PlayerIdleState>();
            
        }

        public static void Update(this PlayerComponent self)
        {
            
            self.PlayereMachine.Update();
            
        }
    }
}