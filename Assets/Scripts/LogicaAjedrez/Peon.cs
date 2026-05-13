using System.Collections.Generic;
using UnityEngine;

public class Peon : Pieza
{
    public string TipoPieza => "Peon";

    public Peon(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int origen = ObtenerCeldaEnTablero(tablero);
        int direccion = esBlanca ? 1 : -1;
        int filaInicial = esBlanca ? 1 : 6;

        Vector2Int avanceSimple = new Vector2Int(origen.x, origen.y + direccion);
        if (EsPosicionValida(avanceSimple) && tablero[avanceSimple.x, avanceSimple.y] == null)
        {
            movimientos.Add(avanceSimple);

            Vector2Int avanceDoble = new Vector2Int(origen.x, origen.y + 2 * direccion);
            if (origen.y == filaInicial &&
                EsPosicionValida(avanceDoble) &&
                tablero[avanceDoble.x, avanceDoble.y] == null)
            {
                movimientos.Add(avanceDoble);
            }
        }

        Vector2Int[] diagonalesCaptura = new Vector2Int[]
        {
            new Vector2Int(origen.x - 1, origen.y + direccion),
            new Vector2Int(origen.x + 1, origen.y + direccion)
        };

        foreach (Vector2Int diag in diagonalesCaptura)
        {
            if (!EsPosicionValida(diag))
            {
                continue;
            }

            Pieza piezaEnDiagonal = tablero[diag.x, diag.y];
            if (piezaEnDiagonal != null && piezaEnDiagonal.esBlanca != this.esBlanca)
            {
                movimientos.Add(diag);
            }
        }

        if (casillaAlPasoDestino.HasValue)
        {
            Vector2Int ep = casillaAlPasoDestino.Value;
            if (Mathf.Abs(origen.x - ep.x) == 1 && origen.y + direccion == ep.y && tablero[ep.x, ep.y] == null)
            {
                int yPeonPasante = esBlanca ? ep.y - 1 : ep.y + 1;
                if (yPeonPasante >= 0 && yPeonPasante < 8)
                {
                    Pieza posiblePeon = tablero[ep.x, yPeonPasante];
                    if (posiblePeon is Peon && posiblePeon.esBlanca != this.esBlanca)
                    {
                        movimientos.Add(ep);
                    }
                }
            }
        }

        return movimientos;
    }

    private bool EsPosicionValida(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    }
}
