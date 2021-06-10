using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DamageType {
    BASIC,
    CRITICAL,
    HEALTH
}

public class DamageDisplay : MonoBehaviour {

    public static DamageDisplay Create(Vector3 pos, int amount, DamageType type) {
        GameObject textDamage = Instantiate(GameManager.Instance.DamageText, pos, Quaternion.identity);
        DamageDisplay damage = textDamage.GetComponent<DamageDisplay>();
        damage.CustomSetup(amount, type);
        return damage;
    }

    public float disapperarTimer;
    public float disappearSpeed;

    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake() {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void CustomSetup(int damageAmount, DamageType type) {
        textMesh.SetText(damageAmount.ToString());
        textColor = GameManager.Instance.colors[type];
        textMesh.color = textColor;
    }

    private void Update() {
        transform.localPosition += Vector3.up * 1f * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        disapperarTimer -= Time.deltaTime;
        if (disapperarTimer < 0f) {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0f) {
                Destroy(gameObject);
            }
        }

    }
}
