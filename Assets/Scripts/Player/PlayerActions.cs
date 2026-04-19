using System.Collections;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Managers and Controllers")]
    [SerializeField] InventoryManager inventoryManager;

    [Header("Testing actions")]
    [SerializeField] float damageAmountToTake = 100.0f;

    [Header("Shooting Parameters")]
    [SerializeField] public float shotDuration = 0.7f;
    [SerializeField] public Transform gunEnd;
    [SerializeField] public Camera camera; //camera
    [SerializeField] public Material smokeMaterial;

    float timeSinceLastShot = 0.0f;

    private Health healthBlock;
    private Attack attackBlock;

    public void GetComponents()
    {
        healthBlock = GetComponent<Health>();
        attackBlock = GetComponent<Attack>();
    }

    private void Update()
    {
        HandleTimePassing();
    }

    void HandleTimePassing()
    {
        if (timeSinceLastShot <= attackBlock.attackDelay)
            timeSinceLastShot += Time.deltaTime;
    }

    public void FireWeapon(bool isAttackKeyDown)
    {
        if (!isAttackKeyDown || timeSinceLastShot < attackBlock.attackDelay)
            return;

        timeSinceLastShot = 0.0f;

        inventoryManager.GetEquipedWeapon().TryFireWeapon();
    }

    public void ReloadWeapon(bool isReloadKeyDown)
    {
        if (!isReloadKeyDown) return;

        inventoryManager.GetEquipedWeapon().ReloadWeapon();
    }

    /*
     * Old deprecated weapon firing, useful for handling some basic debugging
     */
    public void FireWeapon(bool isAttackKeyDown, int fake)
    {
        if (!isAttackKeyDown || timeSinceLastShot < attackBlock.attackDelay)
            return;

        timeSinceLastShot = 0.0f;

        Vector3 rayOrigin = gunEnd.position;
        RaycastHit hit;

        Vector3 targetPoint = rayOrigin + camera.transform.forward * attackBlock.attackRange;

        if (Physics.Raycast(rayOrigin, camera.transform.forward, out hit, attackBlock.attackRange))
        {
            targetPoint = hit.point;

            GameObject hitObject = hit.collider.gameObject;
            Health targetHealth = hitObject.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackBlock.attackDamage);
                Debug.Log("Hit " + hitObject.name + " for " + attackBlock.attackDamage + " damage!");
            }
            else
                Debug.Log(hitObject.name + " has no Health component.");
        }

        GameObject laserObj = new GameObject("Trace");
        LineRenderer lr = laserObj.AddComponent<LineRenderer>();

        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.material = smokeMaterial;
        lr.SetPosition(0, gunEnd.position);
        lr.SetPosition(1, targetPoint);
        StartCoroutine(FadeAndDestroy(lr, laserObj, shotDuration));
    }

    private IEnumerator FadeAndDestroy(LineRenderer lr, GameObject laserObj, float duration)
    {
        float elapsed = 0f;

        // grab the material’s base color (URP uses _BaseColor)
        Color baseColor = lr.material.GetColor("_BaseColor");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            // apply the fading alpha directly to the material color
            baseColor.a = alpha;
            lr.material.SetColor("_BaseColor", baseColor);

            yield return null;
        }

        Destroy(laserObj);
    }

    public void SelfInflictDamage(bool isKeyDown)
    {
        if(isKeyDown)
        {
            healthBlock.TakeDamage(damageAmountToTake);
        }
    }
}
