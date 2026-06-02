# Game Design Notes

## Current Premise

The player is defending a town or safe haven during a spreading supernatural crisis. The world outside the walls is being overrun by possessed people, cultists, undead, corrupted knights, zealots, and other hostile forces. Some enemies are monsters. Some are humans twisted by belief, debt, possession, desperation, or corruption.

The player character is cursed or bound to a Patron. The Patron changes cards and combat style. Patrons should feel like gameplay identities and build modifiers first, not exposition systems.

Player-facing classes are called Hosts. A Host defines the player's starting deck, combat identity, and baseline fantasy. Code may still use class-oriented names until a future naming cleanup, but the game should present this choice as selecting a Host.

## Run Structure

The game does not use a traditional node map.

The core loop is:

1. Finish an encounter or opportunity.
2. Return to the Bounty Board / Threat Board.
3. Choose which threat or opportunity to answer.
4. Countdown advances.
5. Threat Level changes.
6. Repeat until the final crisis arrives.

## Threat Board

The board presents several options at a time:

- Normal enemy threat
- Elite enemy threat
- Trader/shop
- Shrine/upgrade/corruption opportunity
- Random event
- Patron-guided option
- Town/safe haven decision

The player should feel like they are choosing which problems to answer while trying to protect and sustain the safe haven before the countdown reaches zero.

## Countdown

There is a countdown toward a final crisis, boss, siege, ritual breach, or major event. The exact fiction is undecided. For now, it is a placeholder final crisis.

The countdown is game pressure, not a world map. Each board choice consumes time. When the countdown reaches zero, the board should present the final crisis option.

The long-term secret final crisis is Time. The game can be beaten before Time is fully understood or fightable. Time should feel like the thing pressing on the run structure itself rather than a normal boss sitting at the end of a map.

Time may sometimes be encountered before it can be fought. These encounters should feel strange, brief, and mechanically meaningful, but not like normal combat. After enough wins or progression milestones, the player may unlock the ability to fight Time. Time may eventually become an unlockable Patron for players who push far enough.

## Threat Level

Threat Level rises over time. It can eventually:

- Increase enemy difficulty
- Change available board options
- Make events harsher
- Improve rewards from dangerous threats
- Put pressure on safe haven integrity

Threat Level is a design direction only right now. Rebuild the actual system from scratch when the board loop is ready.

## Safe Haven

The safe haven has an integrity value. It is currently a simple number, not a full town simulation.

Future safe haven systems may include:

- Integrity
- Morale
- Supplies
- Defenses
- Refugees
- Districts or facilities

For MVP, keep this simple. The board should communicate that the town matters without becoming a management game.

## Patron Role

The Patron may mark one board option. Taking it can grant Patron Influence, corruption opportunities, or unique rewards.

Do not overbuild lore delivery. Patron presence should mostly appear through:

- Highlighted board choices
- Card corruption
- Combat style changes
- Short reactive lines
- Special rewards or costs

Patrons provide commentary based on their vibe. A protective Patron should not sound like a hungry Patron. A holy Patron should not frame sacrifice the same way a violent or shadowed Patron does. The goal is quick personality pressure, not long speeches.

Patrons can become slightly aware that something is off across repeated wins. After the player has beaten the game and started again, a Patron might comment with a faint sense of memory, such as recognizing the Host or questioning whether they have worked together before. This should be subtle at first and become more pronounced as meta-progression deepens.

Repeated victories can unlock more Patrons and more Patron commentary/lore. Lore should mostly arrive through reactive lines, altered board comments, unlock text, and rare moments rather than heavy exposition scenes.

## Ground-Up MVP Focus

- Preserve card authoring through `CardData`.
- Rebuild combat from a small, understandable core.
- Keep scene-authored objects in `CombatScene` as the first test surface.
- Add runtime systems only when the data and ownership boundaries are clear.
- Keep UI, combat rules, card data, and run state separate.

## Enemy Concepts

### Shattered Mirror

The Shattered Mirror is a future miniboss concept that tests the player's own turn planning.

Design questions still open:

- Should the Mirror copy card costs, only cards actually played, or also unspent energy?
- Should the Mirror copy exact targets once multi-enemy encounters exist, or always reflect damage into the player?
- Should it copy upgraded/corrupted versions exactly, or create distorted Mirror variants?
- Should the Mirror punish long turns harder, or should it have a cap on copied cards?
