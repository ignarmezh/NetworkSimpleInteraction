using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{

	private static void SendTCPData( Packet _packet )
	{
		_packet.WriteLength();
		Client.instance.tcp.SendData( _packet );
	}

	private static void SendUDPData( Packet _packet )
	{
		_packet.WriteLength();
		Client.instance.udp.SendData( _packet );
	}

	#region Packets
	public static void WelcomeReceived()
	{
		using ( Packet _packet = new Packet( ( int )ClientPackets.welcomeReceived ) )
		{
			_packet.Write( Client.instance.myId );
			if ( UIManager.instance.usernameField.text != "" )
				_packet.Write( UIManager.instance.usernameField.text );
			else
				_packet.Write( "DefaultName" + Client.instance.myId );

			SendTCPData( _packet );
		}
	}

	public static void PlayerTargetPosition( Vector3 _targetPosition )
	{
		using ( Packet _packet = new Packet( ( int )ClientPackets.targetPosition ) )
		{
			_packet.Write( _targetPosition );

			SendTCPData( _packet );
		}
	}
	#endregion
}
