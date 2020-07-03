using UnityEngine;

public class Block : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        Destroy(gameObject);
    }
}
