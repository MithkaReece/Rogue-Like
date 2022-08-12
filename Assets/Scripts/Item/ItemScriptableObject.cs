using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemModifier", order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public GameObject itemPrefab;

    [SerializeField]
    public List<ModifierGenerator> modifiers;
}