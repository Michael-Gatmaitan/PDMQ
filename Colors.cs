using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Color brown = new();

    public Color yellow = new();
    public Color drkYellow = new();
    public Color midYellow = new();

    // Hover colors
    public Color hoverBrown = new();
    public Color hoverYellow = new();
}
