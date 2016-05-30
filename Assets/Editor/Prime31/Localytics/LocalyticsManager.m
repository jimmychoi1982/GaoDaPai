//
//  LocalyticsMananer.m
//  Localytics
//
//  Created by Mike DeSaro on 9/16/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "LocalyticsManager.h"
#import "Localytics.h"


@implementation LocalyticsManager

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (LocalyticsManager*)sharedManager
{
	static LocalyticsManager *sharedManager = nil;
	
	if( !sharedManager )
		sharedManager = [[LocalyticsManager alloc] init];
	
	return sharedManager;
}


- (id)init
{
    self = [super init];
    if( self )
	{
		// hooks for starting/stopping localytics
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(appDidBecomeInactive:)
													 name:@"UIApplicationDidEnterBackgroundNotification"
												   object:nil];
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(appWillEnterForeground:)
													 name:@"UIApplicationWillEnterForegroundNotification"
												   object:nil];
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(appDidBecomeInactive:)
													 name:UIApplicationWillTerminateNotification
												   object:nil];
    }

    return self;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSNotification

- (void)appWillEnterForeground:(NSNotification*)note
{
	[Localytics openSession];
	[Localytics upload];
}


- (void)appDidBecomeInactive:(NSNotification*)note
{
	[Localytics closeSession];
	[Localytics upload];
}

@end
