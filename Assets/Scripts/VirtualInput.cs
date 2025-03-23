using UnityEngine;
    
public static class VirtualInput
{
    public static bool GetLeftPressed()
    {
        if (Input.GetKeyDown(KeyCode.A)) return true;

        if (Input.GetMouseButtonDown(0))
            return Input.mousePosition.x < Screen.width / 2;

        return false;
    }

    public static bool GetRightPressed()
    {
        if (Input.GetKeyDown(KeyCode.D)) return true;

        if (Input.GetMouseButtonDown(0))
            return Input.mousePosition.x >= Screen.width / 2;

        return false;
    }
}