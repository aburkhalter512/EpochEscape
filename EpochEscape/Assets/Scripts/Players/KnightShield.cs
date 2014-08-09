using UnityEngine;
using System.Collections;

public class KnightShield : MonoBehaviour
{
	private Player m_player;

	void Start()
	{
		Transform parent = transform.parent;
		
		if(parent != null)
			m_player = parent.GetComponent<Player>();
	}
}
