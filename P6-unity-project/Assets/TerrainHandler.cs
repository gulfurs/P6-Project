using UnityEngine;
using System.Collections;

public class TerrainHandler : MonoBehaviour
{
    public Transform targetPlayer;

    void Start()
    {
        StartCoroutine(UpdatePositionRoutine());
    }

    IEnumerator UpdatePositionRoutine()
    {
        while (true)
        {
            if (targetPlayer != null)
            {
                Vector3 newPosition = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z);
                transform.position = newPosition;
                Debug.Log("YAH!");
            }
            yield return new WaitForSeconds(10f);
        }
    }
}
