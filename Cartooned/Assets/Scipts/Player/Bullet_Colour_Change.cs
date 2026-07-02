using UnityEngine;

public class Bullet_Colour_Change : MonoBehaviour
{

    public Material[] colourCartoonRange;
    public int randomNumber;
    public int max;
    public int min;

    //public LayerMask mask;

    private void Awake()
    {
        max = colourCartoonRange.Length;
        randomNumber = Random.Range(min, max);
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer mr = other.gameObject.GetComponent<MeshRenderer>();
        //Debug.Log(mr);
       mr.material = colourCartoonRange[randomNumber];
    }
}
