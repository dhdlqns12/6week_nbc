using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour, IObject
{
    [SerializeField] private ItemData itemData;

    private MeshRenderer meshRenderer;
    private Collider itemCollider;

    public string ObjectName => itemData.itemName;
    public string Description => itemData.itemDescirption;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        itemCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                StartCoroutine(itemData.itemEffect.ApplyEffect(player));

                StartCoroutine(RespawnItem());
            }
        }
    }

    IEnumerator RespawnItem()
    {
        meshRenderer.enabled = false;
        itemCollider.enabled = false;

        yield return new WaitForSeconds(itemData.respawnTime);

        meshRenderer.enabled = true;
        itemCollider.enabled = true;
    }
}
