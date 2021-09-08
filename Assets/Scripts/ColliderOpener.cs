using ZombieFight.Interfaces.Core;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderOpener : MonoBehaviour, IColliderOpener
{
    BoxCollider boxCollider;
    bool isHit = false;

    public bool IsOpen => boxCollider.isTrigger;
    public bool IsHit => isHit;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject coll = other.gameObject;
        int playerLayer = 3;
        float openColliderTime = 1;
        if (coll.layer == playerLayer)
        {
            isHit = true;
            CloseCollider();
            //Invoke(nameof(OpenCollider), openColliderTime);
            Invoke(nameof(ChangeState), openColliderTime);
        }
    }

    private void ChangeState()
    {
        isHit = false;
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
