using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BoidMovement : MonoBehaviour
{
    public float speed = 5f;
    public float neighborDistance = 5f;
    public float separationDistance = 2f;
    public float maxSteerForce = 3f;
    public float targetBiasStrength = 0.1f;

    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 acceleration;
    private Transform player;

    private static List<BoidMovement> allBoids = new List<BoidMovement>();
    private static Dictionary<Vector3Int, List<BoidMovement>> spatialGrid = new Dictionary<Vector3Int, List<BoidMovement>>();
    private static float cellSize = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        allBoids.Add(this);
    }

    void OnDestroy()
    {
        allBoids.Remove(this);
        spatialGrid.Clear();
    }

    void OnDisable()
    {
        allBoids.Remove(this);
        spatialGrid.Clear();
    }

    void FixedUpdate()
    {
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        Vector3Int currentCell = GetCellPosition(transform.position);

        foreach (var cell in GetNeighboringCells(currentCell))
        {
            if (spatialGrid.ContainsKey(cell))
            {
                foreach (var boid in spatialGrid[cell])
                {
                    if (boid != this)
                    {
                        float distance = Vector3.Distance(transform.position, boid.transform.position);
                        if (distance < neighborDistance)
                        {
                            alignment += boid.rb.velocity;
                            cohesion += boid.transform.position;
                            if (distance < separationDistance)
                            {
                                separation += (transform.position - boid.transform.position) / distance;
                            }
                        }
                    }
                }
            }
        }

        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized * speed;
            cohesion = ((cohesion / neighborCount) - transform.position).normalized * speed;
            separation = (separation / neighborCount).normalized * speed;
        }

        Vector3 wanderForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * speed;

        Vector3 targetDirection = (player.position - transform.position).normalized * speed;
        Vector3 biasedDirection = Vector3.Lerp(wanderForce, targetDirection, targetBiasStrength);

        acceleration = alignment + cohesion + separation + biasedDirection;
        acceleration = Vector3.ClampMagnitude(acceleration, maxSteerForce);
        velocity = Vector3.ClampMagnitude(rb.velocity + acceleration, speed);

        rb.velocity = velocity;

        UpdateSpatialGrid();
    }


    Vector3Int GetCellPosition(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }

    List<Vector3Int> GetNeighboringCells(Vector3Int cell)
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    cells.Add(new Vector3Int(cell.x + x, cell.y + y, cell.z + z));
                }
            }
        }
        return cells;
    }

    void UpdateSpatialGrid()
    {
        Vector3Int currentCell = GetCellPosition(transform.position);
        if (!spatialGrid.ContainsKey(currentCell))
        {
            spatialGrid[currentCell] = new List<BoidMovement>();
        }
        spatialGrid[currentCell].Add(this);
    }
}