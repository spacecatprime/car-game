using UnityEngine;
using System.Collections;

/// <summary>
/// quick input prototype
/// </summary>
public class ProtoInput : MonoBehaviour 
{
	public Transform Car;
	public Transform Grid;

	GGGrid m_grid;
	GGObject m_car;

	// Use this for initialization
	void Start () 
	{
		m_car = Car.GetComponent<GGObject>();
		m_grid = Grid.GetComponent<GGGrid>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.UpArrow))
		{
			var nextCell = m_car.Cell.GetCellInDirection(GGDirection.Down);
			m_car.CachedTransform.position = nextCell.CenterPoint3D;
		}
		else if(Input.GetKeyUp(KeyCode.DownArrow))
		{
			var nextCell = m_car.Cell.GetCellInDirection(GGDirection.Up);
			m_car.CachedTransform.position = nextCell.CenterPoint3D;
		}
		else if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			var nextCell = m_car.Cell.GetCellInDirection(GGDirection.Right);
			m_car.CachedTransform.position = nextCell.CenterPoint3D;
		}
		else if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			var nextCell = m_car.Cell.GetCellInDirection(GGDirection.Left);
			m_car.CachedTransform.position = nextCell.CenterPoint3D;
		}
		else if(Input.GetKeyUp(KeyCode.Delete))
		{
			Car.Rotate(0, -90, 0);
		}
		else if(Input.GetKeyUp(KeyCode.End))
		{
			Car.Rotate(0, 90, 0);
		}
	}
}
