using System.Collections.Generic;
using UnityEngine;

public class Caballo : Pieza
{
    public string TipoPieza => "Caballo";

    public Caballo(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int origen = ObtenerCeldaEnTablero(tablero);

        Vector2Int[] desplazamientos = new Vector2Int[]
        {
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(-1, -2)
        };

        foreach (Vector2Int desplazamiento in desplazamientos)
        {
            Vector2Int destino = origen + desplazamiento;

            // Limites del tablero 8x8.
            if (destino.x < 0 || destino.x > 7 || destino.y < 0 || destino.y > 7)
            {
                continue;
            }

            Pieza piezaEnDestino = tablero[destino.x, destino.y];

            // Puede moverse a vacia o capturar enemiga.
            if (piezaEnDestino == null || piezaEnDestino.esBlanca != this.esBlanca)
            {
                movimientos.Add(destino);
            }
        }

        return movimientos;
    }
}
