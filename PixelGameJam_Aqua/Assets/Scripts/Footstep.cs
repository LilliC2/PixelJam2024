using UnityEngine;
using DG.Tweening;

public class Footstep : GameBehaviour
{
    [SerializeField] float duration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOScale(0f, duration);
        ExecuteAfterSeconds(duration, () => Destroy(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
