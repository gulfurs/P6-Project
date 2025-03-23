using UnityEngine;

public class CrabInteract : InteractHandler
{
    public GameObject commandUIPrefab; // Assign in Inspector
    public LogManager logman;

    private GameObject currentUI; // Stores the instantiated UI

    void Start()
    {
        logman = FindObjectOfType<LogManager>();
    }

    public override void InteractLogic()
    {
        if (currentUI == null) 
        {
            currentUI = Instantiate(commandUIPrefab);
        }

        logman.ToggleLogMenu(true);

        //base.InteractLogic();
    }
}
