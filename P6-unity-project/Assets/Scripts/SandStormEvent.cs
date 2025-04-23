using System.Collections;
using UnityEngine;

public class SandStormEvent : MonoBehaviour
{
    [Header("Boundary Settings")]
    [Tooltip("Center of play area")]
    public Transform boundaryCenter;
    [Tooltip("Maximum distance from center before triggering teleport")]
    public float maxDistance = 100f;
    [Tooltip("Position to teleport player back to")]
    public Transform respawnPoint;

    [Header("Sandstorm Effect")]
    public GameObject sandstormParticleSystem;
    public float teleportDelay = 2.0f;
    public AudioClip sandstormSound;

    [Header("References")]
    public FirstPersonController playerController;
    public CanvasGroup sandstormOverlay;

    private AudioSource audioSource;

    private void Awake()
    {
        if (boundaryCenter == null)
            boundaryCenter = transform;
            
        if (respawnPoint == null)
            respawnPoint = transform;
            
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && sandstormSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the boundary in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(boundaryCenter.position, maxDistance);
    }

    private void Update()
    {
        if (playerController != null)
        {
            float distanceFromCenter = Vector3.Distance(
                new Vector3(playerController.transform.position.x, boundaryCenter.position.y, playerController.transform.position.z),
                new Vector3(boundaryCenter.position.x, boundaryCenter.position.y, boundaryCenter.position.z));
                
            if (distanceFromCenter > maxDistance)
            {
                StartCoroutine(TeleportPlayerWithSandstorm());
            }
        }
    }

    private IEnumerator TeleportPlayerWithSandstorm()
    {
        // Prevent movement during teleportation
        playerController.UnlockMove(false);
        
        // Activate sandstorm effect
        if (sandstormParticleSystem != null)
            sandstormParticleSystem.SetActive(true);
            
        // Play sound effect
        if (audioSource != null && sandstormSound != null)
            audioSource.PlayOneShot(sandstormSound);
            
        // Fade in sandstorm overlay
        if (sandstormOverlay != null)
            StartCoroutine(FadeOverlay(sandstormOverlay, 0, 1, teleportDelay * 0.5f));
        
        // Wait before teleporting
        yield return new WaitForSeconds(teleportDelay);
        
        // Teleport player
        CharacterController controller = playerController.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            playerController.transform.position = respawnPoint.position;
            playerController.transform.rotation = respawnPoint.rotation;
            controller.enabled = true;
        }
        
        // Wait a bit after teleport
        yield return new WaitForSeconds(teleportDelay * 0.5f);
        
        // Fade out sandstorm overlay
        if (sandstormOverlay != null)
            StartCoroutine(FadeOverlay(sandstormOverlay, 1, 0, teleportDelay * 0.5f));
            
        // Disable sandstorm effect
        if (sandstormParticleSystem != null)
            sandstormParticleSystem.SetActive(false);
            
        // Re-enable movement
        playerController.UnlockMove(true);
    }
    
    private IEnumerator FadeOverlay(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}