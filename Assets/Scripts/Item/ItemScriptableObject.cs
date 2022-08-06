using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemModifier", order = 1)]
class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public GameObject itemPrefab;

    public List<ModifierGenerator> modifiers;
}