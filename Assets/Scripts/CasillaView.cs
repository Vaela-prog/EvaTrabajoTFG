using UnityEngine;

// Casilla clickeable + borde de resaltado (LineRenderer local; cuadrado -0.5..0.5 como sprite unity square).
public class CasillaView : MonoBehaviour
{
    public Vector2Int posicionLogica;   // (x, y) en el tablero 0–7
    private TableroVisual tableroVisual;

    private LineRenderer lineaBorde;
    private SpriteRenderer spriteCasilla;

    public void Inicializar(Vector2Int posicion, TableroVisual tableroVisual)
    {
        this.posicionLogica = posicion;
        this.tableroVisual = tableroVisual;
        spriteCasilla = GetComponent<SpriteRenderer>();
        AsegurarLineRendererBorde();
    }

    public void MostrarBorde(Color color, float grosorWorld)
    {
        AsegurarLineRendererBorde();

        Color c = color;
        lineaBorde.enabled = true;
        lineaBorde.startColor = c;
        lineaBorde.endColor = c;

        float sx = Mathf.Max(transform.lossyScale.x, 0.0001f);
        float anchoNormalizado = grosorWorld / sx;
        lineaBorde.widthMultiplier = anchoNormalizado;

        CopiarSortingDesdeCasillaSiHay();
    }

    public void OcultarBorde()
    {
        if (lineaBorde != null)
        {
            lineaBorde.enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (tableroVisual == null) return;

        // Avisamos al TableroVisual que se ha hecho clic en esta casilla
        tableroVisual.OnCasillaClick(posicionLogica);
    }

    private void AsegurarLineRendererBorde()
    {
        if (lineaBorde != null)
        {
            return;
        }

        Transform bordeTf = transform.Find("BordeResaltado");
        GameObject go;
        if (bordeTf != null)
        {
            go = bordeTf.gameObject;
            lineaBorde = go.GetComponent<LineRenderer>();
        }
        else
        {
            go = new GameObject("BordeResaltado");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            lineaBorde = go.AddComponent<LineRenderer>();
        }

        lineaBorde.useWorldSpace = false;
        lineaBorde.loop = true;
        lineaBorde.positionCount = 5;
        // Rectangulo rodeando la casilla (sprite unitario Quad/Square antes de escalado padre).
        float h = 0.499f;
        lineaBorde.SetPositions(new Vector3[]
        {
            new Vector3(-h, -h, 0f),
            new Vector3(h, -h, 0f),
            new Vector3(h, h, 0f),
            new Vector3(-h, h, 0f),
            new Vector3(-h, -h, 0f),
        });

        lineaBorde.numCornerVertices = 2;
        lineaBorde.numCapVertices = 2;
        Shader shaderLinea = Shader.Find("Sprites/Default");
        if (shaderLinea == null)
        {
            shaderLinea = Shader.Find("Unlit/Color");
        }

        if (shaderLinea != null)
        {
            lineaBorde.material = new Material(shaderLinea);
        }
        lineaBorde.textureMode = LineTextureMode.Stretch;

        CopiarSortingDesdeCasillaSiHay();

        lineaBorde.enabled = false;
    }

    private void CopiarSortingDesdeCasillaSiHay()
    {
        if (spriteCasilla != null && lineaBorde != null)
        {
            lineaBorde.sortingLayerID = spriteCasilla.sortingLayerID;
            lineaBorde.sortingOrder = spriteCasilla.sortingOrder + 1;
        }
    }
}
