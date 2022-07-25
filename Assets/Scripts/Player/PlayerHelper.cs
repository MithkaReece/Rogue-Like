using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public void DisplaySword(int par) { player.DisplaySword(par); }
    public void DisplayNothing(int par) { player.DisplayNothing(par); }
    public void CanSecondAttack(int par) { player.CanSecondAttack(par); }
    public void SecondAttack(int par) { player.SecondAttack(par); }


    public void EndRoll(int par) { player.EndRoll(par); }
}
