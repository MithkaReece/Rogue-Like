using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerInst = Instantiate(playerPrefab, transform.position, transform.rotation);
        PlayerController playerController = playerInst.GetComponent<PlayerController>();
        EntityStats entityStats = playerInst.GetComponent<EntityStats>();
        SetupNewPlayerStats(entityStats);

        GameObject cameraInst = Instantiate(cameraPrefab);
        cameraInst.GetComponent<CameraController>().player = playerInst.GetComponent<PlayerController>();


        //TODO: TEMPORARY
        Inventory inv = playerInst.GetComponent<Inventory>();
        inv.PrintItems();
        inv.AddItem(new Weapon("Crude Long Sword", 10, 0.1f));
        inv.PrintItems();
    }

    void SetupPlayerStats(EntityStats entityStats)
    {
        //Either
        //retrieve from saved
        //generate from new
        SetupNewPlayerStats(entityStats);
    }

    void SetupNewPlayerStats(EntityStats entityStats)
    {

        entityStats.Poise = 100f;
        entityStats.ReposRegenSpeed = 20f;
        entityStats.ReposCooldown = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}