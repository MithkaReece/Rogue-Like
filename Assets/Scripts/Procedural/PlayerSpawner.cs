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
        playerController.Name = "Player";
        EntityStats entityStats = playerInst.GetComponent<EntityStats>();
        SetupNewPlayerStats(entityStats);

        GameObject cameraInst = Instantiate(cameraPrefab);
        cameraInst.GetComponent<CameraController>().player = playerInst.GetComponent<PlayerController>();


        //TODO: TEMPORARY
        Inventory inv = playerInst.GetComponent<Inventory>();
        inv.PrintItems();
        playerController.ReceiveItem(new Weapon("Crude Long Sword", 10, 0.1f, 10f, "Attack1"));
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

        entityStats.MoveSpeed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
