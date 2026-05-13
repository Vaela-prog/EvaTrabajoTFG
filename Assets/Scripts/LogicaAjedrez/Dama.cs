using System.Collections.Generic;
using UnityEngine;

public class Dama : Pieza
{
    public string TipoPieza => "Dama";

    public Dama(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int origen = ObtenerCeldaEnTablero(tablero);

        Vector2Int[] direcciones = new Vector2Int[]
        {
            // Rectas (como torre)
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),

            // Diagonales (como alfil)
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
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
                    movimientos.Add(nuevaPos);
                }
                else
                {
                    // Captura de enemiga y corte de la direccion.
                    if (piezaEnCasilla.esBlanca != this.esBlanca)
                    {
                        movimientos.Add(nuevaPos);
                    }

                    // No puede atravesar piezas.
                    break;
                }
            }
        }

        return movimientos;
    }
}
