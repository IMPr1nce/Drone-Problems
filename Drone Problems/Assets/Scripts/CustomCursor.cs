using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    void Start()
    {
        SetCustomCursor();
    }

    void SetCustomCursor()
    {
        if (cursorTexture == null)
        {
            //Debug.LogWarning("No cursor texture assigned.");
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Vector2 hotspot = new Vector2(
            cursorTexture.width / 2f,
            cursorTexture.height / 2f
        );

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}