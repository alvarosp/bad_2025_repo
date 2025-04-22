using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public int Id {  get; set; }
    public Slider Slider { get; set; }
    public Player Owner { get; set; }
    public int Hp { get; set; }
    public int Movement { get; set; }
    public Vector3Int Position { get; set; }
    public UnitType Type { get; set; }

    public bool Damage(int damage)
    {
        Hp = Mathf.Max(Hp - damage, 0);
        Slider.value = Hp;
        DBManager.Instance.UpdateUnitHp(this);
        return Hp > 0;
    }

    public void Heal(int heal)
    {
        if (Movement == Type.MovementMax)
        {
            Hp = Mathf.Min(Hp + heal, Type.HpMax);
            Slider.value = Hp;
            DBManager.Instance.UpdateUnitHp(this);
        }
    }

    internal bool MoveTo(Vector3Int destinationCell, int moveCost)
    {
        if (Movement >= moveCost)
        {
            Movement -= moveCost;
            Position = destinationCell;
            DBManager.Instance.UpdateUnitPosition(this);
            return true;
        }
        Debug.LogError(this + " has not enough movement");
        return false;
    }

    public void endMovement()
    {
        Movement = 0;
        DBManager.Instance.UpdateUnitMovement(this);
    }

    public void ResetMovement()
    {
        Movement = Type.MovementMax;
        DBManager.Instance.UpdateUnitMovement(this);
    }

    public string PanelInfo()
    {
        return $"HP: {Hp}/{Type.HpMax}\nMove: {Movement}/{Type.MovementMax}\nCombat: {Type.Combat}";
    }
    public override string ToString() {
        return $"id: {Id}, name: {Type.Name}, hp: {Hp}/{Type.HpMax}, move: {Movement}/{Type.MovementMax}, combat: {Type.Combat}, x:{Position.x}, y:{Position.y}";
    }
}
