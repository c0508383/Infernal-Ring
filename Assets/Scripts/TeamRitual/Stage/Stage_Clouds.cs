using UnityEngine;

namespace TeamRitual.Stage {
public class Stage_Clouds : MonoBehaviour
{
    public float speed = 1f;
    public float minBounds = -32f;
    public float maxBounds = 40f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(this.transform.position.x - this.speed/1000f, this.transform.position.y, this.transform.position.z);

        if (this.transform.position.x <= minBounds) {
            this.transform.position = new Vector3(maxBounds, this.transform.position.y, this.transform.position.z);
        } else if (this.transform.position.x >= maxBounds) {
            this.transform.position = new Vector3(minBounds, this.transform.position.y, this.transform.position.z);
        }
    }
}
}