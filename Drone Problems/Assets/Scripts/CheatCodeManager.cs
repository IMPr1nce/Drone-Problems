using UnityEngine;
using UnityEngine.InputSystem;

public class CheatCodeManager : MonoBehaviour
{
    [Header("Player Reference")]
    public player playerScript;

    [Header("Cheat Settings")]
    public int healthCheatAmount = 100;
    public int bulletCheatAmount = 500;
    public int coinCheatAmount = 50;

    void Update()
    {
        if (playerScript == null)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        bool holdingShift = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;

        if (!holdingShift)
        {
            return;
        }

        if (keyboard.hKey.wasPressedThisFrame)
        {
            playerScript.health = healthCheatAmount;
            Debug.Log("Cheat activated: Health restored");
        }

        if (keyboard.bKey.wasPressedThisFrame)
        {
            playerScript.current_bullets += bulletCheatAmount;
            Debug.Log("Cheat activated: Bullets added");
        }

        if (keyboard.cKey.wasPressedThisFrame)
        {
            playerScript.coins += coinCheatAmount;
            Debug.Log("Cheat activated: Coins added");
        }
    }
}