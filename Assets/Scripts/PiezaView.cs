using UnityEngine;

public class PiezaView : MonoBehaviour
{
    private Vector2Int posicionLogica;
    private TableroVisual tableroVisual;

    public void Inicializar(Vector2Int posicionLogica, TableroVisual tableroVisual)
    {
        this.posicionLogica = posicionLogica;
        this.tableroVisual = tableroVisual;
    }

    void OnMouseDown()
    {
        if (tableroVisual == null) return;
        tableroVisual.OnCasillaClick(posicionLogica);
    }
}
