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
    public float teleportDelay = 2.0f;
    public AudioClip sandstormSound;
    [Range(0.5f, 2f)]
    public float fadeInSpeed = 1f;
    [Range(0.5f, 2f)]
    public float fadeOutSpeed = 1f;

    [Header("References")]
    public FirstPersonController playerController;
    public CanvasGroup sandstormOverlay;

    private AudioSource audioSource;
    private bool isTeleporting = false;

    private void Awake()
    {
        // Initialize boundary settings
        if (boundaryCenter == null)
            boundaryCenter = transform;
            
        if (respawnPoint == null)
            respawnPoint = transform;
            
        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && sandstormSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // Make it 2D sound
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
        }
            
        // Initialize UI overlay settings
        if (sandstormOverlay != null)
        {
            sandstormOverlay.alpha = 0;
            sandstormOverlay.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the boundary in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(boundaryCenter.position, maxDistance);
    }

    private void Update()
    {
        if (playerController != null && !isTeleporting)
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
        if (isTeleporting)
            yield break;  // Prevent multiple teleportations
            
        isTeleporting = true;
            
        // Play sound effect
        if (audioSource != null && sandstormSound != null)
        {
            audioSource.clip = sandstormSound;
            audioSource.Play();
        }
            
        // Fade in sandstorm overlay
        if (sandstormOverlay != null)
        {
            sandstormOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(FadeOverlay(sandstormOverlay, 0, 1, teleportDelay * 0.5f * (1/fadeInSpeed)));
        }
        
        // Wait before teleporting
        yield return new WaitForSeconds(teleportDelay);
        
        // Teleport player with CharacterController handling
        CharacterController controller = playerController.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            playerController.transform.position = respawnPoint.position;
            playerController.transform.rotation = respawnPoint.rotation;
            controller.enabled = true;
        }
        else
        {
            // Fallback direct teleport
            playerController.transform.position = respawnPoint.position;
            playerController.transform.rotation = respawnPoint.rotation;
        }
        
        // Wait a bit after teleport
        yield return new WaitForSeconds(teleportDelay * 0.5f);
        
        // Fade out sandstorm overlay
        if (sandstormOverlay != null)
        {
            yield return StartCoroutine(FadeOverlay(sandstormOverlay, 1, 0, teleportDelay * 0.5f * (1/fadeOutSpeed)));
            sandstormOverlay.gameObject.SetActive(false);
        }
        
        isTeleporting = false;
    }
    
    private IEnumerator FadeOverlay(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0;
        
        // Set starting value immediately
        canvasGroup.alpha = start;
        
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we reach the exact end value
        canvasGroup.alpha = end;
    }
}