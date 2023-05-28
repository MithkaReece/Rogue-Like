using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject basePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //SpawnSlime(transform.position);
        SpawnBase(transform.position);
        SpawnBase(transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO: Make generic spawn func

    void SpawnSlime(Vector2 spawnPosition)
    {
        GameObject slime = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        EnemyController controller = slime.GetComponent<EnemyController>();
        controller.player = player;
    }

    void SpawnBase(Vector2 spawnPosition)
    {
        GameObject baseEnemy = Instantiate(basePrefab, spawnPosition, Quaternion.identity);
        EnemyController controller = baseEnemy.GetComponent<EnemyController>();
        controller.player = player;
    }
}
