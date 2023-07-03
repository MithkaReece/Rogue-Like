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
        EntityStats entityStats = playerController.entityStats;
        SetupNewPlayerStats(playerController);

        GameObject cameraInst = Instantiate(cameraPrefab);
        cameraInst.GetComponent<CameraController>().player = playerInst.GetComponent<PlayerController>();


        //TODO: TEMPORARY
        Inventory inv = playerInst.GetComponent<Inventory>();
        inv.PrintItems();
        playerController.ReceiveItem(new Weapon("Crude Long Sword", 10, 0.1f, 10f, "Attack1"));
        inv.PrintItems();
    }
    //TODO
    void SetupPlayerStats(EntityStats entityStats)
    {
        //Either
        //retrieve from saved
        //generate from new
        //SetupNewPlayerStats(entityStats);
    }

    void SetupNewPlayerStats(PlayerController playerController)
    {
        playerController.entityStats = new EntityStats(100f,
            100f, 20f, 2f,
            3f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
