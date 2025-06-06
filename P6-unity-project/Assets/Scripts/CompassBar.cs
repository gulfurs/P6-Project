using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DirectionMarker
{
    public string name; // for clarity in inspector
    public RectTransform markerTransform;
    public Vector3 direction; // relative direction (like Vector3.forward, etc.)
}

public class CompassBar : MonoBehaviour
{
    public List<DirectionMarker> directionMarkers;

    public RectTransform compassBarTransform;
    public RectTransform objectiveMarkerTransform;
    public Transform cameraObjectTransform;
    public Transform objectiveObjectTransform;
    public GameObject tickPrefab;
    public GameObject objectiveMarkerPrefab;

    private List<RectTransform> activeObjectiveMarkers = new List<RectTransform>();
    private List<Transform> activeObjectiveTargets = new List<Transform>();

    private float refreshTimer = 0f;
    private float refreshInterval = 2f;

    void Start()
    {
        directionMarkers = new List<DirectionMarker>();

        for (int i = 0; i < 360; i += 5)
        {
            GameObject newMarker = Instantiate(tickPrefab, compassBarTransform);
            RectTransform rt = newMarker.GetComponent<RectTransform>();
            Vector3 dir = Quaternion.Euler(0, i, 0) * Vector3.forward;

            string label = "";
            switch (i)
            {
                case 0: label = "N"; break;
                case 45: label = "NE"; break;
                case 90: label = "E"; break;
                case 135: label = "SE"; break;
                case 180: label = "S"; break;
                case 225: label = "SW"; break;
                case 270: label = "W"; break;
                case 315: label = "NW"; break;
            }

            var text = rt.GetComponentInChildren<TextMeshProUGUI>();

            if (!string.IsNullOrEmpty(label))
            {
                text.text = label;
                text.fontSize = 36; // Bigger for cardinal
            }
            else if (i % 15 == 0)
            {
                text.text = "•";
                text.fontSize = 24; // Medium tick
            }
            else
            {
                text.text = "|";
                text.fontSize = 18; // Tiny tick
            }

            directionMarkers.Add(new DirectionMarker
            {
                name = label == "" ? i.ToString() : label,
                markerTransform = rt,
                direction = dir
            });
        }
    }



    void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshInterval)
        {
            RefreshObjectives();
            refreshTimer = 0f;
        }


        foreach (var marker in directionMarkers)
        {
            Vector3 worldPos = cameraObjectTransform.position + marker.direction * 1000;
            SetMarkerPosition(marker.markerTransform, worldPos);
        }

        for (int i = 0; i < activeObjectiveMarkers.Count; i++)
        {
            if (activeObjectiveTargets[i] == null) continue; 
            SetMarkerPosition(activeObjectiveMarkers[i], activeObjectiveTargets[i].position);
        }


    }

    private void RefreshObjectives()
    {
        // Clear old ones
        foreach (var m in activeObjectiveMarkers)
            Destroy(m.gameObject);

        activeObjectiveMarkers.Clear();
        activeObjectiveTargets.Clear();

        Outline[] outlines = GameObject.FindObjectsOfType<Outline>();
        foreach (var outline in outlines)
        {
            if (outline.enabled)
            {
                GameObject newMarker = Instantiate(objectiveMarkerPrefab, compassBarTransform);
                RectTransform rt = newMarker.GetComponent<RectTransform>();
                activeObjectiveMarkers.Add(rt);
                activeObjectiveTargets.Add(outline.transform);
            }
        }
    }


    private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
    {
        Vector3 directionToTarget = worldPosition - cameraObjectTransform.position;
        float signedAngle = Vector3.SignedAngle(new Vector3(cameraObjectTransform.forward.x, 0, cameraObjectTransform.forward.z), new Vector3(directionToTarget.x, 0, directionToTarget.z), Vector3.up);

        float compassPosition = Mathf.Clamp(signedAngle / Camera.main.fieldOfView, -0.5f, 0.5f);
        markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width * compassPosition, 0);

        // Fade logic
        float fadeStart = 0.4f;
        float fadeEnd = 0.5f;
        float absPos = Mathf.Abs(compassPosition);

        float alpha = absPos > fadeStart
            ? Mathf.InverseLerp(fadeEnd, fadeStart, absPos)
            : 1f;

        var tmp = markerTransform.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }


        markerTransform.gameObject.SetActive(alpha > 0.01f);
    }

}