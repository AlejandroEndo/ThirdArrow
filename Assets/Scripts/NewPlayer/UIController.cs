using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private PlayerMovementController playerMovementController;
    public CanvasGroup canvasGroup;
    void Start() {
        staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
        playerMovementController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementController>();
    }

    void Update() {
        staminaSlider.value = playerMovementController.currentStamina / playerMovementController.totalStamina;
        canvasGroup.alpha = playerMovementController.isTired ? 0.5f : 1;
    }
}
