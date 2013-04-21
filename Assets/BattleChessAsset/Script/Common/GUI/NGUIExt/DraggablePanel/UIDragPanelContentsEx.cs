using UnityEngine;
using System.Collections;


/// <summary>
/// Allows dragging of the specified target panel's contents by mouse or touch.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Panel Contents Ex")]
public class UIDragPanelContentsEx : MonoBehaviour
{
	/// <summary>
	/// This panel's contents will be dragged by the script.
	/// </summary>

	public UIDraggablePanelEx draggablePanel;

	// Version 1.92 and earlier referenced the panel instead of csDraggableContent script.
	[HideInInspector][SerializeField] UIPanel panel;

	/// <summary>
	/// Backwards compatibility.
	/// </summary>

	void Awake ()
	{
		// Legacy functionality support for backwards compatibility
		if (panel != null)
		{
			if (draggablePanel == null)
			{
				draggablePanel = panel.GetComponent<UIDraggablePanelEx>();

				if (draggablePanel == null)
				{
					draggablePanel = panel.gameObject.AddComponent<UIDraggablePanelEx>();
				}
			}
			panel = null;
		}
	}

	/// <summary>
	/// Automatically find the draggable panel if possible.
	/// </summary>

	void Start ()
	{
		if (draggablePanel == null)
		{
			draggablePanel = NGUITools.FindInParents<UIDraggablePanelEx>(gameObject);
		}
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	void OnPress (bool pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Press(pressed);
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Drag(delta);
		}
	}

	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	void OnScroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && draggablePanel != null)
		{
			draggablePanel.Scroll(delta);
		}
	}
}
