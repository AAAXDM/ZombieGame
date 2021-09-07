using UnityEngine;

public class BoxObject : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject blueSpell;
    Box.BoxType boxType;
    int additional;
    #endregion

    #region Properties
    public int Additional => additional;
    public Box.BoxType BoxType => boxType;
    #endregion

    #region Core Methods
    private void Start()
    {
        float lifeTime = 7;
        Invoke(nameof(DestroyBox), lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int bulletLayer = 13;
        GameObject coll = collision.gameObject;
        if (coll.layer == bulletLayer) DestroyBox();
    }
    #endregion

    #region SupportMethods
    public void SetBoxParameters(Box box)
    {
        boxType = box.Type;
        int random = Random.Range(box.AdditionMin, box.AdditionMax + 1);
        additional = random;
    }
    
    public void DestroyBox()
    {
        Instantiate(blueSpell, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    #endregion
}
