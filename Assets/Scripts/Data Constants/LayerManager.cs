using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LayerManager 
{
    public static int EnvironmentLayer { get { return 1 << 7; } }
    public static int UILayer{ get { return 1 << 5; } }
}
