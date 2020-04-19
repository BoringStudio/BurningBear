using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void OnMouseClickDown(Player player, Vector3 position);
    void OnMouseClickUp(Player player, Vector3 position);
    void OnInteractStart(Player player);
	void OnInteractEnd(Player player);
}
