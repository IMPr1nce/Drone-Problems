using UnityEngine;

public class RandomContainerColor : MonoBehaviour
{
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public Color color3 = Color.green;

    private void Start()
    {
        Color[] colors = { color1, color2, color3 };
        Color chosenColor = colors[Random.Range(0, colors.Length)];

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            rend.material.color = chosenColor;
        }
    }
}