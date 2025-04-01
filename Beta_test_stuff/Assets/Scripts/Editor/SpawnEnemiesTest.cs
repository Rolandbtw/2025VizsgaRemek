using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SpawnEnemiesTest : MonoBehaviour
{
    private GameObject spawnEnemiesObject;
    private SpawnEnemies spawnEnemies;
    private GameObject dummyPlayer;
    private TextMeshProUGUI dummyWaveText;
    private GameObject dummyWeaponsUI;
    private Image dummyCircleWipeImage;
    private GameObject dummyShop;
    private GameObject dummyStarterRoom;
    private ShopKeeper dummyShopKeeper;
    private Tilemap dummyWalkableTilemap;
    private Tilemap dummyDecorationsTilemap;
    private Grid dummyGrid;
    private PerlinNoise dummyMapGenerator;

    [SetUp]
    public void SetUp()
    {
        dummyPlayer = new GameObject("Player");
        dummyPlayer.tag = "Player";
        dummyPlayer.AddComponent<Rigidbody2D>();
        dummyPlayer.AddComponent<PlayerMovement>();
        dummyPlayer.AddComponent<PlayerHealth>();

        dummyWeaponsUI = new GameObject("WeaponsUI");
        dummyCircleWipeImage = new GameObject("CircleWipe").AddComponent<Image>();
        dummyWaveText = new GameObject("WaveText").AddComponent<TextMeshProUGUI>();
        dummyShop = new GameObject("Shop");
        dummyStarterRoom = new GameObject("StarterRoom");
        dummyShopKeeper = new GameObject("ShopKeeper").AddComponent<ShopKeeper>();

        dummyGrid = new GameObject("Grid").AddComponent<Grid>();
        dummyWalkableTilemap = new GameObject("WalkableTilemap").AddComponent<Tilemap>();
        dummyDecorationsTilemap = new GameObject("DecorationsTilemap").AddComponent<Tilemap>();

        dummyMapGenerator = new GameObject("MapGenerator").AddComponent<PerlinNoise>();

        spawnEnemiesObject = new GameObject("SpawnEnemies");
        spawnEnemies = spawnEnemiesObject.AddComponent<SpawnEnemies>();

        SetFieldValue("player", dummyPlayer);

        GameObject[] enemiesArray=new GameObject[10];
        for (int i = 0; i < enemiesArray.Length; i++)
        {
            GameObject enemy = new GameObject("Enemy");
            enemy.tag = "Enemy";
            enemiesArray[i] = enemy;
        }
        GameObject[] bossesArray=new GameObject[10];
        for (int i = 0; i < bossesArray.Length; i++)
        {
            GameObject enemy = new GameObject("Boss");
            enemy.tag = "Enemy";
            bossesArray[i] = enemy;
        }
        GameObject[] usableEnemies=new GameObject[10];
        for (int i = 0; i < usableEnemies.Length; i++)
        {
            GameObject enemy = new GameObject("UsableEnemies");
            enemy.tag = "Enemy";
            usableEnemies[i] = enemy;
        }

        GameObject torch = new GameObject("Torch");
        torch.tag = "Clear";
        GameObject column = new GameObject("Column");
        column.tag = "Clear";
        GameObject spawnEffect = new GameObject("SpawnEffect");

        SetFieldValue("spawnEffect", spawnEffect);
        SetFieldValue("torch", torch);
        SetFieldValue("column", column);
        SetFieldValue("usableEnemies", usableEnemies);
        SetFieldValue("enemies", enemiesArray);
        SetFieldValue("bosses", bossesArray);
        SetFieldValue("waveCountText", dummyWaveText);
        SetFieldValue("weaponsUI", dummyWeaponsUI);
        SetFieldValue("circleWipeImage", dummyCircleWipeImage);
        SetFieldValue("shop", dummyShop);
        SetFieldValue("starterRoom", dummyStarterRoom);
        SetFieldValue("shopKeeper", dummyShopKeeper);
        SetFieldValue("walkableTilemap", dummyWalkableTilemap);
        SetFieldValue("decorationsTilemap", dummyDecorationsTilemap);
        SetFieldValue("grid", dummyGrid);
        SetFieldValue("perlinNoiseScript", dummyMapGenerator);
        SetFieldValue("minEnemyCount", 5);
        spawnEnemies.maxEnemyCount = 10;
    }

    [Test]
    public void SpawnBossTest()
    {
        int waveCount = (int)GetFieldValue("waveCount");
        int enemyCount = spawnEnemies.currentEnemiesAlive;
        int minEnemyCount = (int)GetFieldValue("minEnemyCount");
        int maxEnemyCount = spawnEnemies.maxEnemyCount;
        GameObject player = (GameObject)GetFieldValue("player");
        Vector3 playerPos=player.transform.position;

        spawnEnemies.SpawnBoss();

        GameObject boss = GameObject.FindGameObjectWithTag("Enemy");
        player = (GameObject)GetFieldValue("player");

        Assert.AreNotEqual(minEnemyCount, (int)GetFieldValue("minEnemyCount"));
        Assert.AreNotEqual(maxEnemyCount, spawnEnemies.maxEnemyCount);
        Assert.AreNotEqual(playerPos, player.transform.position);
        Assert.IsNotNull(boss);
        Assert.AreNotEqual(waveCount, (int)GetFieldValue("waveCount"));
        Assert.AreNotEqual(enemyCount, spawnEnemies.currentEnemiesAlive);
    }

    [Test]
    public void SpawnEnemyWaveTest()
    {
        int waveCount = (int)GetFieldValue("waveCount");
        int minEnemyCount = (int)GetFieldValue("minEnemyCount");
        int maxEnemyCount = spawnEnemies.maxEnemyCount;
        GameObject player = (GameObject)GetFieldValue("player");
        Vector3 playerPos = player.transform.position;

        spawnEnemies.currentEnemiesAlive = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int beforeCount = enemies.Length;

        spawnEnemies.SpawnEnemyWave();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int afterCount=enemies.Length;

        player = (GameObject)GetFieldValue("player");

        Assert.AreNotEqual(minEnemyCount, (int)GetFieldValue("minEnemyCount"));
        Assert.AreNotEqual(maxEnemyCount, spawnEnemies.maxEnemyCount);
        Assert.AreNotEqual(playerPos, player.transform.position);
        Assert.AreEqual((afterCount-beforeCount), spawnEnemies.currentEnemiesAlive);
        Assert.AreNotEqual(waveCount, (int)GetFieldValue("waveCount"));
    }

    [Test]
    public void SpawnWaveTest()
    {
        spawnEnemies.currentEnemiesAlive = 10;

        spawnEnemies.SpawnWave(true);

        int columns = 0;
        int torches = 0;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Clear");
        foreach (GameObject obj in objects)
        {
            if(obj.name == "Torch(Clone)")
            {
                torches++;
            }
            else if(obj.name=="Column(Clone)")
            {
                columns++;
            }
        }

        Assert.AreEqual(torches, spawnEnemies.currentEnemiesAlive);
        Assert.AreEqual(columns, spawnEnemies.currentEnemiesAlive / 2);
    }

    private void SetFieldValue(string fieldName, object value)
    {
        FieldInfo field = typeof(SpawnEnemies).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(spawnEnemies, value);
    }

    private object GetFieldValue(string fieldName)
    {
        FieldInfo field = typeof(SpawnEnemies).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field.GetValue(spawnEnemies);
    }
}