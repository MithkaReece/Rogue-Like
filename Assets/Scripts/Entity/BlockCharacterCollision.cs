using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Prevents collision between two colliders
// Used because entities use multiple colliders that
// shouldn't interact with each other
public class BlockCharacterCollision : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D characterCollider;
    [SerializeField] private CapsuleCollider2D characterBlockerCollider;

    void Start()
    {
        Physics2D.IgnoreCollision(characterCollider, characterBlockerCollider, true);
    }

}
