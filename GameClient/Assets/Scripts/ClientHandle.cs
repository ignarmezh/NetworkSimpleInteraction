using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{

	public static void Welcome( Packet _packet )
	{
		string msg = _packet.ReadString();
		int _myid = _packet.ReadInt();
		int _sizeOfField = _packet.ReadInt();

		Debug.Log( "Message from server: " + msg );
		Client.instance.myId = _myid;
		UIManager.instance.sizeOfField = _sizeOfField;
		UIManager.instance.CreateMap();

		ClientSend.WelcomeReceived();

		Client.instance.udp.Connect( ( ( IPEndPoint )Client.instance.tcp.socket.Client.LocalEndPoint ).Port );
	}

	public static void SpawnPlayer( Packet _packet )
	{
		int _id = _packet.ReadInt();
		string _username = _packet.ReadString();
		Vector3 _position = _packet.ReadVector3();
		Quaternion _quaternion = _packet.ReadQuaternion();

		GameManager.instance.SpawnPlayer( _id, _username, _position, _quaternion );
		UIManager.instance.ChangeTurnText();
	}

	public static void Route( Packet _packet )
	{
		int _id = _packet.ReadInt();
		int _count = _packet.ReadInt();
		List<Vector3> _route = new List<Vector3>();

		for ( int i = 0; i < _count; i++ )
		{
			_route.Add( _packet.ReadVector3() );
		}

		if ( GameManager.players.Count > 0 )
			GameManager.players[_id].WayOnRoute( _route );
	}

	public static void Disconnect(Packet _packet )
	{
		int _id = _packet.ReadInt();

		GameManager.instance.DeletePlayer( _id );
	}

	public static void QueuePlayer( Packet _packet )
	{
		int _id = _packet.ReadInt();

		GameManager.instance.isQueue = _id;
		UIManager.instance.ChangeTurnText();
	}
}
