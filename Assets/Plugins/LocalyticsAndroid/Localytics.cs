using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID

namespace Prime31
{
	public class Localytics
	{
		private static AndroidJavaObject _plugin;
	
	
		static Localytics()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.LocalyticsPlugin" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}
	
	
	 	// Initializes the Localytics SDK. Call on application startup before calling any other methods. The app key is only required on iOS.
	 	public static void init( string thisJustForApiCompatibility = null )
	 	{
	 		if( Application.platform != RuntimePlatform.Android )
	 			return;
	
	 		_plugin.Call( "init" );
	 	}
	
	
	 	// Sets a custom dimension. Note that this must be called after init and before startSession!
		public static void setCustomDimension( int dimension, string value )
		{
	 		if( Application.platform != RuntimePlatform.Android )
	 			return;
	
	 		_plugin.Call( "setCustomDimension", dimension, value );
		}
	
	
	 	// Starts up the Localytics session. The session will be automatically resumed/closed when appropriate and data will automatically be uploaded for you.
	 	public static void startSession()
	 	{
	 		if( Application.platform != RuntimePlatform.Android )
	 			return;
	
	 		_plugin.Call( "startSession" );
	 	}
	
	
		// Enables or disables logging in the Localytics SDK
		public static void setLoggingEnabled( bool shouldEnable )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "setLoggingEnabled", shouldEnable );
		}
	
	
		// Sets the customerId
		public static void setCustomerId( string customerId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "setCustomerId", customerId );
		}
	
	
		// Sets the value for a specific identifier
		public static void setValueForIdentifier( string identifier, string value )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "setValueForIdentifier", identifier, value );
		}
	
	
		// Tags an event and records it on the server
		public static void tagEvent( string eventName )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "tagEvent", eventName );
		}
	
	
		// Tags an event with key/value pair attributes
		public static void tagEventWithAttributes( string eventName, Dictionary<string,string> attributes )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			if( attributes == null )
			{
				tagEvent( eventName );
				return;
			}
	
			_plugin.Call( "tagEventWithAttributes", eventName, attributes.toJson() );
		}
	
	
		// Tags an event with key/value pair attributes and customer value increase
		public static void tagEventWithAttributesAndCustomerValue( string eventName, Dictionary<string,string> attributes, long customerValueIncrease )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			if( attributes == null )
			{
				tagEvent( eventName );
				return;
			}
	
			_plugin.Call( "tagEventWithAttributesAndCustomerValue", eventName, attributes.toJson(), customerValueIncrease );
		}
	
	
		// Tags a screen and records it on the server
		public static void tagScreen( string screenName )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;
	
			_plugin.Call( "tagScreen", screenName );
		}
	
	}

}
#endif
