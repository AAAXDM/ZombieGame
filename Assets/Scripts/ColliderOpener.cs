using ZombieFight.Interfaces.Core;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderOpener : MonoBehaviour, IColliderOpener
{
    BoxCollider boxCollider;


    public bool IsOpen => boxCollider.isTrigger; 
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }

    public void OpenCollider()
    {
        boxCollider.isTrigger = true;
    }

    public void CloseCollider()
    {
        boxCollider.isTrigger = false;
    }
}
