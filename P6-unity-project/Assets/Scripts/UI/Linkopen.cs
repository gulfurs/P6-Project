using UnityEngine;

public class Linkopen : MonoBehaviour
{
    public string url = "https://www.survey-xact.dk/LinkCollector?key=TWSX13M8U11J";

    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
