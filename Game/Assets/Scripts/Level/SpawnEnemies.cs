using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Spawnable objects and effects")]
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] bosses;
    [SerializeField] GameObject chest;
    [SerializeField] GameObject mimic;
    [SerializeField] GameObject torch;
    [SerializeField] GameObject column;
    [SerializeField] GameObject spawnEffect;

    [Header("Wave spawn varriables and settings")]
    private GameObject[] usableEnemies;
    public int enemyLevel; // needs to be accessed
    [SerializeField] int minEnemyCount;
    public int maxEnemyCount; // needs to be accessed
    public int currentEnemiesAlive; // needs to be accessed
    [SerializeField] float spawnRange;

    [Header("Map generation variables")]
    [SerializeField] PerlinNoise perlinNoiseScript;
    [SerializeField] Tilemap walkableTilemap;
    [SerializeField] Tilemap decorationsTilemap;
    [SerializeField] Grid grid;

    [Header("Circle wipe transition varriables")]
    [SerializeField] Image circleWipeImage;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    [SerializeField] float moveDuration;
    [SerializeField] GameObject weaponsUI;

    [Header("Other objects")]
    [SerializeField] GameObject portal;
    [SerializeField] GameObject starterRoom;
    [SerializeField] GameObject shop;
    [SerializeField] ShopKeeper shopKeeper;
    [SerializeField] TextMeshProUGUI waveCountText;
    private int waveCount = 0;
    public bool isInTransition = false;

    GameObject player;
    bool roomIsReady = false;
    public bool isSpawningShop=false;
    Vector3 portalPosition;

    void Start()
    {
        Time.timeScale = 1;
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(NewLevelTransition(endPos, startPos, moveDuration, true));
    }

    void Update()
    {
        if(currentEnemiesAlive <= 0 && roomIsReady && !isSpawningShop)
        {
            SpawnPortalAndChest();
            roomIsReady = false;
        }
    }

    public void NewRoom(Vector3 position) //needs to be accessed
    {
        portalPosition = position;
        StartCoroutine(NewLevelTransition(startPos, endPos, moveDuration, false));
        player.GetComponent<Runes>().NewRoom();
    }

    public void SpawnWave(bool shouldSpawnObjects) //needs to be accessed
    {
        if (shouldSpawnObjects)
        {
            SpawnTorches();
            SpawnColumns();
        }
        StartCoroutine(NewLevelTransition(endPos, startPos, moveDuration, true));
        roomIsReady = true;
    }

    public void SpawnEnemyWave()
    {
        waveCount++;
        PlayerPrefs.SetInt("waves", waveCount);
        waveCountText.text = "wave: " + waveCount;
        minEnemyCount++;
        maxEnemyCount+=3;

        if (enemyLevel < enemies.Length)
        {
            enemyLevel++;
        }

        Vector3 point = Vector3.zero;
        int enemyCount = Random.Range(minEnemyCount, maxEnemyCount);
        usableEnemies = enemies.Take(enemyLevel).ToArray();
        for (int i = 0; i < enemyCount; i++)
        {
            point = RandomPoint(GetTilemapCenter(), spawnRange);
            int enemyType = Random.Range(0, usableEnemies.Length);
            Instantiate(usableEnemies[enemyType], point, usableEnemies[enemyType].transform.rotation);
            currentEnemiesAlive++;
        }
        point = RandomPoint(GetTilemapCenter(), 5);
        player.transform.position = point+new Vector3(0, 1, 0);
    }

    public void SpawnBoss()
    {
        waveCount++;
        waveCountText.text = "wave: " + waveCount;
        minEnemyCount++;
        maxEnemyCount++;

        if (enemyLevel < enemies.Length)
        {
            enemyLevel++;
        }

        Vector3 point= Vector3.zero;
        point = RandomPoint(GetTilemapCenter(), spawnRange);
        int bossIndex = Random.Range(0, bosses.Length);
        Instantiate(bosses[bossIndex], point, bosses[bossIndex].transform.rotation);
        currentEnemiesAlive++;

        point = RandomPoint(GetTilemapCenter(), 10);
        player.transform.position = point + new Vector3(0, 1, 0);
    }

    public void SpawnShop()
    {
        StartCoroutine(ClearMap(false));
        shopKeeper.RefreshShop();
        shop.SetActive(true);
        player.transform.position = new Vector3(2, 20, 0);
    }

    void SpawnTorches()
    {
        Vector3 point = Vector3.zero;

        int spawnNumber = currentEnemiesAlive;
        if (spawnNumber > 15)
        {
            spawnNumber = 15;
        }

        for (int i = 0; i < spawnNumber; i++)
        {
            point=RandomPoint(GetTilemapCenter(), perlinNoiseScript.radius);
            Vector3Int cellPosition = grid.WorldToCell(point);
            point = grid.GetCellCenterWorld(cellPosition);
            Collider2D collider = Physics2D.OverlapPoint(point);
            if (collider != null && !collider.CompareTag("Clear"))
            {
                Instantiate(torch, point, torch.transform.rotation);
            }
        }
    }

    void SpawnColumns()
    {
        Vector3 point = Vector3.zero;

        int spawnNumber = currentEnemiesAlive / 2;

        if (spawnNumber > 20)
        {
            spawnNumber = 20;
        }

        for (int i = 0; i < spawnNumber; i++)
        {
            point = RandomPoint(GetTilemapCenter(), perlinNoiseScript.radius/1.25f);
            Vector3Int cellPosition = grid.WorldToCell(point);
            point = grid.GetCellCenterWorld(cellPosition);
            point=new Vector2(point.x, point.y-0.4f);
            Collider2D collider = Physics2D.OverlapPoint(point);
            if (collider != null && !collider.CompareTag("Clear"))
            {
                Instantiate(column, point, torch.transform.rotation);
            }
        }
    }

    void SpawnPortalAndChest()
    {
        Vector3 point;
        point = RandomPoint(GetTilemapCenter(), 10);
        Instantiate(portal, point, portal.transform.rotation);

        int isRareChest = Random.Range(0, 3);

        if (isRareChest == 0)
        {
            point = RandomPoint(GetTilemapCenter(), 10);
            Vector3Int cellPosition = grid.WorldToCell(point);
            point = grid.GetCellCenterWorld(cellPosition);
            GameObject chestClone = Instantiate(chest, point, portal.transform.rotation);
            chestClone.GetComponent<Chest>().isRare = true;
            cellPosition = grid.WorldToCell(point);
            point = grid.GetCellCenterWorld(cellPosition);
            point = RandomPoint(GetTilemapCenter(), 10);
            Instantiate(mimic, point, portal.transform.rotation);
        }
        else
        {
            for (int i = 0; i < Random.Range(1, 5); i++)
            {
                point = RandomPoint(GetTilemapCenter(), 10);
                Vector3Int cellPosition = grid.WorldToCell(point);
                point = grid.GetCellCenterWorld(cellPosition);
                Instantiate(chest, point, portal.transform.rotation);
            }
        }
    }

    IEnumerator ClearMap(bool shouldCreateNewMap)
    {
        starterRoom.SetActive(false);
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Clear");
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }
        walkableTilemap.ClearAllTiles();
        yield return null;
        decorationsTilemap.ClearAllTiles();
        yield return null;
        if (shouldCreateNewMap)
        {
            perlinNoiseScript.StartGenerating();
        }
    }

    IEnumerator NewLevelTransition(Vector3 startPos, Vector3 endPos, float duration, bool isEndingLevel)
    {
        isInTransition = true;

        // disabling enemies until transition is done
        GameObject[] enemies = null;
        if (isEndingLevel && !isSpawningShop)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(false);
            }
        }

        // stopping player from moving during transition and disabling weapon UI
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        weaponsUI.SetActive(false);

        // setting varriables needed for the player transition animation
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Quaternion startRotation = player.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, 180);
        Quaternion camTargetRotation = startRotation;
        Vector3 startSize=player.transform.localScale;
        Vector3 startPosition = player.transform.position;


        // using lerp to make the transition
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            circleWipeImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
            // using lerp to make the player transition animation
            if (!isEndingLevel)
            {
                player.transform.localScale = Vector3.Lerp(startSize, new Vector3(0.01f, 0.01f, 0.01f), timeElapsed / duration);
                player.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
                player.transform.position = Vector3.Lerp(startPosition, portalPosition, timeElapsed / duration);
                camera.rotation= Quaternion.Lerp(startRotation, camTargetRotation, timeElapsed / duration);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // clearing the map if it is the end of the level
        if(!isEndingLevel)
        {
            shop.SetActive(false);
            StartCoroutine(ClearMap(true));
        }
        // or enabling all enemies
        else if(!isSpawningShop)
        {
            foreach (GameObject enemy in enemies)
            {
                if(enemy!= null)
                {
                    enemy.SetActive(true);
                    Instantiate(spawnEffect, enemy.transform.position, spawnEffect.transform.rotation);
                }
            }
        }

        // setting the final size and position of the player
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        camera.rotation = Quaternion.Euler(0, 0, 0);
        player.transform.localScale = startSize;

        circleWipeImage.rectTransform.anchoredPosition = endPos;

        // enabling player movement and weapon UI
        weaponsUI.SetActive(true);
        player.GetComponent<PlayerMovement>().enabled = true;
        isInTransition = false;
    }

    public IEnumerator DeathCircleWipe()
    {
        float timeElapsed = 0;
        while (timeElapsed < moveDuration)
        {
            circleWipeImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public Vector3 GetTilemapCenter()
    {
        BoundsInt bounds = walkableTilemap.cellBounds;
        Vector3Int cellCenter = new Vector3Int(
            bounds.xMin + bounds.size.x / 2,
            bounds.yMin + bounds.size.y / 2,
            0
        );
        Vector3 worldCenter = walkableTilemap.CellToWorld(cellCenter);
        return worldCenter;
    }

    public Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 1000, NavMesh.AllAreas);
        return hit.position;
    }
}
