using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;

public class PerlinNoise : MonoBehaviour
{
    private float scale = 5;
    public int radius = 10;
    [SerializeField] private float noiseModifier;
    private SpawnEnemies spawnScript;
    bool lastRoomWasShop = false;

    [SerializeField] Tilemap tileMap;
    [SerializeField] TileBase floor;
    [Header("NavMesh")]
    [SerializeField] NavMeshSurface surface;

    Sounds soundScript;
    private bool isPlayingBattleMusic = false;

    private void Start()
    {
        soundScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<Sounds>();
        spawnScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<SpawnEnemies>();
    }

    public async void StartGenerating()
    {
        await Generate();
    }

    async Task Generate()
    {
        int whatToSpawn = UnityEngine.Random.Range(0, 4);

        if (whatToSpawn == 0 && spawnScript.enemyLevel > 4 && !lastRoomWasShop)
        {
            isPlayingBattleMusic = false;
            soundScript.PlayMusic(0);
            spawnScript.isSpawningShop = true;
            spawnScript.SpawnShop();
            lastRoomWasShop = true;
            spawnScript.SpawnWave(true);
        }
        else
        {
            if (spawnScript.enemyLevel == 0)
            {
                radius = 10;
                noiseModifier = 1f;
            }
            else if (spawnScript.enemyLevel <= 3)
            {
                radius = 15;
                noiseModifier = 0.87f;
            }
            else if (spawnScript.enemyLevel <= 5)
            {
                radius = 20;
                noiseModifier = 0.86f;
            }
            else if (spawnScript.enemyLevel >= 6)
            {
                if (whatToSpawn == 1)
                {
                    radius = 15;
                    noiseModifier = 1f;
                }
                else
                {
                    radius = 25;
                    noiseModifier = 0.85f;
                }
            }

            float offSetX = UnityEngine.Random.Range(0f, 1000f);
            float offSetY = UnityEngine.Random.Range(0f, 1000f);

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double distance = x * x + y * y;
                    if (distance <= Math.Pow(radius, 2))
                    {
                        float i = (float)x / (float)radius * 2f * scale + offSetX;
                        float j = (float)y / (float)radius * 2f * scale + offSetY;

                        float noise = Mathf.PerlinNoise((float)i, (float)j);

                        if (noise < noiseModifier)
                        {
                            tileMap.SetTile(new Vector3Int(x, y), floor);
                        }
                        else
                        {
                            tileMap.SetTile(new Vector3Int(x, y), null);
                        }
                    }
                    else
                    {
                        tileMap.SetTile(new Vector3Int(x, y), null);
                    }
                }

                await Task.Yield();
            }

            await BuildNavMeshAsync();

            if (whatToSpawn == 1 && spawnScript.enemyLevel >= 6)
            {
                if (!isPlayingBattleMusic)
                {
                    soundScript.PlayMusic(1);
                    isPlayingBattleMusic = true;
                }
                spawnScript.isSpawningShop = false;
                spawnScript.SpawnBoss();
                lastRoomWasShop = false;
            }
            else
            {
                if (!isPlayingBattleMusic)
                {
                    soundScript.PlayMusic(1);
                    isPlayingBattleMusic = true;
                }
                spawnScript.isSpawningShop = false;
                spawnScript.SpawnEnemyWave();
                lastRoomWasShop = false;
            }
            spawnScript.SpawnWave(true);
        }
    }

    private async Task BuildNavMeshAsync()
    {
        surface.BuildNavMesh();
        await Task.Yield();
    }
}
