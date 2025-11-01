# Particle Overdrive

A mod for Hollow Knight: Silksong that allows increasing the amount of particles to ridiculous levels


## Installation

### The Simple Way

Use r2modman or Gale.

### Manual

1. Download [BepInExPack Silksong](https://thunderstore.io/c/hollow-knight-silksong/p/BepInEx/BepInExPack_Silksong/) and extract it to the game folder, next to the game executable
2. Download [BepinExConfigurationManager](https://thunderstore.io/c/hollow-knight-silksong/p/jakobhellermann/BepInExConfigurationManager/)
2. Download from [thunderstore](https://thunderstore.io/c/hollow-knight-silksong/p/alexkiri/ParticleOverdrive/) or [github]
(https://github.com/alexkiri/ParticleOverdrivePlugin/releases) and extract it inside of `<game folder>/BepInEx/plugins`
3.
    - (Windows) Run the game normally
    - (Linux/MacOS) Run `run_bepinex.sh`


## Configuration

Use `BepinExConfigurationManager` (open with F1 by default) to adjust the parameter in realtime, or edit the `io.github.alexkiri.particleoverdrive.cfg` file inside `<game folder>/BepInEx/config`

`ParticlesMultiplier`
- parameter can greatly increases the amount of particles
- can greatly decrease performance
- default value: 1
- acceptable value range: from 1 to 256
- useful values 2 to 10
- ridiculously high values are allowed, use carefully, as they can make the game unplayable

The "Particle Effects" setting from the main menu does not have any effect with this mod enabled.
