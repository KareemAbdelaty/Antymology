using System.Collections;
using System.Collections.Generic;
using Antymology.Terrain;
using UnityEngine;

public class AntBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public int health;
    public int health_share_chance;
    public int health_share_with_queen_chance;
    public int queen_health_share_chance;
    public int eatChance;
    public int digChance;
    public int xdimention;
    public int yimention;
    public int zdimention;
    public bool isqueen;
    public WorldManager wm;
    private System.Random RNG;
    void Start()
    {
        RNG = new System.Random();
        wm = GameObject.FindObjectOfType<WorldManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        int x = (int)(transform.position.x + RNG.Next(-2,3));
        int y = (int)(transform.position.y + RNG.Next(-2,3));
        int z = (int)(transform.position.z + RNG.Next(-2,3));
        if (x >= xdimention || x <0)
        {
            x = (int)transform.position.x;
        }
        if (y >= yimention || y < 0)
        {
            y = (int)transform.position.y;
        }
        if (z >= xdimention || z < 0)
        {
            z = (int)transform.position.z;
        }
        if (wm.GetBlock(x, y, z) as AirBlock != null && wm.GetBlock(x, y-1, z) as AirBlock == null) {
            Vector3 v = new Vector3(x, y, z);
            transform.position = v;
        }

    }
}
