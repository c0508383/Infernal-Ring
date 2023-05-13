using UnityEngine;

namespace TeamRitual.UI {
public class UIComponent : MonoBehaviour
{
    protected Vector3 originalPosition;
    protected int ticks = 0;
    private bool reachedDest = false;

    public float positionLerp = 2f;
    public bool setDestinations = false;
    public float destX = 0, destY = 0;

    // Start is called before the first frame update
    public virtual void Start()
    {
        this.originalPosition = this.transform.localPosition;
        if (!setDestinations) {
            this.destX = this.PosX();
            this.destY = this.PosY();
        }
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        Vector3 destPosition = new Vector3(this.destX, this.destY, this.transform.localPosition.z);        
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, destPosition, this.positionLerp*Time.deltaTime);
        if (DistanceToDest() > 2.5D) {
            this.reachedDest = false;
        } else {
            this.reachedDest = true;
        }
        this.ticks++;
    }

    public float PosX() {
        return this.transform.localPosition.x;
    }

    public float PosY() {
        return this.transform.localPosition.y;
    }

    public double DistanceToDest() {
        Vector3 localPos = this.transform.localPosition;
        Vector3 destPosition = new Vector3(this.destX, this.destY, this.transform.localPosition.z);
        return Mathf.Sqrt(Mathf.Pow(localPos.x - destPosition.x,2) + Mathf.Pow(localPos.y - destPosition.y,2) + Mathf.Pow(localPos.z - destPosition.z,2));
    }

    public bool ReachedDest() {
        return this.reachedDest;
    }
}
}