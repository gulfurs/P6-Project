using UnityEngine;

public class CrabInteract : InteractHandler
{
    public GameObject commandUIPrefab;
    public LogManager logman;

    private GameObject currentUI;

    void Start()
    {
        logman = FindObjectOfType<LogManager>();
    }

    public override void InteractLogic()
    {
        if (currentUI == null) 
        {
            currentUI = Instantiate(commandUIPrefab);
            currentUI.GetComponent<CrabInterface>().crab = GetComponent<CrabHandler>();
            logman.SetCrabInterface(commandUIPrefab.GetComponent<CrabInterface>());
        }

        logman.ToggleLogMenu(true);

        base.InteractLogic();
        interactable = true;
    }
}
