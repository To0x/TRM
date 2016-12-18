﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TUI_Web;

public enum ElementTypes { None, Topic, Text, Graphic };

public class GridElement
{
	// Die Größe des Bootstrap-Grids
	// Bsp. col-md-4
	public int size = 4;

	// ist die Zelle markiert?
	// wird auf der Seite hervorgehoben dargestellt
	public bool cursor = false;

	// was für ein Typ ist das Element?
	public ElementTypes type = ElementTypes.None;


	/*public GridElement(long s_id, int f_id, float xpos, float ypos, float angle) : base(s_id, f_id, xpos, ypos, angle)
    {
    }

    public GridElement(TuioObject o) : base(o)
    {
    */
	public void paint(Graphics g)
	{
		// needed?
	}

}
