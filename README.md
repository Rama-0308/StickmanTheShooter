# Stickman Doodle: Rock Paper Pistols

A 2-player networked stickman shooter built in Unity, where turns are won with Rock-Paper-Scissors and losing a round means your opponent gets to build up their stickman — or shoot yours down.

## How it plays

Each player controls four rows (their own column of stickmen). Every round opens with a Rock-Paper-Scissors throw; whoever wins gets one forced action:

- Click an empty row to draw a face on it
- Click your own row to advance it: **Face → Body → Gun → Bullet**
- Click a loaded (bulleted) row, then an opponent's built row, to shoot it dead

Only rows that have been built up (not empty, not already dead) can be shot. After firing, the shooter's row drops back to **Gun** and has to earn another bullet before it can fire again.

**Win condition:** eliminate all four of your opponent's rows first.

## Tech

- Unity 2D (URP)
- C#
- Unity Netcode for GameObjects — server-authoritative game state
- Unity Relay & Lobby services — matchmaking, public/private lobby browsing, peer connection
- Audio mixing with ducking for SFX (shoot, place, gun click)

## Project structure

- `Assets/Script/` — core game logic (`GameManager`, `GameOverUI`)
- `Assets/LobbyTutorial/` — lobby UI and networking setup (authentication, lobby browsing/creation)
- `Assets/Prefab/` — gameplay and SFX prefabs
- `Assets/Scenes/` — `GameScene` (main gameplay) and the lobby scene

## Status

Complete. Round logic, RPS turn resolution, the full lobby/matchmaking flow, rematch handling, and disconnect recovery are all implemented and working.

## Running it

Open the project in Unity (2D URP template), load the lobby scene, and run two instances (or build + editor) to test host/client play locally.