﻿using System.IO;
using System.Reflection;
using HarmonyLib;
using IPA;
using IPA.Utilities;
using JetBrains.Annotations;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SimpleSoundChanger
{
    [Plugin(RuntimeOptions.DynamicInit), UsedImplicitly]
    public class Plugin
    {
        internal static IPALogger Log { get; private set; }
        
        private static Harmony _harmony;
        
        internal static readonly string SoundsPath = Path.Combine(UnityGame.UserDataPath, "SimpleSoundChanger");

        [Init]
        public void Init(Zenjector zenjector, IPALogger logger)
        {
            Log = logger;

            zenjector.UseLogger(logger);
            zenjector.UseMetadataBinder<Plugin>();
        }
        
        [OnEnable]
        public void OnEnable()
        {
            _harmony = new Harmony("TheBlackParrot.SimpleSoundChanger");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }
    }
}