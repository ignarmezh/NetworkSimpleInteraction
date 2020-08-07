using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{

	private List<GameObject> selectedPlayers;

	[HideInInspector]
	public List<GameObject> selectablePlayers;
	private Vector3 mousePos1;
	private Vector3 mousePos2;

	public int GetCountSelectedPlayers()
	{
		return selectedPlayers.Count;
	}

	private void Awake()
	{
		selectedPlayers = new List<GameObject>();
		selectablePlayers = new List<GameObject>();
	}
	private void Update()
	{
		#region Обработка выделения объектов

		if ( Input.GetMouseButtonDown( 0 ) )
		{
			mousePos1 = Camera.main.ScreenToViewportPoint( Input.mousePosition );

			RaycastHit hit;
			if ( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out hit, Mathf.Infinity ) )
			{
				if ( Input.GetKey( KeyCode.LeftControl ) )
				{
					if ( hit.collider.GetComponent<PlayerContr>() != null )
					{
						//выделение только своего персонажа
						if ( Client.instance.myId == hit.collider.GetComponent<PlayerManager>().id )
						{
							hit.collider.GetComponent<PlayerManager>().ChangeState();
							selectedPlayers.Add( hit.transform.gameObject );
						}
					}
				}
				else
				{
					ClearSelected();

					if ( hit.collider.GetComponent<PlayerContr>() != null )
					{
						//выделение только своего персонажа
						if ( Client.instance.myId == hit.collider.GetComponent<PlayerManager>().id )
						{
							hit.collider.GetComponent<PlayerManager>().ChangeState();
							selectedPlayers.Add( hit.transform.gameObject );
						}
					}
				}
			}
			else
			{
				ClearSelected();
			}
		}

		if ( Input.GetMouseButtonUp( 0 ) )
		{
			mousePos2 = Camera.main.ScreenToViewportPoint( Input.mousePosition );

			if ( mousePos1 != mousePos2 )
			{
				SelectPlayers();
			}
		}

		#endregion


		if ( Input.GetKey( KeyCode.Escape ) )
		{
			Application.Quit();
		}
	}

	//не совсем понял задачу сразу, думал мы можем выделать нескольких игроков, поэтому делал выделение нескольких игроков, убирать не стал
	private void SelectPlayers()
	{
		List<GameObject> remPlayers = new List<GameObject>();

		if ( Input.GetKey( KeyCode.LeftControl ) == false )
		{
			ClearSelected();
		}

		Rect selectRect = new Rect( mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y );

		foreach ( GameObject selectPlayer in selectablePlayers )
		{
			if ( selectPlayer != null )
			{
				if ( selectRect.Contains( Camera.main.WorldToViewportPoint( selectPlayer.transform.position ), true ) )
				{
					if ( Client.instance.myId == selectPlayer.GetComponent<PlayerManager>().id )
					{
						selectedPlayers.Add( selectPlayer );
						selectPlayer.GetComponent<PlayerManager>().ChangeState();
					}
				}
			}
			else
			{
				remPlayers.Add( selectPlayer );
			}
		}

		if ( remPlayers.Count > 0 )
		{
			foreach ( GameObject rem in remPlayers )
			{
				selectablePlayers.Remove( rem );
			}

			remPlayers.Clear();
		}
	}

	private void ClearSelected()
	{
		foreach ( GameObject oneplayer in selectedPlayers )
		{
			oneplayer.GetComponent<PlayerManager>().ChangeState();
		}
		selectedPlayers.Clear();
	}
}
