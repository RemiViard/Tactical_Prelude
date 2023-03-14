using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Slider slider;
    [SerializeField] Color[] colors;
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    public void InitMaxHp(int maxLife)
    {
        slider.maxValue = maxLife;
        slider.value = maxLife;
    }
    private void LateUpdate()
    {
        canvas.transform.LookAt(canvas.transform.position + cam.transform.forward);
    }
    public void ChangeValue(int _hp)
    {
        slider.value = _hp;
    }
    public void ChangeColor(Army.ArmyColor color)
    {
        slider.gameObject.transform.GetChild(0).GetComponent<Image>().color = colors[(int)color];
    }
}
