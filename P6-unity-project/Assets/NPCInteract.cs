using UnityEngine;

public class NPCInteract : InteractHandler
{
    public override void InteractLogic()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.borders.SetTrigger("ToggleBorders");
        base.InteractLogic();
    }
}
