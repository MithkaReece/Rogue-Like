using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    public static DataPersistenceManager instance { get; private set; }
    private GameData gameData;
    private List<IDataPersistence> dataPesistenceObjects;
    private FileDataHandler dataHandler;


    private void Awake()
    {
        if (instance)
            Debug.Log("Multiple data persistance manager somehow???");
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPesistenceObjects = FindAllDataPersistenceObjects();
        gameData = new GameData();
        //LoadGame();
    }

    public void NewGame()
    {

    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPesistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("new game");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPesistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
