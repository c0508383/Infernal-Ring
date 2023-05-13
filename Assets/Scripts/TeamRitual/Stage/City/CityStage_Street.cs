using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Stage {
public class CityStage_Street : MonoBehaviour
{
	private Vector2 originalPosition;

	private void Start() {
        this.originalPosition = (Vector2) this.transform.position;
    }

	private void Update() {
        this.transform.position = (Vector3) new Vector2(this.originalPosition.x, this.originalPosition.y + Mathf.Clamp(GameController.Instance.GetCameraY() * 1.3f, 0.0f, 5f));
    }
}
}