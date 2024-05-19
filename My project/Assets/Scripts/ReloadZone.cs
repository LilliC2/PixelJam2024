using UnityEngine;

public class ReloadZone : GameBehaviour
{
    [SerializeField] float minTime, maxTime;
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        ExecuteAfterSeconds(Random.Range(minTime, maxTime),()=> DestroyPuddle());
    }

    void DestroyPuddle()
    {
        anim.SetTrigger("Destroy");

        ExecuteAfterSeconds(1, () => Destroy(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            print("player in trigger");
            other.GetComponent<PlayerController>().isInReloadZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().isInReloadZone = false;
        }
    }
}
