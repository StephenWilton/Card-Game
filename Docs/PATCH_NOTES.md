# Patch Notes

This file tracks implementation slices and the practical steps attached to each one.

## V2.2 - Hosts, Patron Memory, and Time

Goal: document the next layer of run identity and long-term progression without implementing it prematurely.

### Current Status

- Player-facing classes are now documented as Hosts.
- Patrons are defined as vibe-driven commentary sources and gameplay identities, not lore-heavy narrators.
- Repeated wins can make Patrons subtly aware that something is wrong with the loop.
- Repeated wins can unlock more Patrons and more Patron commentary/lore.
- Time is documented as the long-term secret final boss/final crisis.
- Time may appear in rare pre-fight encounters before becoming fightable.
- Time may eventually become an unlockable Patron for high-progression players.

### Not Implemented Yet

- Host terminology is not fully reflected in scene/UI labels or code names.
- Meta-progression does not yet track wins, clears, or Patron unlock gates.
- Patron commentary does not yet have unlock tiers or run-memory variants.
- Time encounters, Time boss gating, and Time-as-Patron unlocks are not implemented.

## V2 - Safe Haven Threat Board Foundation

Goal: replace traditional map assumptions with a simple safe-haven Bounty/Threat Board loop.

### Current Status

- The canonical run structure is now documented in `Docs/GAME_DESIGN.md`.
- Added `ThreatBoardConfig` for countdown, threat level, safe haven integrity, option count, and option pool.
- Added `ThreatBoardOptionData` for authored normal threats, elite threats, trader, shrine, random event, Patron-guided option, town decision, and final crisis.
- Added runtime `ThreatBoardState`, `ThreatBoardService`, `ThreatBoardOption`, and `ThreatBoardSelectionResult`.
- Added `ThreatBoardController` and `ThreatBoardView` for an interactive board screen.
- Added sample board data in `Assets/ThreatBoard/DefaultThreatBoardConfig.asset`.
- Added sample options in `Assets/ThreatBoard/Options/`.
- Added `Tools > Card Game > Create Threat Board Scene`.
- Added `The Glass Double` elite threat option pointing at the Mirror miniboss encounter.

### Unity Editor Steps

1. Open the project in Unity.
2. Let scripts compile.
3. Run `Tools > Card Game > Create Threat Board Scene`.
4. Open `Assets/Scenes/ThreatBoardScene.unity`.
5. Press Play and choose board options to verify countdown, threat level, safe haven integrity, Patron suggestion, and log updates.

### Not Completed Yet

- Board combat choices do not yet load the combat scene or pass selected `EncounterData`.
- Trader, shrine, random event, and town decision options currently log placeholder outcomes.
- Threat Level does not yet scale encounters.
- The final crisis is a placeholder option using the existing opening encounter reference.
- Safe haven is currently one integrity number, not a deeper town system.

## V2.1 - Shattered Mirror Miniboss

Goal: add a miniboss that does nothing on its first enemy turn, then mimics the player's turns afterward.

### Current Status

- Added `EnemySpecialBehavior.MirrorMiniBoss`.
- Added `MirrorCardResolver`.
- `Combat` now records cards played during the current player turn.
- `EnemyTurnResolver` passes the completed player turn into Mirror enemies.
- Added `Assets/Enemies/Shattered Mirror.asset`.
- Added `Assets/Encounters/MirrorMiniBossEncounter.asset`.
- Added `Assets/ThreatBoard/Options/The Glass Double.asset`.
- Added the Mirror option to `Assets/ThreatBoard/DefaultThreatBoardConfig.asset`.

### Current Mimic Rules

- First Mirror enemy turn: observe and do nothing.
- Later Mirror enemy turns: replay the player's previous completed turn.
- Effects targeting `Player` are treated as Mirror self-effects.
- Effects targeting enemies are reflected into the player.
- `Both` and `AllUnits` affect both the Mirror and the player.
- Conditional effects such as lethal/non-lethal checks are evaluated from the mirrored result.
- Draw is not mirrored yet.

### Not Completed Yet

- The Mirror does not have custom visuals or animations.
- The Mirror does not cap the number of copied cards.
- Draw effects do not translate into an alternate Mirror behavior.
- Exact target-copying is not implemented for future multi-character/player-side scenarios.

## V1 - Interactive Combat Foundation

Goal: replace the current text-box/button combat surface with real interactive combat pieces while keeping the rules system modular.

### Implementation Steps

1. Create prefab-backed combat UI.
   - Create `Assets/Prefabs/UI/Combat/CardView.prefab`.
   - Create `Assets/Prefabs/UI/Combat/EnemyView.prefab`.
   - Create `Assets/Prefabs/UI/Combat/PlayerView.prefab`.
   - Create `Assets/Prefabs/UI/Combat/CombatBoardView.prefab`.
   - Use TextMesh Pro fields, image slots, layout containers, and serialized references instead of constructing the whole combat UI in code.

2. Convert hand cards into real card objects.
   - `CardView` displays name, cost, type, description, rarity, corruption state, and target style.
   - `CardView` supports hover, selected, dragging, playable, and disabled visual states.
   - Cards remain bound to `CardInstance`, not directly to mutable `CardData`.

3. Add drag-and-drop card play.
   - Dragging a card lifts it above the hand.
   - Valid targets highlight while dragging.
   - Dropping on a valid enemy requests a play action from the combat controller.
   - Dropping on invalid space returns the card to hand.
   - Clicking can remain as a fallback, but dragging is the primary interaction.

4. Convert enemies into targetable board objects.
   - `EnemyView` displays health, block, name, position, and intent.
   - `EnemyView` exposes target data through a component rather than relying on button text.
   - Front row, back row, and column targeting should highlight correctly for cards like row attacks and pierce-column attacks.

5. Separate UI requests from combat rules.
   - UI sends commands such as `TryPlayCard(card, target)`.
   - Combat controller validates state, energy, hand ownership, and targeting.
   - Card resolver applies effects only after validation succeeds.
   - UI updates from combat state after the rules finish.

6. Add basic combat feedback.
   - Played cards animate toward their target or discard pile.
   - Enemies flash or shake when damaged.
   - Player and enemies show floating numbers for damage, block, and healing.
   - Energy, draw pile, discard pile, and hand update visually after each action.

7. Keep this V1 scoped.
   - Do not build class select, patron select, map, reward scene, or rest scene in this patch unless needed to support combat interaction.
   - Do not add new card mechanics before the card interaction layer is stable.
   - Do not hard-code a final UI style. Build the interaction architecture first, then polish.

### Current Status

- In progress.
- Runtime card identity exists through `CardInstance`.
- Sample Paladin and The Devourer data assets exist.
- Combat now renders through `CombatBoardView` instead of the old monolithic text-button renderer.
- Hand cards now use `CombatCardView`, with hover, selected, playable, disabled, and drag states.
- Enemies now use `CombatEnemyView`, with clickable target views, target highlighting, status display, and floating feedback hooks.
- Combat play attempts now pass through `CombatPlayValidator` before effects resolve.
- `TargetResolver` now supports target preview checks for selectable and affected enemies.
- Status runtime support exists on `Unit`; cards can apply statuses and conditionally check player/enemy statuses.
- Enemy intents now support authored `EnemyIntentData` actions with fallback attack behavior for existing enemy assets.
- Editor tools added:
  - `Tools/Card Game/Generate Combat UI Prefabs`
  - `Tools/Card Game/Validate Card Assets`

### Unity Editor Steps

1. Open the project in Unity.
2. Let Unity finish compiling scripts.
3. Run `Tools > Card Game > Generate Combat UI Prefabs`.
4. Confirm these files exist:
   - `Assets/Prefabs/UI/Combat/CardView.prefab`
   - `Assets/Prefabs/UI/Combat/EnemyView.prefab`
   - `Assets/Prefabs/UI/Combat/CombatBoardView.prefab`
5. Run `Tools > Card Game > Validate Card Assets`.
6. Review Console errors/warnings and fix authored card data as needed.

### Not Completed In V1 Yet

- `PlayerView.prefab` is not split into its own component yet; player state is currently part of `CombatBoardView`.
- Played-card movement animations are not finished.
- Enemy hit shake/flash is not finished; floating text hooks exist.
- Draw pile and discard pile are textual in `CombatBoardView`; dedicated pile widgets still need to be split out.
- The reward and rest-site UI still use command buttons and should become dedicated views in a later patch.
- Unity batch-mode prefab generation could not be completed from the shell while the editor/project lock was active, so the menu item above is the supported route.
