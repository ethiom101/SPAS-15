using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Ethiom101
{
	[BepInPlugin("h3vr.SPATCH", "SPATCH", "0.0.1")]
	[BepInProcess("h3vr.exe")]
    public partial class SPAS15Fix : BaseUnityPlugin
    {
        /* == Quick Start == 
         * Your plugin class is a Unity MonoBehaviour that gets added to a global game object when the game starts.
         * You should use Awake to initialize yourself, read configs, register stuff, etc.
         * If you need to use Update or other Unity event methods those will work too.
         *
         * Some references on how to do various things:
         * Adding config settings to your plugin: https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/4_configuration.html
         * Hooking / Patching game methods: https://harmony.pardeike.net/articles/patching.html
         * Also check out the Unity documentation: https://docs.unity3d.com/560/Documentation/ScriptReference/index.html
         * And the C# documentation: https://learn.microsoft.com/en-us/dotnet/csharp/
         */
        
        private void Awake()
        {
            Logger = base.Logger;
            
            // Your plugin's ID, Name, and Version are available here.
            Logger.LogMessage($"Hey guys. Thanks for tuning in to another video on Forgotten Weapons dot com. I'm Ethiom101 and today we're fixing behaviour on the SPAS-15.");

            Harmony.CreateAndPatchAll( typeof( SPATCH ) );
        }
        
        // The line below allows access to your plugin's logger from anywhere in your code, including outside of this file.
        // Use it with 'YourPlugin.Logger.LogInfo(message)' (or any of the other Log* methods)
        internal new static ManualLogSource Logger { get; private set; }
    }
}
