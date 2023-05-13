using UnityEngine;

namespace TeamRitual.Stage {
public class StageRuin_Toast : MonoBehaviour
{
    int ticks = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("StageRuin_ToastBlink", -1, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.ticks++;
        if (this.ticks%1000 == 0 && Random.Range(0,10) >= 5f) {
            GetComponent<Animator>().Play(Random.Range(0,10) >= 5f ? "StageRuin_ToastBlink" : "StageRuin_ToastScratch", -1, 0f);
        }
    }
}
}
