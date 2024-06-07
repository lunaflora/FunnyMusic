using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameServerUrl", menuName = "Settings/GameServerUrl", order =3)]
    public class GameServerUrl : ScriptableObject
    {
        [Header("默认地址索引")]
        public int urlIndex = 0; 
        [Header("所有服务器地址")]
        public string[] Urls = { "http://192.168.0.12:9000/" };


    }
}
