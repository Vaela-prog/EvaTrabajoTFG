using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base abstracta que representa una pieza de ajedrez.
/// Contiene la información común a todas las piezas (color, posición)
/// y define el método que cada pieza concreta debe implementar
/// para calcular sus movimientos posibles.
/// </summary>
public abstract class Pieza
{
    /// <summary>
    /// Indica si la pieza es blanca (true) o negra (false).
    /// </summary>
    public bool esBlanca;

    /// <summary>
    /// Posición actual de la pieza en el tablero lógico.
    /// Vector2Int porque el tablero es una cuadrícula de enteros (0-7, 0-7).
    /// </summary>
    public Vector2Int posicion;

    /// <summary>
    /// True despues del primer movimiento (enroque y derecho al paso).
    /// </summary>
    public bool haMovido;

    /// <summary>
    /// Constructor de la pieza.
    /// Obliga a especificar el color y la posición inicial.
    /// </summary>
    /// <param name="esBlanca">True si la pieza es blanca, false si es negra.</param>
    /// <param name="posicion">Posición inicial de la pieza en el tablero.</param>
    public Pieza(bool esBlanca, Vector2Int posicion)
    {
        this.esBlanca = esBlanca;
        this.posicion = posicion;
        this.haMovido = false;
    }

    /// <summary>
    /// Casilla donde esta instancia esta en la matriz (fuente de verdad frente a <see cref="posicion"/>).
    /// </summary>
    protected Vector2Int ObtenerCeldaEnTablero(Pieza[,] tablero)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (tablero[x, y] == this)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return posicion;
    }

    /// <summary>
    /// Método abstracto que debe implementar cada tipo de pieza.
    /// Calcula todas las casillas a las que la pieza puede moverse
    /// según sus reglas de movimiento y el estado del tablero.
    /// </summary>
    /// <param name="tablero">
    /// Matriz 8x8 que representa el tablero lógico.
    /// Cada casilla contiene una pieza o null si está vacía.
    /// </param>
    /// <returns>
    /// Lista de posiciones (Vector2Int) a las que la pieza puede moverse.
    /// </returns>
    /// <param name="casillaAlPasoDestino">
    /// Casilla a la que un peon puede mover en captura al paso (null si no hay pasante).
    /// </param>
    public abstract List<Vector2Int> CalcularMovimientosPosibles(Pieza[,] tablero, Vector2Int? casillaAlPasoDestino);
}
