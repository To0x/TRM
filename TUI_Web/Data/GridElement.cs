using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TUI_Web;
using TUI_Web.Settings;

public enum ElementTypes { None, Topic, Text, Graphic, Delete, Save, Remove, Unknown };
public enum ManipulationTypes { FontColor, FontStyle, LookAndFeel, Unknown };

public class OverlayElement : GridElement
{
    public OverlayElement()
    {
        setOverlay(null);
    }
}

public class GridElement
{
	// Die Größe des Bootstrap-Grids
	// Bsp. col-md-4 (errechnet sich aus Gesamtgröße / Anzahl Elemente
	public int size = SettingsControler.BOOTSTRAP_SIZE / SettingsControler.GRID_ELEMENTS;

	// ist die Zelle markiert?
	// wird auf der Seite hervorgehoben dargestellt
	public bool cursor = false;

	// was für ein Typ ist das Element?
	public ElementTypes type = ElementTypes.None;
    private ManipulationTypes manType = ManipulationTypes.Unknown;

    // existiert ein aktuelles Element auf der Oberfläche, welches das gesicherte Element überdeckt?
    private OverlayElement overlayElement = null;

    public GridElement()
    {
        cursor = SettingsControler.DEFAULT_CURSOR_STATE;
    }

	public void setCursor()
	{
		cursor = true;
	}

    public void setOverlay(OverlayElement element)
    {
        overlayElement = element;
    }

    public OverlayElement getOverlay()
    {
        return overlayElement;
    }

    public void setManipulationType(ManipulationTypes type)
    {
        manType = type;
    }

    public ManipulationTypes getManipulationType()
    {
        return manType;
    }
}
