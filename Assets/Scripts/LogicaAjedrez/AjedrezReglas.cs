using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reglas de ajedrez: legalidad (no dejar rey en jaque), jaque, mate, ahogado.
/// Simula enroque y captura al paso correctamente al probar movimientos.
/// </summary>
public static class AjedrezReglas
{
    public static List<Vector2Int> FiltrarMovimientosLegales(Pieza pieza, Vector2Int origen, TableroLogico tableroLogico)
    {
        Pieza[,] tablero = tableroLogico.casillas;
        List<Vector2Int> pseudo = pieza.CalcularMovimientosPosibles(tablero, tableroLogico.CasillaAlPasoDestino);
        List<Vector2Int> legales = new List<Vector2Int>();

        foreach (Vector2Int destino in pseudo)
        {
            if (MovimientoDejaReyEnJaque(tableroLogico, origen, destino))
            {
                continue;
            }

            if (pieza is Rey && Mathf.Abs(destino.x - origen.x) == 2 &&
                !EnroqueCumpleCasillasNoAtacadas(tablero, origen, destino, pieza.esBlanca))
            {
                continue;
            }

            legales.Add(destino);
        }

        return legales;
    }

    public static bool MovimientoDejaReyEnJaque(TableroLogico tableroLogico, Vector2Int origen, Vector2Int destino)
    {
        Pieza[,] tablero = tableroLogico.casillas;
        Pieza piezaQueMueve = tablero[origen.x, origen.y];
        if (piezaQueMueve == null)
        {
            return true;
        }

        bool esBlanca = piezaQueMueve.esBlanca;
        Vector2Int posAnterior = piezaQueMueve.posicion;
        Pieza capturada = tablero[destino.x, destino.y];

        bool esAlPaso = piezaQueMueve is Peon &&
                        tableroLogico.CasillaAlPasoDestino.HasValue &&
                        destino == tableroLogico.CasillaAlPasoDestino.Value &&
                        capturada == null &&
                        Mathf.Abs(destino.x - origen.x) == 1;

        Pieza peonCapturadoAlPaso = null;
        Vector2Int posPeonAlPaso = Vector2Int.zero;
        if (esAlPaso)
        {
            int yPeon = esBlanca ? destino.y - 1 : destino.y + 1;
            posPeonAlPaso = new Vector2Int(destino.x, yPeon);
            peonCapturadoAlPaso = tablero[posPeonAlPaso.x, posPeonAlPaso.y];
            tablero[posPeonAlPaso.x, posPeonAlPaso.y] = null;
        }

        bool esEnroque = piezaQueMueve is Rey && Mathf.Abs(destino.x - origen.x) == 2;
        Pieza torreEnroque = null;
        Vector2Int posTorreOrigen = Vector2Int.zero;
        Vector2Int posTorreDestino = Vector2Int.zero;
        Vector2Int posTorreAntes = Vector2Int.zero;

        if (esEnroque)
        {
            int dir = destino.x > origen.x ? 1 : -1;
            int rookFromX = dir > 0 ? 7 : 0;
            posTorreOrigen = new Vector2Int(rookFromX, origen.y);
            torreEnroque = tablero[posTorreOrigen.x, posTorreOrigen.y];
            posTorreDestino = new Vector2Int(destino.x - dir, origen.y);
            posTorreAntes = torreEnroque != null ? torreEnroque.posicion : Vector2Int.zero;

            tablero[posTorreOrigen.x, posTorreOrigen.y] = null;
            tablero[posTorreDestino.x, posTorreDestino.y] = torreEnroque;
            if (torreEnroque != null)
            {
                torreEnroque.posicion = posTorreDestino;
            }
        }

        tablero[origen.x, origen.y] = null;
        tablero[destino.x, destino.y] = piezaQueMueve;
        piezaQueMueve.posicion = destino;

        bool enJaque = EsJaqueEnMatriz(tablero, esBlanca);

        tablero[origen.x, origen.y] = piezaQueMueve;
        tablero[destino.x, destino.y] = capturada;
        piezaQueMueve.posicion = posAnterior;

        if (esEnroque && torreEnroque != null)
        {
            tablero[posTorreDestino.x, posTorreDestino.y] = null;
            tablero[posTorreOrigen.x, posTorreOrigen.y] = torreEnroque;
            torreEnroque.posicion = posTorreAntes;
        }

        if (esAlPaso && peonCapturadoAlPaso != null)
        {
            tablero[posPeonAlPaso.x, posPeonAlPaso.y] = peonCapturadoAlPaso;
            peonCapturadoAlPaso.posicion = posPeonAlPaso;
        }

        return enJaque;
    }

    public static bool EsJaque(TableroLogico tableroLogico, bool defensorEsBlanco)
    {
        return EsJaqueEnMatriz(tableroLogico.casillas, defensorEsBlanco);
    }

    private static bool EsJaqueEnMatriz(Pieza[,] tablero, bool defensorEsBlanco)
    {
        Vector2Int? rey = BuscarPosicionRey(tablero, defensorEsBlanco);
        if (!rey.HasValue)
        {
            return false;
        }

        return EsCasillaAtacada(tablero, rey.Value, defensorEsBlanco);
    }

    public static bool EsCasillaAtacada(Pieza[,] tablero, Vector2Int casilla, bool defensorEsBlanco)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Pieza p = tablero[x, y];
                if (p == null || p.esBlanca == defensorEsBlanco)
                {
                    continue;
                }

                List<Vector2Int> ataques = p.CalcularMovimientosPosibles(tablero, null);
                if (ataques.Contains(casilla))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// El rey no puede enrocar estando en jaque ni pasar por casillas atacadas ni quedar en jaque (las dos casillas del trayecto).
    /// </summary>
    private static bool EnroqueCumpleCasillasNoAtacadas(Pieza[,] tablero, Vector2Int origenRey, Vector2Int destinoRey, bool reyEsBlanco)
    {
        if (EsJaqueEnMatriz(tablero, reyEsBlanco))
        {
            return false;
        }

        int paso = destinoRey.x > origenRey.x ? 1 : -1;
        Vector2Int casillaIntermedia = new Vector2Int(origenRey.x + paso, origenRey.y);
        Vector2Int casillaFinal = new Vector2Int(origenRey.x + 2 * paso, origenRey.y);

        if (EsCasillaAtacada(tablero, casillaIntermedia, reyEsBlanco))
        {
            return false;
        }

        if (EsCasillaAtacada(tablero, casillaFinal, reyEsBlanco))
        {
            return false;
        }

        return true;
    }

    public static Vector2Int? BuscarPosicionRey(Pieza[,] tablero, bool esBlanco)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Pieza p = tablero[x, y];
                if (p is Rey && p.esBlanca == esBlanco)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    public static bool TieneAlgunMovimientoLegal(TableroLogico tableroLogico, bool esBlanco)
    {
        Pieza[,] tablero = tableroLogico.casillas;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Pieza p = tablero[x, y];
                if (p == null || p.esBlanca != esBlanco)
                {
                    continue;
                }

                Vector2Int origen = new Vector2Int(x, y);
                List<Vector2Int> legales = FiltrarMovimientosLegales(p, origen, tableroLogico);
                if (legales.Count > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void EvaluarEstadoTrasTurno(TableroLogico tableroLogico, bool bandoQueDebeMoverEsBlanco, out bool enJaque, out bool mate, out bool ahogado)
    {
        enJaque = EsJaque(tableroLogico, bandoQueDebeMoverEsBlanco);
        bool puedeMover = TieneAlgunMovimientoLegal(tableroLogico, bandoQueDebeMoverEsBlanco);

        mate = enJaque && !puedeMover;
        ahogado = !enJaque && !puedeMover;
    }
}
