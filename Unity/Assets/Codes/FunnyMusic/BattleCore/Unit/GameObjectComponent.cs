using Framework;
using UnityEngine;

namespace FunnyMusic
{
    [ComponentOf(typeof(Unit))]
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject { get; set; }
    }
}