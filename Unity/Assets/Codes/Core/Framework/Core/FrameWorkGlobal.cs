namespace Framework
{
    public static class GlobalMagicNumber
    {
        public static long ConfigFixNumberScale = 10000;

        /// <summary>
        /// 当前位于的局内ID，一般同时只有一个Scene存在
        /// </summary>
        public static long CurrentSceneID = 999999;
        
        //UI的一些预设前缀 注意这个是对应编辑时 UICodeSpawner
        //1.UI窗口以Wd开头（带Canvas） 
        //2.UI公共Panel以Pn开头（不带Canvas，需要挂载到Canvas下的） 
        public static string UIWindowPrefix  = "Wd";
        public static string UIPanelPrefix  = "Pn";
        public static string UIItemPrefix = "Item";
        
        
        //ui节点命名规则 
        //引用Widget的以Wg开头
        //引用GameObjec的以Go开头
        public static string UIGameObjectPrefix = "Go";
        public static string UIWidgetPrefix = "Wg";

    }
}