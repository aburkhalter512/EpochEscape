using UnityEngine;
using System.Collections;

public class DetectionArea : MonoBehaviour
{
    #region Instance Variables
	protected Player m_player;
	protected GameObject m_parent;
	protected SpriteRenderer m_parentRenderer;
	
	protected bool audioIsPlaying = false;
    #endregion
	
    #region Class Constants
	public static readonly Color BLUE = new Color(0.33f, 0.5f, 0.78f, 0.33f);
	public static readonly Color RED = new Color(1f, 0f, 0.12f, 0.45f);
	
	public enum ColorStatus
	{
		NORMAL,
		CAUTION
	};
    #endregion
	
	protected void Start()
	{
		m_parent = null;
		
		if(transform.parent != null)
		{
			m_parent = transform.parent.gameObject;
			m_parentRenderer = GetComponent<SpriteRenderer>();
			
			if(m_parentRenderer != null)
				m_parentRenderer.color = BLUE;
		}
	}
	
    #region Instance Methods
	protected void ChangeColor(ColorStatus colorStatus)
	{
		int parentNumChildren = m_parent.transform.childCount;
		
		if(parentNumChildren == 0) return;
		
		Transform currentChild = null;
		SpriteRenderer currentRenderer = null;
		
		for(int i = 0; i < parentNumChildren; i++)
		{
			currentChild = m_parent.transform.GetChild(i);
			currentRenderer = currentChild.GetComponent<SpriteRenderer>();
			
			if(currentRenderer != null)
			{
				if(colorStatus == ColorStatus.NORMAL)
					currentRenderer.color = BLUE;
				else
					currentRenderer.color = RED;
			}
		}
	}
	
	protected void OnTriggerStay2D(Collider2D other)
	{
        Player player = other.GetComponent<Player>();

		if(player != null)
		{	
			if(!(m_parent == null || player == null))
			{
				ChangeColor(ColorStatus.CAUTION);
					
				player.detect();
					
				if(m_parent != null)
				{
					if(m_parent.tag == "SecurityCamera")
					{
						if (!audioIsPlaying)
                        {
							GetComponent<AudioSource>().Play ();
							audioIsPlaying = true;
						}
					}
				}
			}
		}
	}
	
	protected void OnTriggerExit2D(Collider2D other)
	{
        Player player = other.GetComponent<Player>();

		if(player != null)
		{
				ChangeColor(ColorStatus.NORMAL);

				if(m_parent != null)
				{
					if(m_parent.tag == "SecurityCamera")
					{
						GetComponent<AudioSource>().Stop();
						audioIsPlaying = false;
					}
				}
			}
    }
    #endregion
}
