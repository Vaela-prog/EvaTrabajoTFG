using System.Collections.Generic;
using UnityEngine;

public class Rey : Pieza
{
    public string TipoPieza => "Rey";

    public Rey(bool esBlanca, Vector2Int posicion) : base(esBlanca, posicion)
    {
    }

    public override List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino)
    {
        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int origen = ObtenerCeldaEnTablero(tablero);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                {
                    continue;
                }

                Vector2Int destino = new Vector2Int(origen.x + dx, origen.y + dy);

                if (destino.x < 0 || destino.x > 7 || destino.y < 0 || destino.y > 7)
                {
                    continue;
                }

                Pieza piezaEnDestino = tablero[destino.x, destino.y];

                if (piezaEnDestino == null || piezaEnDestino.esBlanca != this.esBlanca)
                {
                    movimientos.Add(destino);
                }
            }
        }

        if (!haMovido)
        {
            AnadirEnroqueSiAplica(tablero, movimientos, origen, 1);
            AnadirEnroqueSiAplica(tablero, movimientos, origen, -1);
        }

        return movimientos;
    }

    private void AnadirEnroqueSiAplica(Pieza[,] tablero, List<Vector2Int> movimientos, Vector2Int celdaRey, int dir)
    {
        int y = celdaRey.y;
        int kingX = celdaRey.x;

        if (dir > 0)
        {
            for (int xi = kingX + 1; xi <= 6; xi++)
            {
                if (tablero[xi, y] != null)
                {
                    return;
                }
            }

            Pieza r = tablero[7, y];
            if (r is Torre && r.esBlanca == esBlanca && !r.haMovido)
            {
                movimientos.Add(new Vector2Int(kingX + 2, y));
            }
        }
        else
        {
            for (int xi = kingX - 1; xi >= 1; xi--)
            {
                if (tablero[xi, y] != null)
                {
                    return;
                }
            }

            Pieza r = tablero[0, y];
            if (r is Torre && r.esBlanca == esBlanca && !r.haMovido)
            {
                movimientos.Add(new Vector2Int(kingX - 2, y));
            }
        }
    }
}
