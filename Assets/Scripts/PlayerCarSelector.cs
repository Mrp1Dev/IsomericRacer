using UnityEngine;

public class PlayerCarSelector : MonoBehaviour
{
    [SerializeField] private Transform carVisualPivot;
    [SerializeField] private float rotationSpeedDeg;

    private GameObject currentVisual;
    public void SetCarVisual(GameObject prefab)
    {
        if(currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }
        currentVisual = Instantiate(prefab, carVisualPivot);
    }

    private void Update()
    {
        carVisualPivot.Rotate(Vector3.up * rotationSpeedDeg * Time.deltaTime);
    }
}
