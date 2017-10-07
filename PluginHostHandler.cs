using System;
using ImGeneralPluginEngine;
using NeonScripting;

namespace MiniPlayer
{
    public sealed class PluginHostHandler
    {
        public IImGeneralPlugin ThisPlugin { get; set; }
        public INeonScriptHost ScriptHost { get; set; }
        public Action<IImGeneralPlugin> PluginCloseAction { get; set; }
        private static readonly PluginHostHandler _instance = new PluginHostHandler();
        public static PluginHostHandler Instance
        {
            get { return _instance; }
        }
    }
}
