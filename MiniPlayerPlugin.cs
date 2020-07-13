using System;
using ImGeneralPluginEngine;
using ImGeneralPluginEngine.Abstractions;
using NeonScripting;
using NeonScripting.Models;

namespace MiniPlayer
{
    public class MiniPlayerPlugin : IImGeneralPlugin
    {
        private MiniPlayerWindow _miniPlayerWindow;
        public void InitializePlugin(INeonScriptHost host, Action<IImGeneralPlugin> pluginCloseAction)
        {
            PluginHostHandler.Instance.ScriptHost = host;
            PluginHostHandler.Instance.PluginCloseAction = pluginCloseAction;
            PluginHostHandler.Instance.ThisPlugin = this;
            _miniPlayerWindow = new MiniPlayerWindow { ShowInTaskbar = false };
            _miniPlayerWindow.Show();
        }

        public void OnEvent(NeonScriptEventTypes eventType)
        {
            _miniPlayerWindow.OnEvent(eventType);
        }

        public void ClosePlugin()
        {
            _miniPlayerWindow.Close();
        }

        public string Name => "Mini player";
        public string Author => "Mikael Stalvik";
    }
}
