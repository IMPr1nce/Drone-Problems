using UnityEngine;

public class breakable : MonoBehaviour
{
    [SerializeField] private GameObject brokenVersionPrefab;
    [SerializeField] private GameObject notBrokenVersionPrefab;

    [Header("Audio")]
    AudioSource audioSource;
    private BoxCollider bc;

    private void Awake()
    {
        bc = GetComponent<BoxCollider>();

        if (notBrokenVersionPrefab != null)
            notBrokenVersionPrefab.SetActive(true);

        if (brokenVersionPrefab != null)
            brokenVersionPrefab.SetActive(false);

        if (bc != null)
            bc.enabled = true;
        audioSource = GetComponent<AudioSource>();
    }

    private void Break()
    {
        if (notBrokenVersionPrefab != null)
            notBrokenVersionPrefab.SetActive(false);

        if (brokenVersionPrefab != null)
            brokenVersionPrefab.SetActive(true);

        if (bc != null)
            bc.enabled = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Break();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}