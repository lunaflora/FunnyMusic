using UniFramework.Machine;

namespace FunnyMusic
{
    /// <summary>
    /// 玩家休闲状态
    /// </summary>
    public class PlayerIdleState : IStateNode
    {
        private StateMachine machine;
        private Unit unit;
        private AnimatorComponent animatorComponent;
        

        void IStateNode.OnCreate(StateMachine machine)
        {
            this.machine = machine;
            unit  = (Unit)machine.GetBlackboardValue("Unit");
            animatorComponent = unit.GetComponent<AnimatorComponent>();

        }
        void IStateNode.OnEnter()
        {
           animatorComponent.PlayAnimation(PlayerAction.Idle,1);
         
        }
        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }

       
    }
}