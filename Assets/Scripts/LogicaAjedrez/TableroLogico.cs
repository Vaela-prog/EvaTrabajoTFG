using System.Collections.Generic;
using UnityEngine;

// Representa el tablero lógico de ajedrez (8x8).
// Aquí no hay sprites, solo piezas a nivel de código.
public class TableroLogico
{
    // Matriz 8x8 donde guardamos las piezas.
    // Si una casilla está vacía, habrá null.
    public Pieza[,] casillas;

    /// <summary>
    /// Casilla a la que un peon puede mover capturando al paso (la casilla "saltada" por el peon rival).
    /// Se limpia al final de cada jugada salvo que se haya creado un nuevo pasante.
    /// </summary>
    public Vector2Int? CasillaAlPasoDestino;

    // Constructor tablero vacío de 8x8.
    public TableroLogico()
    {
        casillas = new Pieza[8, 8];
    }

    // Coloca una pieza en una posición del tablero.
    public void ColocarPieza(Pieza pieza, Vector2Int posicion)
    {
        // Evita que la misma instancia quede en dos casillas si no se limpio el origen antes.
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (casillas[x, y] == pieza)
                {
                    casillas[x, y] = null;
                }
            }
        }

        casillas[posicion.x, posicion.y] = pieza;
        pieza.posicion = posicion;
    }

    // Devuelve la pieza que hay en una posición (o null si está vacía).
    public Pieza ObtenerPieza(Vector2Int posicion)
    {
        return casillas[posicion.x, posicion.y];
    }
}
