using UnityEngine;
    
public static class VirtualInput
{
    public static bool GetLeftPressed()
    {
        if (Input.GetKeyDown(KeyCode.A)) return true;

        // Touch
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
                return true;
        }

        // Mouse
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width / 2)
            return true;
        
        return false;
    }

    public static bool GetRightPressed()
    {
        if (Input.GetKeyDown(KeyCode.D)) return true;

        // Touch
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began && touch.position.x >= Screen.width / 2)
                return true;
        }

        // Mouse
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x >= Screen.width / 2)
            return true;
        
        return false;
    }
}