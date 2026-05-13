using UnityEngine;

public class FichaPrueba : MonoBehaviour
{
    public GameObject fichaPrefab;

    private GameObject fichaInstanciada;

    public float velocidad = 5f;

    // límites calculados según la cámara
    private float limiteIzq;
    private float limiteDer;
    private float limiteArriba;
    private float limiteAbajo;

    void Start()
    {
        //ficha en el centro
        fichaInstanciada = Instantiate(fichaPrefab, Vector3.zero, Quaternion.identity);

        // calculamos los límites visibles de la cámara
        CalcularLimites();
    }

    void Update()
    {
        if (fichaInstanciada == null)
            return;

        // movimiento con teclado
        float moverX = Input.GetAxisRaw("Horizontal");
        float moverY = Input.GetAxisRaw("Vertical");

        Vector3 movimiento = new Vector3(moverX, moverY, 0) * velocidad * Time.deltaTime;

        fichaInstanciada.transform.position += movimiento;

        // aplicamos límites
        Vector3 pos = fichaInstanciada.transform.position;

        pos.x = Mathf.Clamp(pos.x, limiteIzq, limiteDer);
        pos.y = Mathf.Clamp(pos.y, limiteAbajo, limiteArriba);

        fichaInstanciada.transform.position = pos;
    }

    // calcula los límites visibles de la cámara 
    void CalcularLimites()
    {
        Camera cam = Camera.main;

        // esquina inf izq
        Vector3 abajoIzq = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));

        // esquina sup dcha
        Vector3 arribaDer = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        limiteIzq = abajoIzq.x;
        limiteAbajo = abajoIzq.y;

        limiteDer = arribaDer.x;
        limiteArriba = arribaDer.y;
    }
}
