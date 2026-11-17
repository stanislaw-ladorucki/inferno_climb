using System;
using System.Collections.Generic;
using Godot;
using InfernoClimb.entities;

namespace InfernoClimb.level;

public partial class EnemyManager : Node2D {
    [Signal] public delegate void EnemySpawnedEventHandler(Entity enemy);
    [Signal] public delegate void EnemyDiedEventHandler(Entity enemy);

    [Export] public float FinalEnemyDifficultyHeight = 40_000; 
    [Export] public int StartingEnemySpawnsPerChunk = 0;
    [Export] public int FinalEnemySpawnsPerChunk = 25;
    
    public struct EnemySpawner {
        public delegate Entity[] SpawnHandler(EnemyManager manager, Platform platform);
        public SpawnHandler Spawn;
        public float Weight = 1.0f;

        public EnemySpawner(SpawnHandler spawn, float weight) {
            Spawn = spawn;
            Weight = weight;
        }
    }
    
    public List<EnemySpawner> EnemySpawnerPool = new();
    public Stack<KeyValuePair<int, EnemySpawner>> EnemyPoolUpdateThresholds = new([
        // decara (shoots at the player, standing still on a platform)
        new (28_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            return [manager.SpawnAtPosition(
                GD.Load<PackedScene>("res://scenes/enemies/decara.tscn"),
                platform.GetParent<Node2D>(),
                platform.GlobalPosition + new Vector2(rng.RandfRange(-platform.Size * 0.4f, platform.Size * 0.4f), -100.0f)
            )];
        }, 0.5f)),
        
        // imp (flying and shooting at the player)
        new (14_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            LevelChunk chunk = platform.GetParent<LevelChunk>();
            float horizontalPosition = rng.RandfRange(-chunk.Size.X / 2, chunk.Size.X / 2);
            return [manager.SpawnAtPosition(GD.Load<PackedScene>("res://scenes/enemies/imp.tscn"), platform,
                platform.GlobalPosition with {
                    // spawn on random horizontal position
                    X = horizontalPosition,
                }
            )];
        }, 0.5f)),
        
        
        // gaap! (leaps at the player like a frog, kamikaze style)
        new (10_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            return [manager.SpawnAtPosition(
                GD.Load<PackedScene>("res://scenes/enemies/gaap.tscn"),
                platform.GetParent<Node2D>(),
                platform.GlobalPosition + new Vector2(rng.RandfRange(-platform.Size * 0.4f, platform.Size * 0.4f), -100.0f)
            )];
        }, 0.5f)),
		
        // clump of vasago guys
        new (7_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            PackedScene scene = GD.Load<PackedScene>("res://scenes/enemies/vasago.tscn");
            List<Entity> enemies = new();
            LevelChunk chunk = platform.GetParent<LevelChunk>();
            Vector2 position = new(rng.RandfRange(-chunk.Size.X / 2, chunk.Size.X / 2), platform.GlobalPosition.Y);
            int enemyCount = rng.RandiRange(3, 6); 
            for (int i = 0; i < enemyCount; i++) {
                enemies.Add(manager.SpawnAtPosition(scene, chunk, position + Vector2.FromAngle(rng.Randf() * Mathf.Pi * 2.0f) * rng.Randf() * 175.0f * new Vector2(1.0f, 0.5f)));
            }
            return enemies.ToArray();
        }, 0.25f)),
        
        // agares
        new (6_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            return [manager.SpawnAtPosition(
                GD.Load<PackedScene>("res://scenes/enemies/agares.tscn"),
                platform.GetParent<Node2D>(),
                platform.GlobalPosition + new Vector2(rng.RandfRange(-platform.Size * 0.4f, platform.Size * 0.4f), -100.0f)
            )];
        }, 1.0f)),
        
        
        // buer (falling down at the player)
        new (3_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            LevelChunk chunk = platform.GetParent<LevelChunk>();
            float horizontalPosition = rng.RandfRange(-chunk.Size.X / 2, chunk.Size.X / 2);
            return [manager.SpawnAtPosition(GD.Load<PackedScene>("res://scenes/enemies/buer.tscn"), platform,
                platform.GlobalPosition with {
                    // spawn on random horizontal position
                    X = horizontalPosition,
                }
            )];
        }, 0.5f)),
        
        // vasago (just floating about)
        new (1_000, new((manager, platform) => {
            using RandomNumberGenerator rng = new();
            LevelChunk chunk = platform.GetParent<LevelChunk>();
            float horizontalPosition = rng.RandfRange(-chunk.Size.X / 2, chunk.Size.X / 2);
            return [manager.SpawnAtPosition(GD.Load<PackedScene>("res://scenes/enemies/vasago.tscn"), platform,
                platform.GlobalPosition with {
                    // spawn on random horizontal position
                    X = horizontalPosition,
                }
            )];
        }, 1.0f)),
    ]);

    public Entity SpawnAtPlatform(PackedScene scene, Platform platform) {
        return SpawnAtPosition(scene, platform.GetParent<Node2D>(), platform.GlobalPosition);
    }

    public Entity SpawnAtPosition(PackedScene scene, Node2D parent, Vector2 position) {
        Entity enemy = scene.Instantiate<Entity>();   
        parent.AddChild(enemy);
        enemy.GlobalPosition = position;
        
        EmitSignalEnemySpawned(enemy);
        enemy.Died += () => EmitSignalEnemyDied(enemy);
        
        return enemy;
    }

    public void UpdateEnemySpawnerPool(float height) {
        // update enemy pool after reaching a threshold
        if (EnemyPoolUpdateThresholds.Count > 0 && EnemyPoolUpdateThresholds.Peek().Key <= height) {
            EnemySpawnerPool.Add(EnemyPoolUpdateThresholds.Pop().Value);
        }
    }

    public void PopulateEnemies(LevelChunk chunk, Platform[] platforms) {
        using RandomNumberGenerator rng = new RandomNumberGenerator();
        
        UpdateEnemySpawnerPool(-chunk.Position.Y);
        float difficulty = chunk.Position.Y == 0 ? 0 : Mathf.Min(chunk.Position.Y / -FinalEnemyDifficultyHeight, 1.0f);
        float[] spawnerWeights = Array.ConvertAll(EnemySpawnerPool.ToArray(), spawner => spawner.Weight);
        
        EnemySpawner.SpawnHandler[] spawnerHandlers = Array.ConvertAll(EnemySpawnerPool.ToArray(), spawner => spawner.Spawn);
        if (EnemySpawnerPool.Count > 0) {
            for (int i = 0; i < Mathf.Lerp(StartingEnemySpawnsPerChunk, FinalEnemySpawnsPerChunk, difficulty); i++) {
                Platform platform = platforms[rng.RandiRange(0, platforms.Length - 1)];
                spawnerHandlers[rng.RandWeighted(spawnerWeights)].Invoke(this, platform);
            }
        }
    }
}