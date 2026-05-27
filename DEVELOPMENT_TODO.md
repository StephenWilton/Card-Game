# Development To-Do

This file tracks work that needs direct creator input or future implementation. Anything listed here should not be guessed permanently in code.

## Direct Creator Decisions Needed

- Name the game and define the tone boundaries for the narrator: grim, cruel, sardonic, religious, cosmic, or a mix.
- Confirm the starting class roster. Current implemented class data includes `Paladin` only.
- Confirm the starting patron roster. Current implemented patron data includes `The Devourer` only.
- Define each patron's corruption fantasy. Example for The Devourer: hunger, sacrifice, self-harm for power, healing from lethal blows.
- Decide whether patron influence is purely spendable currency, a corruption threshold meter, or both.
- Decide whether sacrificing rewards should always grant influence, or whether reward rarity changes the influence gained.
- Decide whether praying at rest sites should cost influence, consume the rest site, trigger patron dialogue, or present multiple patron-specific options.
- Decide whether card upgrades and card corruptions are mutually exclusive paths or whether a card can be both upgraded and corrupted.
- Provide final class/patron combinations that deserve custom corrupted card variants.
- Define how visible character corruption should be represented: portrait swaps, sprite layers, colors, VFX, UI frame changes, or model changes.
- Decide encounter/map structure: Slay the Spire node map, linear rooms, branching rooms, or another format.
- Decide save/run persistence requirements: single active run, multiple save slots, meta-progression, or no meta-progression.

## Implementation Standards

- Prefer ScriptableObject data for authored content: cards, classes, patrons, enemies, encounters, relics, events, and map node definitions.
- Runtime state must use separate runtime models or instances, not mutate authored assets directly.
- Keep UI presentation separate from gameplay rules. UI components can request actions; gameplay services decide if actions are valid.
- Combat logic should be deterministic enough to test outside a scene wherever possible.
- New systems should be built as reusable services/controllers before being connected to scene glue.
- Avoid hard-coded Paladin/Devourer assumptions outside default sample data and temporary scene assignments.
- Drag-and-drop cards should remain a first-class interaction, with click fallback only as accessibility/convenience.

## Next Engineering Work

- Split the current `Combat` MonoBehaviour into a scene-level installer/controller plus separate run, combat, reward, and rest-site coordinators.
- Replace programmatic combat UI construction with prefab-backed views under `Assets/Prefabs/UI/Combat`.
- Create dedicated scenes for boot/class select, patron select, combat, reward, rest site, map, and run summary.
- Add a class selection flow that writes selected `HeroClassData` into run setup.
- Add a patron selection flow that writes selected `PatronData` into run setup.
- Expand card action resolution into composable effects with explicit condition/effect ordering.
- Add status effect runtime support for player and enemies.
- Add enemy intents as data-driven actions instead of fixed attack damage.
- Add reward generation rules: rarity weighting, class filtering, patron influence hooks, and duplicate controls.
- Add rest-site option data so each patron can define prayer behavior without changing combat code.
- Add tests for deck shuffling, card play, targeting, lethal/non-lethal conditional effects, reward sacrifice, upgrade, and corruption.
- Add editor validation for card assets to catch missing names, invalid targets, missing upgrade/corruption links, and orphaned reward cards.

## Current Sample Content

- `Assets/HeroClasses/Paladin.asset` is sample class data for the Paladin.
- `Assets/Patrons/The Devourer.asset` is sample patron data for The Devourer.
- `Assets/Cards/Paladin/Devouring Smite.asset` demonstrates lethal/non-lethal conditional corruption behavior.

