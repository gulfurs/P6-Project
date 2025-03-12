using UnityEngine;

public class Container : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Destroy(other.gameObject);
            ObjectiveManager.Instance.UpdateObjectiveProgress("Put objects in container", 1);
        }
    }
}
