using System;
using UnityEngine;

namespace Test
{
    public class MaterialProperty : MonoBehaviour
    {
        ///统一使用MaterialPropertyBlock修改材质属性
        /// </summary>
        private MaterialPropertyBlock propertyBlock;

        private void Start()
        {
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            
            propertyBlock.SetColor("_BaseColor", Color.red);
            //apply propertyBlock to renderer
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }
    }
}