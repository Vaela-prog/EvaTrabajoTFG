using System.Collections.Generic;
using UnityEngine;

public class Alfil : Pieza
{
    public string TipoPieza => "Alfil";

    public Alfil(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int origen = ObtenerCeldaEnTablero(tablero);

        Vector2Int[] direcciones = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // diagonal arriba-derecha
            new Vector2Int(1, -1),  // diagonal abajo-derecha
            new Vector2Int(-1, 1),  // diagonal arriba-izquierda
            new Vector2Int(-1, -1)  // diagonal abajo-izquierda
        };

        foreach (Vector2Int dir in direcciones)
        {
            for (int i = 1; i < 8; i++)
            {
                Vector2Int nuevaPos = new Vector2Int(
                    origen.x + dir.x * i,
                    origen.y + dir.y * i
                );

                // Limites del tablero.
                if (nuevaPos.x < 0 || nuevaPos.x > 7 || nuevaPos.y < 0 || nuevaPos.y > 7)
                {
                    break;
                }

                Pieza piezaEnCasilla = tablero[nuevaPos.x, nuevaPos.y];

                if (piezaEnCasilla == null)
                {
                    // Casilla vacia: el alfil puede seguir avanzando.
                    movimientos.Add(nuevaPos);
                }
                else
                {
                    // Si es enemiga, se puede capturar y se corta la direccion.
                    if (piezaEnCasilla.esBlanca != this.esBlanca)
                    {
                        movimientos.Add(nuevaPos);
                    }

                    // Si hay cualquier pieza, no puede atravesarla.
                    break;
                }
            }
        }

        return movimientos;
    }
}
