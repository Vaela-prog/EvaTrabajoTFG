using UnityEngine;


[DefaultExecutionOrder(-50)]
public class Tablero : MonoBehaviour
{
    // Prefab de una casilla (un sprite cuadrado con SpriteRenderer)
    public GameObject casillaPrefab;

    // Porcentaje de la vista (altura y ancho) que ocupará como máximo el tablero; se usa el menor para que quepa en móvil/tablet/ultrapanorámico.
    [Range(0.1f, 1f)]
    public float porcentajeAlturaPantalla = 0.8f; // 80% por defecto

    // Tamaño de cada casilla en unidades 
    float tamañoCasilla;

    void Start()
    {
        CalcularTamañoCasilla();
        GenerarTablero();
    }

    // Calcula el tamaño de cada casilla en función de la cámara y del porcentaje de pantalla
    void CalcularTamañoCasilla()
    {
        Camera cam = Camera.main;

        float alturaMundo = 2f * cam.orthographicSize;
        float anchoMundo = alturaMundo * cam.aspect;

        float alturaTablero = alturaMundo * porcentajeAlturaPantalla;
        float anchoTablero = anchoMundo * porcentajeAlturaPantalla;

        float desdeAltura = alturaTablero / 8f;
        float desdeAncho = anchoTablero / 8f;
        tamañoCasilla = Mathf.Min(desdeAltura, desdeAncho);
    }

    // Genera las 64 casillas del tablero
    void GenerarTablero()
    {
        // Ancho y alto del tablero en unidades del mundo
        float anchoTablero = 8f * tamañoCasilla;
        float altoTablero = 8f * tamañoCasilla;

        // Punto de inicio (esquina inferior izquierda) para centrar el tablero en el origen (0,0)
        float xInicio = -anchoTablero / 2f;
        float yInicio = -altoTablero / 2f;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // Calculamos la posición de cada casilla, sumando medio tamaño para centrarla en su "celda"
                Vector3 posicion = new Vector3(
                    xInicio + x * tamañoCasilla + tamañoCasilla / 2f,
                    yInicio + y * tamañoCasilla + tamañoCasilla / 2f,
                    0f
                );

                // Instanciamos la casilla
                GameObject casilla = Instantiate(casillaPrefab, posicion, Quaternion.identity);

                // La hacemos hija del objeto Tablero para tenerlo todo ordenado en el Hierarchy
                casilla.transform.parent = transform;

                // Obtenemos el SpriteRenderer para poder cambiar el color
                SpriteRenderer sr = casilla.GetComponent<SpriteRenderer>();

                // Alternar colores 
                bool esNegra = (x + y) % 2 == 0;
                sr.color = esNegra ? Color.black : Color.white;

                // Ajustamos la escala de la casilla para que ocupe exactamente el tamaño calculado
                casilla.transform.localScale = new Vector3(tamañoCasilla, tamañoCasilla, 1f);

                // Vinculamos la casilla visual con su posicion logica para detectar clics.
                CasillaView casillaView = casilla.GetComponent<CasillaView>();
                if (casillaView == null)
                {
                    casillaView = casilla.AddComponent<CasillaView>();
                }

                TableroVisual tableroVisual = GetComponent<TableroVisual>();
                Vector2Int posLogic = new Vector2Int(x, y);
                casillaView.Inicializar(posLogic, tableroVisual);
                tableroVisual.RegistrarCasillaView(posLogic, casillaView);
            }
        }
    }
}
