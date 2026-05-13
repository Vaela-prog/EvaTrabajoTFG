using System.Collections.Generic;
using UnityEngine;

// La torre hereda de Pieza y define su propio movimiento.
public class Torre : Pieza
{
    // constructor
    public Torre(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    // Calcula todos los movimientos posibles de la torre.
    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        // Origen de los rayos: la casilla donde esta esta pieza en la matriz (fuente de verdad).
        // Asi evitamos desincronizar posicion respecto al tablero tras capturas u otros movimientos.
        Vector2Int origen = ObtenerCeldaEnTablero(tablero);

        // Direcciones en las que puede moverse la torre
        Vector2Int[] direcciones = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // dcha
            new Vector2Int(-1, 0),  // izq
            new Vector2Int(0, 1),   // arriba
            new Vector2Int(0, -1)   // abajo
        };

        // Recorremos cada dirección
        foreach (Vector2Int dir in direcciones)
        {
            // Avanzamos casilla por casilla en esa dirección
            for (int i = 1; i < 8; i++)
            {
                // Nueva posición sumando la dirección multiplicada por i
                Vector2Int nuevaPos = new Vector2Int(
                    origen.x + dir.x * i,
                    origen.y + dir.y * i
                );

                // Si la nueva posición está fuera del tablero, paramos
                if (nuevaPos.x < 0 || nuevaPos.x > 7 ||
                    nuevaPos.y < 0 || nuevaPos.y > 7)
                {
                    break;
                }

                // Miramos qué hay en esa casilla
                Pieza piezaEnCasilla = tablero[nuevaPos.x, nuevaPos.y];

                // Si está vacía, podemos movernos ahí
                if (piezaEnCasilla == null)
                {
                    movimientos.Add(nuevaPos);
                }
                else
                {
                    // Si hay pieza enemiga, podemos capturarla
                    if (piezaEnCasilla.esBlanca != this.esBlanca)
                    {
                        movimientos.Add(nuevaPos);
                    }

                    // Si hay pieza, no podemos seguir 
                    break;
                }
            }
        }

        return movimientos;
    }
}
