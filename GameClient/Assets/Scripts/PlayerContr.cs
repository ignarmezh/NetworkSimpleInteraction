using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContr : MonoBehaviour
{

	private void Update()
	{
		CheckInputMouse();
	}

	private void CheckInputMouse()
	{
		if ( Input.GetMouseButtonDown( 1 ) && Camera.main.gameObject.GetComponent<MouseInput>().GetCountSelectedPlayers() == 1 &&
			GameManager.instance.isQueue == gameObject.GetComponent<PlayerManager>().id && gameObject.GetComponent<PlayerManager>().currentState == PlayerState.idle )
		{
			RaycastHit hit;
			if ( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, Mathf.Infinity ) )
			{
				if ( hit.transform.tag == "Player" || hit.transform.tag == "Point" )
				{
					ClientSend.PlayerTargetPosition( hit.transform.position );
				}
			}
		}
	}
}
