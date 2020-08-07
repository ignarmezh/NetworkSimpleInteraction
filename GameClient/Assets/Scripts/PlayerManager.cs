using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
	idle = 0,
	move = 1,
}

public class PlayerManager : MonoBehaviour
{
	public int id;
	public string username;

	[SerializeField]
	private Material defaultMat;
	[SerializeField]
	private Material selectMat;

	[HideInInspector]
	public bool currentlySelected = false;
	[HideInInspector]
	public PlayerState currentState = PlayerState.idle;

	private MeshRenderer meshRend;

	List<Vector3> route = new List<Vector3>();
	private Vector3 startPosition;
	private Vector3 targetPosition;
	private float lerpTime = 0f;
	private int statePos;

	private void Start()
	{
		meshRend = GetComponent<MeshRenderer>();
		Camera.main.gameObject.GetComponent<MouseInput>().selectablePlayers.Add( this.gameObject );
		targetPosition = transform.position;
		startPosition = transform.position;
		statePos = 1;
	}

	private void Update()
	{
		if ( route.Count == 0 )
			return;
		targetPosition = route[statePos];
		if ( targetPosition != transform.position )
		{
			transform.position = Vector3.Lerp( startPosition, targetPosition, lerpTime );
			lerpTime += Time.deltaTime * 2;
		}
		else
		{
			lerpTime = 0f;
			if ( statePos < route.Count - 1 )
				statePos++;
			else
			{
				statePos = 1;
				route.Clear();
				currentState = PlayerState.idle;
				GetComponent<Animator>().SetBool( "Moving", false );
			}
			startPosition = targetPosition;
		}
	}

	public void ChangeState()
	{
		currentlySelected = !currentlySelected;
		if ( !currentlySelected )
		{
			meshRend.material = defaultMat;
		}
		else
		{
			meshRend.material = selectMat;
		}
	}

	public void WayOnRoute( List<Vector3> _route )
	{
		route = _route;
		currentState = PlayerState.move;
		GetComponent<Animator>().SetBool( "Moving", true );
	}
}
