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
    public int elementCount = -1;

    private int affectedCount = 0;
    private bool affected = false;

	public GridRow(GridElement[] elements = null)
    {
		if (elements == null)
		{
			this.elements = new GridElement[SettingsControler.GRID_ELEMENTS];
			for (int i = 0; i < SettingsControler.GRID_ELEMENTS; i++)
			{
				this.elements[i] = new GridElement();
			}
            elementCount = SettingsControler.GRID_ELEMENTS;
		}
		else
		{
			this.elements = elements;
            elementCount = elements.Length;
		}
    }

    public int getDecreasableElementCount(GridElement sender)
    {
        int count = 0;
        foreach (GridElement element in elements)
        {
            GridElement checkElement = null;
            if (element.getElement() != null)
                checkElement = element.getElement();
            else
                checkElement = element;

            if (checkElement == sender)
                continue;

            if (checkElement.size > SettingsControler.MINIMUN_ELEMENT_SIZE)
                count++;
        }

        return count;
    }

    private void affectRow(bool affected)
    {
        if (affected)
        {
            this.affected = true;
            foreach (GridElement element in elements)
            {
                // if there is already a element, we do not need to create new one
                if (element.getElement() == null)
                {
                    element.setElement(new OverlayElement());
                    element.getElement().type = element.type;
                    element.getElement().size = element.size;
                    element.getElement().cursor = element.cursor;
                }
            }
        }
        else
        {
            this.affected = false;
            foreach (GridElement element in elements)
            {
                element.setElement(null);
            }
        }
    }

    public void increaseSizeAffected()
    {
        affectedCount++;
        if (affectedCount > 0)
            affectRow(true);
        else
            affectRow(false);
    }

    public int getIncreasableElementCount(GridElement sender)
    {
        int count = 0;
        foreach (GridElement element in elements)
        {
            GridElement checkElement = null;
            if (element.getElement() != null)
                checkElement = element.getElement();
            else
                checkElement = element;

            if (checkElement == sender)
                continue;

            if (checkElement.size != 0 &&
                checkElement.size < SettingsControler.MAXIMUM_ELEMENT_SIZE)
                count++;
        }

        return count;
    }

    public int getSmallestSize()
    {
        int smallest = SettingsControler.MAXIMUM_ELEMENT_SIZE;
        foreach (GridElement element in elements)
        {
            GridElement checkElement = null;
            if (element.getElement() != null)
                checkElement = element.getElement();
            else
                checkElement = element;


            if (checkElement.size != 0 && checkElement.size < smallest)
                smallest = checkElement.size;
        }

        return smallest;
    }

    public int getBiggestSize()
    {
        int biggest = SettingsControler.MINIMUN_ELEMENT_SIZE;
        foreach (GridElement element in elements)
        {
            if (element.getElement().size != SettingsControler.MAXIMUM_ELEMENT_SIZE && element.size > biggest)
                biggest = element.getElement().size;
        }

        return biggest;
    }

    public GridElement getSmallestElement()
    {
        int smallest = SettingsControler.MAXIMUM_ELEMENT_SIZE;
        GridElement checkElement = null;
        GridElement smallestElement = null;

        foreach (GridElement element in elements)
        {
            if (element.getElement() != null)
                checkElement = element.getElement();
            else
                checkElement = element;


            if (checkElement.size != 0 && checkElement.size <= smallest)
            {
                smallestElement = element;
                smallest = checkElement.size;

            }
        }
        return smallestElement;
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

