using Cysharp.Threading.Tasks;
using FLib;
using UniFramework.Machine;
using UnityEngine;

namespace Core
{
    internal class FsmUpdaterDone : IStateNode
    {
        private StateMachine _machine;
        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }
        
        void IStateNode.OnEnter() 
        {
            FDebug.Print("资源更新完成！");
           
        }
        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }
        
       
    }
}