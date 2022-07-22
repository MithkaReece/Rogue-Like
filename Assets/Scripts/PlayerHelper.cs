using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    
    public void DisplaySword1(int par){player.DisplaySword1(par);}
    public void DisplaySword2(int par){player.DisplaySword2(par);}
    public void DisplaySword3(int par){player.DisplaySword3(par);}
    public void DisplayNothing(int par){player.DisplayNothing(par);}
}
