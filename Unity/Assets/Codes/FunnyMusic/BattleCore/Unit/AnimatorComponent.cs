using System;
using System.Collections.Generic;
using FLib;
using Framework;
using UnityEngine;

namespace FunnyMusic
{

    /// <summary>
    /// 使用Animation Playable控制动画
    /// </summary>
    public class AnimatorComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public Dictionary<string, AnimationClip> AnimationClips = new Dictionary<string, AnimationClip>();
        
        
    }
    
    public static class AnimatorComponentSystem
    {
        [ObjectSystem]
        public class AnimatorComponentAwakeSystem: AwakeSystem<AnimatorComponent>
        {
            public override void Awake(AnimatorComponent self)
            {
                self.Awake();
            }
        }
        
        [ObjectSystem]
        public class AnimatorComponentUpdateSystem : UpdateSystem<AnimatorComponent>
        {
            public override void Update(AnimatorComponent self)
            {
                self.Update();
            }
        }
	
        [ObjectSystem]
        public class AnimatorComponentDestroySystem : DestroySystem<AnimatorComponent>
        {
            public override void Destroy(AnimatorComponent self)
            {
                self.AnimationClips = null;
              
            }
        }
        
        public static void Awake(this AnimatorComponent self)
        {
            
        }
        
        public static void Update(this AnimatorComponent self)
        {
           
        }


        #region Play
        
        /// <summary>
        /// 按固定持续时间播放动画
        /// </summary>
       

        #endregion


        #region Query

       



        #endregion
		
    }
    
    

}