using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[DefaultExecutionOrder(50)]
public class TableroVisual : MonoBehaviour
{
    [Header("Prefabs por pieza y color (12)")]
    public GameObject rey_blanco;
    public GameObject rey_negro;
    public GameObject dama_blanca;
    public GameObject dama_negra;
    public GameObject alfil_blanco;
    public GameObject alfil_negro;
    public GameObject caballo_blanco;
    public GameObject caballo_negro;
    public GameObject torre_blanca;
    public GameObject torre_negra;
    public GameObject peon_blanco;
    public GameObject peon_negro;

    [Tooltip("Si falta algun prefab de los 12, se usa este como respaldo.")]
    public GameObject piezaPrefabPorDefecto;

    [Tooltip("Mismo significado que en Tablero: fracción máxima de la vista; el tablero encaja usando altura y ancho de la cámara.")]
    public float porcentajeAlturaPantalla = 0.8f;

    [Header("Resaltado de seleccion")]
    public Color colorCasillaSeleccionada = Color.yellow;
    public Color colorMovimientoValido = Color.green;
    [Tooltip("Grosor del borde en unidades del mundo (depende del tamanoCasilla calculado).")]
    public float grosorBordeSeleccion = 0.06f;
    [Tooltip("Grosor del borde en unidades del mundo para movimientos validos.")]
    public float grosorBordeMovimiento = 0.045f;

    [Header("Color temporal de piezas (opcional)")]
    [Tooltip("Desactivalo si tus 12 prefabs ya muestran bien blancas/negras; asi no se recolorean los sprites.")]
    public bool aplicarColorTemporalPiezas = false;
    public Color colorPiezaBlanca = new Color(0.95f, 0.95f, 0.95f, 1f);
    public Color colorPiezaNegra = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("UI")]
    public Text textoTurnoUI;
    [Tooltip("Opcional: si está vacío y se crea el Canvas del turno en código, se genera un botón automáticamente.")]
    public Button botonNuevaPartida;

    private float tamanoCasilla;
    private Vector3 origenTablero;// centro de la casilla (0,0)
    private TableroLogico tableroLogico;
    private Vector2Int? casillaSeleccionada;
    private Pieza piezaSeleccionada;
    private List<Vector2Int> movimientosSeleccionados = new List<Vector2Int>();
    private List<GameObject> piezasVisualesInstanciadas = new List<GameObject>();
    private readonly Dictionary<Vector2Int, CasillaView> casillasPorPosicion = new Dictionary<Vector2Int, CasillaView>();
    private bool turnoBlancas = true;
    private bool partidaTerminada;
    private string mensajeEstadoPartida = string.Empty;

    public void RegistrarCasillaView(Vector2Int posicionLogica, CasillaView vista)
    {
        if (vista == null)
        {
            return;
        }

        casillasPorPosicion[posicionLogica] = vista;
    }

    void Start()
    {
        CalcularGeometriaTablero();
        CrearTableroLogicoInicial();
        DibujarPiezas();
        ActualizarResaltadoVisualCasillas();
        InicializarTextoTurnoUI();
        ConfigurarBotonNuevaPartida();
        partidaTerminada = false;
        mensajeEstadoPartida = string.Empty;
        ActualizarTextoTurnoUI();
    }

    // Calcula tamanoCasilla y origenTablero de la MISMA forma que script Tablero
    void CalcularGeometriaTablero()
    {
        Camera cam = Camera.main;

        float alturaMundo = 2f * cam.orthographicSize;
        float anchoMundo = alturaMundo * cam.aspect;

        float alturaTableroMax = alturaMundo * porcentajeAlturaPantalla;
        float anchoTableroMax = anchoMundo * porcentajeAlturaPantalla;

        float desdeAltura = alturaTableroMax / 8f;
        float desdeAncho = anchoTableroMax / 8f;
        tamanoCasilla = Mathf.Min(desdeAltura, desdeAncho);

        float anchoTablero = 8f * tamanoCasilla;
        float altoTablero = 8f * tamanoCasilla;

        float xInicio = -anchoTablero / 2f;
        float yInicio = -altoTablero / 2f;

        // centro de la casilla (0,0), igual que en Tablero
        origenTablero = new Vector3(
            xInicio + tamanoCasilla / 2f,
            yInicio + tamanoCasilla / 2f,
            0f
        );
    }

    void CrearTableroLogicoInicial()
    {
        tableroLogico = new TableroLogico();
        ColocarPiezasPosicionInicial();
    }

    /// <summary>
    /// Coloca las 32 piezas en la posicion inicial estandar de ajedrez (blancas y == 0 y 1, negras y == 7 y 6).
    /// </summary>
    void ColocarPiezasPosicionInicial()
    {
        // Blancas: fila 0 (piezas mayores) y fila 1 (peones)
        tableroLogico.ColocarPieza(new Torre(true, new Vector2Int(0, 0)), new Vector2Int(0, 0));
        tableroLogico.ColocarPieza(new Caballo(true, new Vector2Int(1, 0)), new Vector2Int(1, 0));
        tableroLogico.ColocarPieza(new Alfil(true, new Vector2Int(2, 0)), new Vector2Int(2, 0));
        tableroLogico.ColocarPieza(new Dama(true, new Vector2Int(3, 0)), new Vector2Int(3, 0));
        tableroLogico.ColocarPieza(new Rey(true, new Vector2Int(4, 0)), new Vector2Int(4, 0));
        tableroLogico.ColocarPieza(new Alfil(true, new Vector2Int(5, 0)), new Vector2Int(5, 0));
        tableroLogico.ColocarPieza(new Caballo(true, new Vector2Int(6, 0)), new Vector2Int(6, 0));
        tableroLogico.ColocarPieza(new Torre(true, new Vector2Int(7, 0)), new Vector2Int(7, 0));

        for (int x = 0; x < 8; x++)
        {
            tableroLogico.ColocarPieza(new Peon(true, new Vector2Int(x, 1)), new Vector2Int(x, 1));
        }

        // Negras: fila 7 y fila 6 (peones)
        tableroLogico.ColocarPieza(new Torre(false, new Vector2Int(0, 7)), new Vector2Int(0, 7));
        tableroLogico.ColocarPieza(new Caballo(false, new Vector2Int(1, 7)), new Vector2Int(1, 7));
        tableroLogico.ColocarPieza(new Alfil(false, new Vector2Int(2, 7)), new Vector2Int(2, 7));
        tableroLogico.ColocarPieza(new Dama(false, new Vector2Int(3, 7)), new Vector2Int(3, 7));
        tableroLogico.ColocarPieza(new Rey(false, new Vector2Int(4, 7)), new Vector2Int(4, 7));
        tableroLogico.ColocarPieza(new Alfil(false, new Vector2Int(5, 7)), new Vector2Int(5, 7));
        tableroLogico.ColocarPieza(new Caballo(false, new Vector2Int(6, 7)), new Vector2Int(6, 7));
        tableroLogico.ColocarPieza(new Torre(false, new Vector2Int(7, 7)), new Vector2Int(7, 7));

        for (int x = 0; x < 8; x++)
        {
            tableroLogico.ColocarPieza(new Peon(false, new Vector2Int(x, 6)), new Vector2Int(x, 6));
        }
    }

    void DibujarPiezas()
    {
        LimpiarPiezasVisuales();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Pieza pieza = tableroLogico.casillas[x, y];
                if (pieza == null) continue;

                Vector2Int posicionLogica = new Vector2Int(x, y);
                Vector3 posMundo = PosicionLogicaAMundo(posicionLogica);
                GameObject prefab = ObtenerPrefabParaPieza(pieza);
                if (prefab == null)
                {
                    Debug.LogWarning("No hay prefab asignado para " + pieza.GetType().Name + " " +
                        (pieza.esBlanca ? "blanca" : "negra") + ". Se omite su dibujo.");
                    continue;
                }

                GameObject piezaInstanciada = Instantiate(prefab, posMundo, Quaternion.identity, transform);
                piezasVisualesInstanciadas.Add(piezaInstanciada);

                if (aplicarColorTemporalPiezas)
                {
                    AplicarColorVisualPieza(piezaInstanciada, pieza);
                }

                PiezaView piezaView = piezaInstanciada.GetComponent<PiezaView>();
                if (piezaView == null)
                {
                    piezaView = piezaInstanciada.AddComponent<PiezaView>();
                }

                piezaView.Inicializar(posicionLogica, this);
            }
        }
    }

    private void LimpiarPiezasVisuales()
    {
        for (int i = 0; i < piezasVisualesInstanciadas.Count; i++)
        {
            if (piezasVisualesInstanciadas[i] != null)
            {
                Destroy(piezasVisualesInstanciadas[i]);
            }
        }

        piezasVisualesInstanciadas.Clear();
    }

    private void AplicarColorVisualPieza(GameObject piezaInstanciada, Pieza piezaLogica)
    {
        SpriteRenderer sr = piezaInstanciada.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            return;
        }

        sr.color = piezaLogica.esBlanca ? colorPiezaBlanca : colorPiezaNegra;
    }

    private GameObject ObtenerPrefabParaPieza(Pieza pieza)
    {
        bool blanca = pieza.esBlanca;
        GameObject elegido = null;

        switch (pieza)
        {
            case Torre:
                elegido = blanca ? torre_blanca : torre_negra;
                break;
            case Caballo:
                elegido = blanca ? caballo_blanco : caballo_negro;
                break;
            case Alfil:
                elegido = blanca ? alfil_blanco : alfil_negro;
                break;
            case Dama:
                elegido = blanca ? dama_blanca : dama_negra;
                break;
            case Rey:
                elegido = blanca ? rey_blanco : rey_negro;
                break;
            case Peon:
                elegido = blanca ? peon_blanco : peon_negro;
                break;
        }

        return elegido != null ? elegido : piezaPrefabPorDefecto;
    }

    Vector3 PosicionLogicaAMundo(Vector2Int posLogica)
    {
        float xMundo = origenTablero.x + posLogica.x * tamanoCasilla;
        float yMundo = origenTablero.y + posLogica.y * tamanoCasilla;

        return new Vector3(xMundo, yMundo, 0f);
    }

    public void OnCasillaClick(Vector2Int posicion)
    {
        SeleccionarCasilla(posicion);
    }

    private void SeleccionarCasilla(Vector2Int posicion)
    {
        if (partidaTerminada)
        {
            Debug.Log("La partida ha terminado.");
            return;
        }

        if (!EsPosicionValida(posicion))
        {
            Debug.LogWarning("Casilla fuera del tablero: " + posicion);
            LimpiarSeleccion();
            return;
        }

        Pieza piezaEnCasilla = tableroLogico.ObtenerPieza(posicion);

        // Si no hay seleccion previa, intentamos seleccionar una pieza como origen.
        if (piezaSeleccionada == null)
        {
            if (piezaEnCasilla == null)
            {
                Debug.Log("Seleccionada casilla " + posicion + " vacia.");
                ActualizarResaltadoVisualCasillas();
                return;
            }

            if (piezaEnCasilla.esBlanca != turnoBlancas)
            {
                Debug.Log("No es el turno de esa pieza.");
                LimpiarSeleccion();
                return;
            }

            casillaSeleccionada = posicion;
            piezaSeleccionada = piezaEnCasilla;
            CargarMovimientosLegalesPiezaSeleccionada();

            string color = piezaSeleccionada.esBlanca ? "blanca" : "negra";
            string tipo = piezaSeleccionada.GetType().Name;
            Debug.Log("Seleccionada casilla " + posicion + " con pieza " + tipo + " " + color + ".");
            ActualizarResaltadoVisualCasillas();
            return;
        }

        // Si hay seleccion previa y se clica una pieza.
        if (piezaEnCasilla != null)
        {
            // Si el clic es sobre un destino valido (pieza enemiga), se ejecuta captura.
            if (movimientosSeleccionados.Contains(posicion))
            {
                MoverPiezaSeleccionada(posicion);
                return;
            }

            // Si se clica una pieza del color en turno, reselecciona origen.
            if (piezaEnCasilla.esBlanca != turnoBlancas)
            {
                Debug.Log("No puedes seleccionar esa pieza en este turno.");
                return;
            }

            casillaSeleccionada = posicion;
            piezaSeleccionada = piezaEnCasilla;
            CargarMovimientosLegalesPiezaSeleccionada();

            string color = piezaSeleccionada.esBlanca ? "blanca" : "negra";
            string tipo = piezaSeleccionada.GetType().Name;
            Debug.Log("Reseleccionada casilla " + posicion + " con pieza " + tipo + " " + color + ".");
            ActualizarResaltadoVisualCasillas();
            return;
        }

        // Si hay seleccion previa y se clica una casilla vacia, intentamos mover.
        if (movimientosSeleccionados.Contains(posicion))
        {
            MoverPiezaSeleccionada(posicion);
            return;
        }
        else
        {
            Debug.Log("Movimiento no valido hacia " + posicion + ".");
            LimpiarSeleccion();
            return;
        }
    }

    private void MoverPiezaSeleccionada(Vector2Int destino)
    {
        if (casillaSeleccionada == null || piezaSeleccionada == null)
        {
            return;
        }

        Vector2Int origen = casillaSeleccionada.Value;
        Vector2Int? casillaAlPasoAntes = tableroLogico.CasillaAlPasoDestino;

        bool esCapturaAlPaso = piezaSeleccionada is Peon &&
                               casillaAlPasoAntes.HasValue &&
                               destino == casillaAlPasoAntes.Value &&
                               tableroLogico.casillas[destino.x, destino.y] == null &&
                               Mathf.Abs(destino.x - origen.x) == 1;

        bool esEnroque = piezaSeleccionada is Rey && Mathf.Abs(destino.x - origen.x) == 2;

        Vector2Int? nuevoAlPaso = null;
        if (piezaSeleccionada is Peon peonMoviendo)
        {
            int dirY = peonMoviendo.esBlanca ? 1 : -1;
            if (destino.x == origen.x && Mathf.Abs(destino.y - origen.y) == 2)
            {
                nuevoAlPaso = new Vector2Int(origen.x, origen.y + dirY);
            }
        }

        if (esCapturaAlPaso)
        {
            Vector2Int posPeonCapturado = new Vector2Int(destino.x, origen.y);
            tableroLogico.casillas[posPeonCapturado.x, posPeonCapturado.y] = null;
        }

        tableroLogico.casillas[origen.x, origen.y] = null;

        if (esEnroque)
        {
            int dir = destino.x > origen.x ? 1 : -1;
            int rookFromX = dir > 0 ? 7 : 0;
            Vector2Int posTorreOrigen = new Vector2Int(rookFromX, origen.y);
            Vector2Int posTorreDestino = new Vector2Int(destino.x - dir, origen.y);
            Pieza torre = tableroLogico.casillas[posTorreOrigen.x, posTorreOrigen.y];
            if (torre != null)
            {
                tableroLogico.casillas[posTorreOrigen.x, posTorreOrigen.y] = null;
                tableroLogico.ColocarPieza(torre, posTorreDestino);
                torre.haMovido = true;
            }
        }

        tableroLogico.ColocarPieza(piezaSeleccionada, destino);
        piezaSeleccionada.haMovido = true;

        tableroLogico.CasillaAlPasoDestino = nuevoAlPaso;

        Debug.Log("Pieza movida de " + origen + " a " + destino + ".");
        AplicarPromocionPeonSiCorresponde(destino);

        turnoBlancas = !turnoBlancas;
        Debug.Log("Turno: " + (turnoBlancas ? "Blancas" : "Negras"));

        AjedrezReglas.EvaluarEstadoTrasTurno(
            tableroLogico,
            turnoBlancas,
            out bool enJaque,
            out bool mate,
            out bool ahogado);

        if (mate)
        {
            partidaTerminada = true;
            string ganador = turnoBlancas ? "Negras" : "Blancas";
            mensajeEstadoPartida = "Jaque mate. Ganan las " + ganador + ".";
            Debug.Log(mensajeEstadoPartida);
        }
        else if (ahogado)
        {
            partidaTerminada = true;
            mensajeEstadoPartida = "Tablas por ahogado.";
            Debug.Log(mensajeEstadoPartida);
        }
        else if (enJaque)
        {
            mensajeEstadoPartida = "Jaque a las " + (turnoBlancas ? "blancas" : "negras") + ".";
            Debug.Log(mensajeEstadoPartida);
        }
        else
        {
            mensajeEstadoPartida = string.Empty;
        }

        ActualizarTextoTurnoUI();

        // Redibujar todas las piezas desde el tablero logico.
        DibujarPiezas();
        LimpiarSeleccion();
    }

    /// <summary>
    /// Promocion automatica a dama al llegar a la ultima fila (sin elegir pieza).
    /// </summary>
    private void AplicarPromocionPeonSiCorresponde(Vector2Int destino)
    {
        Pieza p = tableroLogico.casillas[destino.x, destino.y];
        if (p is Peon peon)
        {
            bool ultimaFilaBlanca = peon.esBlanca && destino.y == 7;
            bool ultimaFilaNegra = !peon.esBlanca && destino.y == 0;
            if (ultimaFilaBlanca || ultimaFilaNegra)
            {
                Dama promovida = new Dama(peon.esBlanca, destino);
                tableroLogico.ColocarPieza(promovida, destino);
                Debug.Log("Peón promovido a dama.");
            }
        }
    }

    private void CargarMovimientosLegalesPiezaSeleccionada()
    {
        if (piezaSeleccionada == null || !casillaSeleccionada.HasValue)
        {
            movimientosSeleccionados.Clear();
            return;
        }

        movimientosSeleccionados = AjedrezReglas.FiltrarMovimientosLegales(
            piezaSeleccionada,
            casillaSeleccionada.Value,
            tableroLogico);
    }

    private void LimpiarSeleccion()
    {
        casillaSeleccionada = null;
        piezaSeleccionada = null;
        movimientosSeleccionados.Clear();
        ActualizarResaltadoVisualCasillas();
    }

    private bool EsPosicionValida(Vector2Int posicion)
    {
        return posicion.x >= 0 && posicion.x < 8 &&
               posicion.y >= 0 && posicion.y < 8;
    }

    private void ActualizarResaltadoVisualCasillas()
    {
        foreach (KeyValuePair<Vector2Int, CasillaView> par in casillasPorPosicion)
        {
            if (par.Value != null)
            {
                par.Value.OcultarBorde();
            }
        }

        if (piezaSeleccionada == null || !casillaSeleccionada.HasValue)
        {
            return;
        }

        foreach (Vector2Int destino in movimientosSeleccionados)
        {
            if (destino == casillaSeleccionada.Value)
            {
                continue;
            }

            if (casillasPorPosicion.TryGetValue(destino, out CasillaView casillaMov) && casillaMov != null)
            {
                casillaMov.MostrarBorde(colorMovimientoValido, grosorBordeMovimiento);
            }
        }

        if (casillasPorPosicion.TryGetValue(casillaSeleccionada.Value, out CasillaView casillaSel) && casillaSel != null)
        {
            casillaSel.MostrarBorde(colorCasillaSeleccionada, grosorBordeSeleccion);
        }
    }

    private void InicializarTextoTurnoUI()
    {
        if (textoTurnoUI != null)
        {
            if (botonNuevaPartida == null)
            {
                Canvas canvasExistente = textoTurnoUI.GetComponentInParent<Canvas>();
                if (canvasExistente != null)
                {
                    CrearBotonNuevaPartidaEnCanvas(canvasExistente.gameObject);
                }
            }

            return;
        }

        GameObject canvasGO = new GameObject("CanvasTurno");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textoGO = new GameObject("TextoTurno");
        textoGO.transform.SetParent(canvasGO.transform, false);
        textoTurnoUI = textoGO.AddComponent<Text>();
        Font fuenteTurno = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (fuenteTurno != null)
        {
            textoTurnoUI.font = fuenteTurno;
        }
        else
        {
            Debug.LogWarning("No se encontro la fuente integrada LegacyRuntime.ttf; el Text usara la fuente por defecto del editor.");
        }

        textoTurnoUI.fontSize = 28;
        textoTurnoUI.alignment = TextAnchor.UpperLeft;
        textoTurnoUI.color = Color.white;

        RectTransform rt = textoTurnoUI.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(20f, -20f);
        rt.sizeDelta = new Vector2(420f, 110f);

        CrearBotonNuevaPartidaEnCanvas(canvasGO);
    }

    private void CrearBotonNuevaPartidaEnCanvas(GameObject canvasGO)
    {
        if (botonNuevaPartida != null)
        {
            return;
        }

        GameObject btnGO = new GameObject("BotonNuevaPartida");
        btnGO.transform.SetParent(canvasGO.transform, false);
        RectTransform rtBtn = btnGO.AddComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0f, 1f);
        rtBtn.anchorMax = new Vector2(0f, 1f);
        rtBtn.pivot = new Vector2(0f, 1f);
        rtBtn.anchoredPosition = new Vector2(20f, -140f);
        rtBtn.sizeDelta = new Vector2(200f, 40f);

        Image img = btnGO.AddComponent<Image>();
        img.color = new Color(0.25f, 0.45f, 0.75f, 1f);
        botonNuevaPartida = btnGO.AddComponent<Button>();

        GameObject textoBtnGO = new GameObject("Texto");
        textoBtnGO.transform.SetParent(btnGO.transform, false);
        Text txtBtn = textoBtnGO.AddComponent<Text>();
        txtBtn.text = "Nueva partida";
        Font fuenteBtn = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (fuenteBtn != null)
        {
            txtBtn.font = fuenteBtn;
        }

        txtBtn.fontSize = 20;
        txtBtn.alignment = TextAnchor.MiddleCenter;
        txtBtn.color = Color.white;
        RectTransform rtTxt = txtBtn.rectTransform;
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.offsetMin = Vector2.zero;
        rtTxt.offsetMax = Vector2.zero;
    }

    private void ConfigurarBotonNuevaPartida()
    {
        if (botonNuevaPartida == null)
        {
            return;
        }

        botonNuevaPartida.onClick.RemoveListener(NuevaPartida);
        botonNuevaPartida.onClick.AddListener(NuevaPartida);
    }

    /// <summary>
    /// Reinicia tablero lógico, turno, estado de partida y piezas visuales. Enlázalo también desde un Button en la UI.
    /// </summary>
    public void NuevaPartida()
    {
        CalcularGeometriaTablero();
        tableroLogico = new TableroLogico();
        ColocarPiezasPosicionInicial();
        turnoBlancas = true;
        partidaTerminada = false;
        mensajeEstadoPartida = string.Empty;
        LimpiarSeleccion();
        DibujarPiezas();
        ActualizarTextoTurnoUI();
    }

    private void ActualizarTextoTurnoUI()
    {
        if (textoTurnoUI == null)
        {
            return;
        }

        if (partidaTerminada)
        {
            textoTurnoUI.text = mensajeEstadoPartida;
            return;
        }

        string lineaTurno = "Turno: " + (turnoBlancas ? "Blancas" : "Negras");
        if (!string.IsNullOrEmpty(mensajeEstadoPartida))
        {
            textoTurnoUI.text = lineaTurno + "\n" + mensajeEstadoPartida;
        }
        else
        {
            textoTurnoUI.text = lineaTurno;
        }
    }

}
