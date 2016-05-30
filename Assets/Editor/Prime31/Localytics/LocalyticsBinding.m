//
//  LocalyticsBinding.m
//  Localytics
//
//  Created by Mike DeSaro on 9/16/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "Localytics.h"
#import "LocalyticsManager.h"



// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil



NSObject* _localyticsObjectFromJsonString( NSString* json )
{
	NSError *error = nil;
	NSData *data = [NSData dataWithBytes:json.UTF8String length:json.length];
    NSObject *object = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingAllowFragments error:&error];
	
	if( error )
		NSLog( @"failed to deserialize JSON: %@ with error: %@", json, error );
		
	return object;
}



// Initializes the Localytics SDK.  Call on application startup.
void _localyticsInit( const char *apiKey )
{
	NSString *key = GetStringParam( apiKey );
	[Localytics integrate:key];
}


// Sets a custom dimension. Note that this must be called after init and before startSession!
void _localyticsSetCustomDimension( int dimension, const char * value )
{
	[Localytics setValue:GetStringParam( value ) forCustomDimension:dimension];
}


// Starts up the Localytics session. The session will be automatically resumed/closed when appropriate and data will automatically be uploaded for you.
void _localyticsStartSession()
{
	[Localytics openSession];
	[Localytics upload];
	[LocalyticsManager sharedManager];
}


void _localyticsSetLoggingEnabled( BOOL shouldEnable )
{
	[Localytics setLoggingEnabled:shouldEnable];
}


// Sets the customerId
void _localyticsSetCustomerId( const char * customerId )
{
	[Localytics setCustomerId:GetStringParamOrNil( customerId )];
}


// Sets the value for a specific identifier
void _localyticsSetValueForIdentifier( const char * identifier, const char * value )
{
	[Localytics setValue:GetStringParam( value ) forIdentifier:GetStringParam( identifier )];
}


// Tags an event
void _localyticsTagEvent( const char * eventName )
{
	[Localytics tagEvent:GetStringParam( eventName )];
}


// Tags an event with optional key/value pairs
void _localyticsTagEventWithAttributes( const char * eventName, const char * params )
{
	NSDictionary *dict = nil;
	NSString *json = GetStringParamOrNil( params );
	if( json )
		dict = (NSDictionary*)_localyticsObjectFromJsonString( json );

	[Localytics tagEvent:GetStringParam( eventName ) attributes:dict];
}


// Tags an event with optional key/value pairs, report attributes and customer value increase
void _localyticsTagEventWithAttributesAndCustomerValue( const char * eventName, const char * params, double customerValueIncrease )
{
	NSDictionary *paramsDict = nil;
	NSString *json = GetStringParamOrNil( params );
	if( json )
		paramsDict = (NSDictionary*)_localyticsObjectFromJsonString( json );

	[Localytics tagEvent:GetStringParam( eventName ) attributes:paramsDict customerValueIncrease:[NSNumber numberWithDouble:customerValueIncrease]];
}


// Tags a screen
void _localyticsTagScreen( const char * screenName )
{
	[Localytics tagScreen:GetStringParam( screenName )];
}



