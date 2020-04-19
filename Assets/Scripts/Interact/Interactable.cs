using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void OnMouseInteractStart(Player player, Vector3 position);
    void OnMouseInteractEnd(Player player, Vector3 position);
    void OnInteractStart(Player player);
	void OnInteractEnd(Player player);
}
