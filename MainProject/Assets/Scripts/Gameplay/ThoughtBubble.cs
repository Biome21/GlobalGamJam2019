using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
	public List<Sprite> m_ThoughtBubbleSpite = new List<Sprite> ();
	public SpriteRenderer m_BubbleIcon;

	public void SetIcon(int fatness)
	{
		m_BubbleIcon.sprite = m_ThoughtBubbleSpite [fatness];
	}
}
