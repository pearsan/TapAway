using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayGenerater : CubeGenerator
{
 
    public delegate void PuzzleGeneratedHandler();
    public event PuzzleGeneratedHandler OnPuzzleGenerated;

    
}
