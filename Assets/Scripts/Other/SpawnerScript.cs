using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnSlime(transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void SpawnSlime(Vector2 spawnPosition)
    {
        GameObject slime = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        EnemyController controller = slime.GetComponent<EnemyController>();
        controller.SetPlayer((GameObject.Find("Player").GetComponent<PlayerController>()));
    }
}
