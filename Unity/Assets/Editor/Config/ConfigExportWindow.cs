using System;
using System.IO;
using UnityEditor;

namespace Framework.Editor
{
    public sealed class ExportWindow : EditorWindow
    {
        /// <summary>
        /// 页签
        /// </summary>
        private readonly string[] tabs = {"ExcelToJson", "NetProto"};
        
        /// <summary>
        /// 监视源文件夹文件变化
        /// </summary>
       // private static FileSystemWatcher fileSystemWatcher = null;
    }
}