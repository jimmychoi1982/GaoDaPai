using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;



#if UNITY_IPHONE

namespace Prime31
{
	public class Localytics
	{
		[DllImport("__Internal")]
		private static extern void _localyticsInit( string appKey );
	
		// Initializes the Localytics SDK. Call on application startup before calling any other methods. The app key is only required on iOS.
		public static void init( string appKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsInit( appKey );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsSetCustomDimension( int dimension, string value );
	
		// Sets a custom dimension. Note that this must be called after init and before startSession!
		public static void setCustomDimension( int dimension, string value )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsSetCustomDimension( dimension, value );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsStartSession();
	
		// Starts up the Localytics session. The session will be automatically resumed/closed when appropriate and data will automatically be uploaded for you.
		public static void startSession()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsStartSession();
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsSetLoggingEnabled( bool shouldEnable );
	
		// Enables or disables logging in the Localytics SDK
		public static void setLoggingEnabled( bool shouldEnable )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsSetLoggingEnabled( shouldEnable );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsSetCustomerId( string customerId );
	
		// Sets the customerId
		public static void setCustomerId( string customerId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsSetCustomerId( customerId );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsSetValueForIdentifier( string identifier, string value );
	
		// Sets the value for a specific identifier
		public static void setValueForIdentifier( string identifier, string value )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsSetValueForIdentifier( identifier, value );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsTagEvent( string eventName );
	
		// Tags an event
		public static void tagEvent( string eventName )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsTagEvent( eventName );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsTagEventWithAttributes( string eventName, string attributes );
	
		// Tags an event with key/value pair attributes
		public static void tagEventWithAttributes( string eventName, Dictionary<string,string> attributes )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsTagEventWithAttributes( eventName, attributes.toJson() );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsTagEventWithAttributesAndCustomerValue( string eventName, string attributes, double customerValueIncrease );
	
		// Tags an event with key/value pair attributes and customer value increase
		public static void tagEventWithAttributesAndReportAttributes( string eventName, Dictionary<string,string> attributes, double customerValueIncrease )
		{
			if( attributes == null )
			{
				tagEvent( eventName );
				return;
			}
	
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsTagEventWithAttributesAndCustomerValue( eventName, attributes.toJson(), customerValueIncrease );
		}
	
	
		[DllImport("__Internal")]
		private static extern void _localyticsTagScreen( string screenName );
	
		// Tags a screen
		public static void tagScreen( string screenName )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_localyticsTagScreen( screenName );
		}
	
	}

}
#endif
