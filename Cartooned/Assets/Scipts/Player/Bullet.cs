using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletSpeed;

    [Header("Other Scripts")]
    //public PlayerController playerController;

    //public Transform shootPt;

    //public Material[] colourCartoonRange;
    
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
     
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("HIT");
        Destroy(this.gameObject);
    }

}
