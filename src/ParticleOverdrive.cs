using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace ParticleOverdrive;

[BepInAutoPlugin(id: "io.github.alexkiri.particleoverdrive")]
public partial class ParticleOverdrive : BaseUnityPlugin {
    internal static ManualLogSource Log;
    private readonly Harmony harmony = new(Id);
    public static ConfigEntry<ParticlesMultiplier> particlesMultiplierConfigEntry;
    public static ConfigEntry<bool> upgradeAllParticleSystemsConfigEntry;
    public static ConfigEntry<int> extremeMultiplierConfigEntry;

    public enum ParticlesMultiplier {
        VanillaLow,
        VanillaHigh,
        Double,
        Triple,
        Quadruple,
        High5x,
        High6x,
        High7x,
        High8x,
        Ultra10x,
        Ultra12x,
        Ultra16x,
        Extreme
    }

    public static float MultiplierToFloat(ParticlesMultiplier multiplier) {
        return multiplier switch {
            ParticlesMultiplier.VanillaLow => 0.5f,
            ParticlesMultiplier.VanillaHigh => 1.0f,
            ParticlesMultiplier.Double => 2.0f,
            ParticlesMultiplier.Triple => 3.0f,
            ParticlesMultiplier.Quadruple => 4.0f,
            ParticlesMultiplier.High5x => 5.0f,
            ParticlesMultiplier.High6x => 6.0f,
            ParticlesMultiplier.High7x => 7.0f,
            ParticlesMultiplier.High8x => 8.0f,
            ParticlesMultiplier.Ultra10x => 10.0f,
            ParticlesMultiplier.Ultra12x => 12.0f,
            ParticlesMultiplier.Ultra16x => 16.0f,
            _ => 1.0f
        };
    }

    private void Awake() {
        Log = base.Logger;
        harmony.PatchAll();

        particlesMultiplierConfigEntry = Config.Bind(
            "General",
            "ParticlesMultiplier",
            ParticlesMultiplier.VanillaHigh,
            new ConfigDescription(
                "Greatly increases the amount of particles, while greatly decreasing performance. Ridiculously high values are allowed, use carefully.",
                null,
                new ConfigurationManagerAttributes { Order = 3 }
            )
        );
        particlesMultiplierConfigEntry.SettingChanged += (sender, args) => {
            GameManager.instance.RefreshParticleSystems();
        };

        upgradeAllParticleSystemsConfigEntry = Config.Bind(
            "General",
            "UpgradeAllParticleSystems",
            true,
            new ConfigDescription(
                "yep",
                null,
                new ConfigurationManagerAttributes { Order = 1 }
            )
        );
        extremeMultiplierConfigEntry = Config.Bind(
            "General",
            "ExtremeMultiplier",
            32,
            new ConfigDescription(
                "ye ye",
                new AcceptableValueRange<int>(16, 128),
                new ConfigurationManagerAttributes { IsAdvanced = true, Order = 2 }
            )
        );
        extremeMultiplierConfigEntry.SettingChanged += (sender, args) => {
            particlesMultiplierConfigEntry.Value = ParticlesMultiplier.Extreme;
            GameManager.instance.RefreshParticleSystems();
        };

        Log.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    private void OnDestroy() {
        harmony.UnpatchSelf();
        Log.LogInfo($"Plugin {Name} ({Id}) has unloaded!");
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Log.LogDebug($"loaded '{scene.name}'");
        if (upgradeAllParticleSystemsConfigEntry.Value) {
            upgradeAllParticleSystems();
        }
    }

    private void upgradeAllParticleSystems() {
        var particleSystems = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Log.LogDebug($"Found particleSystems: {particleSystems.Length}");

        var filteredParticleSystems = particleSystems
            .Where(ps =>
                ps.emission.enabled
                && ps.emission.rateOverTime.constant > 0
                && ps.emission.rateOverTimeMultiplier > 0
                && ps.gameObject.GetComponent<ReduceParticleEffects>() == null
            )
            .ToArray();

        foreach (var ps in filteredParticleSystems) {
            ps.gameObject.AddComponentIfNotPresent<ReduceParticleEffects>();
        }
        Log.LogInfo($"Filtered and upgraded: {filteredParticleSystems.Length}");
    }
}
