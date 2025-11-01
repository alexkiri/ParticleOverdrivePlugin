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
    public static ConfigEntry<int> particlesMultiplierConfigEntry;

    private void Awake() {
        Log = base.Logger;
        harmony.PatchAll();

        particlesMultiplierConfigEntry = Config.Bind(
            "General",
            "ParticlesMultiplier",
            1,
            new ConfigDescription(
                "Greatly increases the amount of particles, while greatly decreasing performance. Ridiculously high values are allowed, use carefully.",
                new AcceptableValueRange<int>(1, 256),
                []
            )
        );
        particlesMultiplierConfigEntry.SettingChanged += (sender, args) => {
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
        upgradeParticleSystems();
    }

    private void upgradeParticleSystems() {
        var particleSystems = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Log.LogDebug($"Found particleSystems: {particleSystems.Length}");

        var filteredParticleSystems = particleSystems
            .Where(ps =>
                ps.emission.enabled
                && ps.emission.rateOverTime.constant > 0
                && ps.emission.rateOverTimeMultiplier > 0
                && ps.main.cullingMode == ParticleSystemCullingMode.PauseAndCatchup
                && ps.gameObject.GetComponent<ReduceParticleEffects>() == null
            )
            .ToArray();

        // Log.LogDebug("name, instanceID, maxParticles, duration, rateOverTimeMultiplier");
        foreach (var ps in filteredParticleSystems) {
            // Log.LogDebug($"{ps.gameObject.name}, {ps.GetInstanceID()}, {ps.main.maxParticles}, {ps.main.duration}, {ps.emission.rateOverTimeMultiplier}, {ps.emission.burstCount}");
            ps.gameObject.AddComponentIfNotPresent<ReduceParticleEffects>();
        }
        Log.LogDebug($"Filtered and upgraded: {filteredParticleSystems.Length}");
    }
}
