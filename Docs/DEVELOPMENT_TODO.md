# Development To-Do

This file tracks work that needs direct creator input or future implementation. Anything listed here should not be guessed permanently in code.

## Documentation Rule

- Whenever architecture, gameplay rules, content structure, or implementation plans change, update the relevant files in `Docs/`.
- Use `Docs/DEVELOPMENT_TODO.md` for open decisions, standards, and future work.
- Use `Docs/PATCH_NOTES.md` for completed or planned patch slices.
- Use `Docs/GAME_DESIGN.md` for current premise and run-structure decisions.

## Direct Creator Decisions Needed

- Name the game and define the tone boundaries for short Patron reactions: grim, cruel, sardonic, religious, cosmic, protective, tender, or a mix.
- Confirm the starting Host roster. Current implemented class/Host data includes `Paladin` only.
- Confirm the starting patron roster. Current implemented patron data includes `The Devourer` only.
- Define each patron's corruption fantasy. Example for The Devourer: hunger, sacrifice, self-harm for power, healing from lethal blows.
- Define each Patron's commentary vibe so reactive lines stay short, distinct, and mechanically relevant.
- Decide whether patron influence is purely spendable currency, a corruption threshold meter, or both.
- Decide whether sacrificing rewards should always grant influence, or whether reward rarity changes the influence gained.
- Decide whether praying at rest sites should cost influence, consume the rest site, trigger patron dialogue, or present multiple patron-specific options.
- Decide whether card upgrades and card corruptions are mutually exclusive paths or whether a card can be both upgraded and corrupted.
- Provide final class/patron combinations that deserve custom corrupted card variants.
- Define how visible character corruption should be represented: portrait swaps, sprite layers, colors, VFX, UI frame changes, or model changes.
- Decide what the final crisis is when the countdown reaches zero: siege, boss, ritual breach, possession outbreak, patron trial, or another event.
- Decide the exact unlock condition for Time as the secret final boss: total wins, unique Patron wins, Host clears, special board encounter chain, or another milestone.
- Decide how often Time can be encountered before it is fightable and what those encounters do mechanically.
- Decide whether Time eventually becomes an unlockable Patron and what makes that unlock appropriately difficult.
- Decide how Patron memory/meta-awareness escalates across repeated wins without becoming too lore-heavy too early.
- Decide the cadence for unlocking more Patrons and additional Patron commentary/lore through victories.
- Decide which safe haven stats matter for MVP. Current implementation only uses safe haven integrity.
- Decide how Threat Level affects future encounters and board options.
- Decide what rewards Patron-guided board choices should give beyond Influence.
- Decide final tuning for the Shattered Mirror miniboss: exact targeting rules, card-copy cap, and whether draw effects should become something else.
- Decide save/run persistence requirements: single active run, multiple save slots, meta-progression, or no meta-progression.

## Implementation Standards

- Prefer ScriptableObject data for authored content: cards, classes, patrons, enemies, encounters, relics, events, and threat-board option definitions.
- Runtime state must use separate runtime models or instances, not mutate authored assets directly.
- Keep UI presentation separate from gameplay rules. UI components can request actions; gameplay services decide if actions are valid.
- Combat logic should be deterministic enough to test outside a scene wherever possible.
- New systems should be built as reusable services/controllers before being connected to scene glue.
- Avoid hard-coded Paladin/Devourer assumptions outside default sample data and temporary scene assignments.
- Drag-and-drop cards should remain a first-class interaction, with click fallback only as accessibility/convenience.
- Do not add a traditional branching node map. The run structure is the safe-haven Threat Board loop.
- Keep Patron content short and game-facing. Patrons are build identities, not lore delivery machines.

## Next Engineering Work

- Split the current `Combat` MonoBehaviour into a scene-level installer/controller plus separate run, combat, reward, and rest-site coordinators.
- Run `Tools > Card Game > Create Threat Board Scene` in Unity after scripts compile.
- Wire selected combat threats from the Threat Board into the combat scene through a persistent run/session context.
- Make Threat Level scale encounter difficulty or enemy intent patterns.
- Replace placeholder shop, shrine, event, and town-decision logs with real screens or modal flows.
- Decide and implement the final crisis outcome when countdown reaches zero.
- Run `Tools > Card Game > Generate Combat UI Prefabs` in Unity after scripts compile.
- Assign generated prefab references on any scene-authored `CombatBoardView` if the scene uses a placed board instead of runtime fallback creation.
- Split `PlayerView`, pile widgets, reward choices, and rest-site choices into dedicated view components.
- Replace remaining programmatic fallback UI construction with generated or hand-authored prefabs under `Assets/Prefabs/UI/Combat`.
- Create dedicated scenes for boot/class select, patron select, threat board, combat, reward, rest site, trader, event, and run summary.
- Add a class selection flow that writes selected `HeroClassData` into run setup.
- Update player-facing labels from class/class select to Host/Host select while preserving data compatibility.
- Add a patron selection flow that writes selected `PatronData` into run setup.
- Add meta-progression tracking for completed runs, wins, Host clears, Patron clears, and secret boss unlock gates.
- Add Patron commentary unlock tiers based on repeated wins and progression milestones.
- Add rare Time encounter support that can appear before Time is fightable.
- Add Time final boss gating and a future unlock path for Time as a Patron if that design is confirmed.
- Expand card action resolution into composable effects with explicit condition/effect ordering.
- Expand status effects beyond stored stacks: start/end turn behavior, duration rules, tooltips, and icons.
- Expand enemy intents beyond single-turn static data: intent patterns, random choices, phase changes, and telegraphed multi-action turns.
- Add reward generation rules: rarity weighting, class filtering, patron influence hooks, and duplicate controls.
- Add rest-site option data so each patron can define prayer behavior without changing combat code.
- Add tests for deck shuffling, card play, targeting, lethal/non-lethal conditional effects, reward sacrifice, upgrade, corruption, status application, and enemy intents.
- Expand editor validation for enemies, encounters, hero classes, patrons, and rest-site data.

## Implemented Architecture Notes

- `CombatBoardView` is now the main combat UI view component.
- `CombatCardView` owns card presentation, hover state, selected state, playable state, disabled state, and drag/drop behavior.
- `CombatEnemyView` owns enemy presentation, target preview state, click targeting, status display, and floating feedback triggers.
- `CombatPlayValidator` validates combat state, hand ownership, energy, and required enemy targets before cards resolve.
- `TargetResolver` now supports both real target resolution and UI target preview checks.
- `Unit` now has runtime status storage.
- `EnemyData` now supports authored `EnemyIntentData`; old `attackDamage` remains as a fallback for existing assets.
- `CardAssetValidator` gives a Unity menu validation pass for card authoring mistakes.
- `ThreatBoardConfig` and `ThreatBoardOptionData` define the safe-haven board option pool.
- `ThreatBoardState` stores countdown, threat level, safe haven integrity, and current board options.
- `ThreatBoardController` and `ThreatBoardView` provide a simple interactive Bounty/Threat Board screen.
- `ThreatBoardSceneGenerator` adds a Unity menu item to generate a working board scene.
- `EnemySpecialBehavior.MirrorMiniBoss` enables enemies that spend their first turn observing and then mimic the player's previous completed turn.
- `MirrorCardResolver` maps player card effects into mirrored enemy actions.

## Current Sample Content

- `Assets/HeroClasses/Paladin.asset` is sample class data for the Paladin.
- `Assets/Patrons/The Devourer.asset` is sample patron data for The Devourer.
- `Assets/Cards/Paladin/Devouring Smite.asset` demonstrates lethal/non-lethal conditional corruption behavior.
- `Assets/ThreatBoard/DefaultThreatBoardConfig.asset` is sample safe-haven board data.
- `Assets/ThreatBoard/Options/` contains sample board options for threats, trader, shrine, event, town decision, Patron option, and final crisis.
- `Assets/Enemies/Shattered Mirror.asset` is the sample Mirror miniboss enemy.
- `Assets/Encounters/MirrorMiniBossEncounter.asset` is the sample Mirror miniboss encounter.
- `Assets/ThreatBoard/Options/The Glass Double.asset` adds the Mirror miniboss to the Threat Board option pool.
