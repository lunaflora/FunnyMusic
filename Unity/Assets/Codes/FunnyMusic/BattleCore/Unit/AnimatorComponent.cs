using System;
using System.Collections.Generic;
using FLib;
using Framework;
using UnityEngine;
using UPlayable.AnimationMixer;

namespace FunnyMusic
{

    /// <summary>
    /// 使用Animation Playable控制动画
    /// </summary>
    [ComponentOf(typeof(Unit))]
    public class AnimatorComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public Dictionary<PlayerAction, AnimationClip> AnimationClips = new Dictionary<PlayerAction, AnimationClip>();
        /// <summary>
        /// 动画管理器
        /// </summary>
        public AnimationMixerManager AnimationMixerManager;
        /// <summary>
        /// <summary>
        /// 单个动画播放器
        /// </summary>
        public AnimationClipOutput AnimationClipOutput;
        /// <summary>
        /// 多个动画混合播放器
        /// </summary>
        public AnimationMixerOuput AnimationMixerOuput;
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
            GameObjectComponent gameObjectComponent = self.GetParent<Unit>().GetComponent<GameObjectComponent>();
            self.AnimationClips = gameObjectComponent.GameObject.GetComponentInChildren<AnimationMap>(true)
                .PlayerAnimationMap;

            self.AnimationMixerManager =
                gameObjectComponent.GameObject.GetComponentInChildren<AnimationMixerManager>(true);
            self.AnimationClipOutput =  gameObjectComponent.GameObject.GetComponentInChildren<AnimationClipOutput>(true);
            self.AnimationMixerOuput  = gameObjectComponent.GameObject.GetComponentInChildren<AnimationMixerOuput>(true);
            

        }

        public static void PlayAnimation(this AnimatorComponent self, PlayerAction playerAction, float speed = 1.0f, float fadeIn = 0.0f)
        {
            if (!self.AnimationClips.ContainsKey(playerAction))
            {
                LoggerHelper.LogPlayer($"payer action not find {playerAction}");
                return;
            }

            self.AnimationClipOutput.ToClip = self.AnimationClips[playerAction];
            self.AnimationClipOutput.SetSpeed(speed);
            self.AnimationClipOutput.SetFadeInTime(fadeIn);
            

        }

        public static void MixAnimation(this AnimatorComponent self)
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