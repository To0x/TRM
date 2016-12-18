using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GridRow : IEnumerable<GridRow>
{
    // Die Maximale Bootstrap-Größe einer Zeile ist auf 12 beschränkt.
    public const int MAX_BOOTSTRAP_SIZE = 12;
    const int GRID_ELEMENTS = 3;

    // Eine Zeile besteht immer aus 3 Elementen
    public GridElement[] elements = new GridElement[GRID_ELEMENTS];

    public GridRow()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = new GridElement();
        }
    }

    public IEnumerator<GridRow> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}

