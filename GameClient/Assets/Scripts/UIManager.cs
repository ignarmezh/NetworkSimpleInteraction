using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public static UIManager instance;

	public GameObject startMenu;
	public InputField usernameField;
	public Text TextTurn;

	public GameObject StartPanel;
	public GameObject GamePanel;
	public GameObject fieldGO;
	public GameObject spherePrefab;

	public int sizeOfField = 0;

	private void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
		else if ( instance != this )
		{
			Debug.Log( "Instance already exists, destroing." );
			Destroy( this );
		}
	}

	public void ConnectToServer()
	{
		startMenu.SetActive( false );
		usernameField.interactable = false;
		Client.instance.ConnectToServer();
	}

	public void CreateMap()
	{
		for ( int i = 0; i < sizeOfField; i++ )
		{
			for ( int j = 0; j < sizeOfField; j++ )
			{
				GameObject newObj = Instantiate( spherePrefab, new Vector3( i, j, 0f ), Quaternion.identity );
				newObj.transform.parent = fieldGO.transform;
			}
		}
		Vector3 camPos = new Vector3( ( sizeOfField - 0 ) / 2f, ( sizeOfField - 0 ) / 2f, -15f );
		Camera.main.transform.position = camPos;
		StartPanel.SetActive( false );
		GamePanel.SetActive( true );
	}

	public void ChangeTurnText()
	{
		if ( GameManager.instance.isQueue == Client.instance.myId )
		{
			TextTurn.text = "Your turn";
			TextTurn.color = Color.green;
		}
		else
		{
			TextTurn.text = "Not your turn";
			TextTurn.color = Color.red;
		}
	}
}
