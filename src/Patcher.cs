using HarmonyLib;
using BepInEx.Logging;

namespace ParticleOverdrive;

[HarmonyPatch]
public class Patcher {
    private static readonly ManualLogSource Log = ParticleOverdrive.Log;

    [HarmonyPatch(typeof(ReduceParticleEffects), nameof(ReduceParticleEffects.SetEmission))]
    [HarmonyPrefix]
    static bool ReduceParticleEffects_SetEmission(ReduceParticleEffects __instance) {
        float multiplier;
        var presetMultiplier = ParticleOverdrive.particlesMultiplierConfigEntry.Value;
        if (presetMultiplier == ParticleOverdrive.ParticlesMultiplier.Extreme) {
            multiplier = ParticleOverdrive.extremeMultiplierConfigEntry.Value;
        } else {
            multiplier = ParticleOverdrive.MultiplierToFloat(presetMultiplier);
        }

        // Log.LogDebug($"ReduceParticleEffects.SetEmission called on {__instance}[{__instance.GetInstanceID()}], (rateOverTimeMultiplier {__instance.emitter.emission.rateOverTimeMultiplier}, maxParticles {__instance.emitter.main.maxParticles}) x {multiplier}");
        if (__instance.emitter != null) {
            var emission = __instance.emitter.emission;
            emission.rateOverTimeMultiplier = __instance.emissionRateHigh * multiplier;
            var main = __instance.emitter.main;
            main.maxParticles = (int)(__instance.maxParticlesHigh * multiplier);
        }
        return false;
    }
}