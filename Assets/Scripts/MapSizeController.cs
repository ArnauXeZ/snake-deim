using UnityEngine;
using UnityEngine.UI;

public class MapSizeController : MonoBehaviour
{
    public Slider mapSizeSlider;

    private GameManager gameManager;

    private void Start()
    {

        mapSizeSlider.onValueChanged.AddListener(UpdateMapAndBackgroundSize);
    }

    private void UpdateMapAndBackgroundSize(float newSize)
    {
        // Llamamos al m?todo en el GameManager para cambiar el tama?o del mapa
        GameManager.Instance.ChangeMapSize((int)newSize, (int)newSize);

        // Llamamos al m?todo en el GameManager para cambiar el tama?o del fondo
        GameManager.Instance.ChangeBackgroundSize(newSize);
    }
}



