using System.Collections.Generic;
using UnityEngine;

public class AI
{
    MapManager map;

    public AI()
    {
        map = MapManager.Instance;
    }

    public List<Vector3Int> CalculatePath(Unit unit)
    {
        Dictionary<Vector3Int, int> dict = new();
        PriorityQueue<CellData> queue = new();
        CellData target = new CellData(null, unit.Position, 0, 0);
        dict.Add(target.pos, 0);
        queue.Enqueue(target, 0);
        while (queue.Count > 0)
        {
            CellData processingCell = queue.Dequeue();
            Unit unitInCell = map.mapUnits[processingCell.pos.x, processingCell.pos.y];
            if (unitInCell == null || unitInCell == unit)
            {
                List<Vector3Int> neighbors = map.GetValidNeighbors(processingCell.pos);
                foreach (var n in neighbors)
                {
                    int mCost = map.mapTerrains[n.x, n.y].MovementCost + processingCell.totalMovementCost;
                    int tDistance = processingCell.totalDistance + 1;
                    if (!dict.ContainsKey(n) && tDistance <= unit.Type.SightDistance)
                    {
                        dict.Add(n, mCost);
                        queue.Enqueue(new CellData(processingCell, n, mCost, tDistance), mCost);
                    }
                }
                target = processingCell;
            } else
            {
                if (unitInCell.Owner != unit.Owner)
                {
                    Debug.Log("Enemy detected at " + processingCell.pos);
                    target = processingCell;
                    break;
                }
            }
        }
        List<Vector3Int> path = GetParentsList(target);
        return path;
    }

    private List<Vector3Int> GetParentsList(CellData cellData)
    {
        List<Vector3Int> path = new()
        {
            cellData.pos
        };
        CellData parent = cellData.parent;
        while (parent != null)
        {
            path.Add(parent.pos);
            parent = parent.parent;
        }
        return path;
    }

    private void PrintQueue(CellData cellData)
    {
        CellData tmpData = cellData;
        string data = cellData.ToString();
        while (tmpData.parent != null)
        {
            tmpData = tmpData.parent;
            data += "\n" + tmpData.ToString();
        }
        Debug.Log(data);
    }

    class CellData
    {
        public Vector3Int pos {  get; private set; }
        public int totalMovementCost { get; private set; }
        public int totalDistance { get; private set; }
        public CellData parent;

        public CellData(CellData parent, Vector3Int pos, int movement, int distance)
        {
            this.parent = parent;
            this.pos = pos;
            this.totalMovementCost = movement;
            this.totalDistance = distance;
        }

        public override string ToString()
        {
            return $"pos: {pos}, tMovement: {totalMovementCost}, tDistance: {totalDistance}";
        }
    }
}
