using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Interactable
{
    public override void OnFocus()
    {
        print("Focus on " + gameObject.name);   
    }

    public override void OnInteract()
    {
        print("Interact on " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("Lose focus from " + gameObject.name);
    }
}
