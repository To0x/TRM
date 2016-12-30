﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TUI_Web.Settings;

public class GridRow : IEnumerable<GridRow>
{
    // Eine Zeile besteht immer aus 3 Elementen
    public GridElement[] elements = new GridElement[SettingsControler.GRID_ELEMENTS];

	public GridRow(GridElement[] elements = null)
    {
		if (elements == null)
		{
			this.elements = new GridElement[SettingsControler.GRID_ELEMENTS];
			for (int i = 0; i < SettingsControler.GRID_ELEMENTS; i++)
			{
				this.elements[i] = new GridElement();
			}
		}
		else
		{
			this.elements = elements;
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

