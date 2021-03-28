using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANTBlock : AbstractBlock

{
    #region Fields

    public int health;
    public int health_share_chance;
    public int health_share_with_queen_chance;
    public int queen_health_share_chance;
    public int eatChance;
    public int digChance;
    public bool isqueen;
    /// <summary>
    /// Statically held tile map coordinate.
    /// </summary>
    private static Vector2 _tileMapCoordinate = new Vector2(0, 1);

    /// <summary>
    /// Statically held is visible.
    /// </summary>
    private static bool _isVisible = true;

    #endregion
    public override bool isVisible()
    {
        return _isVisible;
    }

    public override Vector2 tileMapCoordinate()
    {
        return _tileMapCoordinate;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
