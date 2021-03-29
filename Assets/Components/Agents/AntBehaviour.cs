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
        if (wm.GetBlock((int)transform.position.x, (int)(transform.position.y - 1), (int)transform.position.z) as AcidicBlock != null)
        {
            health -= 4;
        }
        else {
            health -= 2;
        }
        if (health <= 0)
        {
            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            int x = (int)(transform.position.x + RNG.Next(-10, 11));
            int y = (int)(transform.position.y + RNG.Next(-10, 11));
            int z = (int)(transform.position.z + RNG.Next(-10, 11));
            if (x >= xdimention || x < 0)
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
            if (wm.GetBlock(x, y, z) as AirBlock != null && wm.GetBlock(x, y - 1, z) as AirBlock == null)
            {
                wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                Vector3 v = new Vector3(x, y, z);
                transform.position = v;
                wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
            }
            int antsInSamePosition = wm.Positions[((int)transform.position.x), ((int)transform.position.y),( (int)transform.position.z)].Count;
            if(antsInSamePosition == 1)
            {
                if (wm.GetBlock(((int)transform.position.x), ((int)transform.position.y) -1, ((int)transform.position.z)) as MulchBlock != null) {
                    int rand = RNG.Next(100);
                    if (rand < eatChance) {
                        wm.SetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z), ((new AirBlock()) as AbstractBlock));
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                        Vector3 v = new Vector3(x, y-1, z);
                        transform.position = v;
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
                    }
                }
            }
        }

    }
}
