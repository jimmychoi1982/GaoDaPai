using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class AnimationCallbackManager : StateMachineBehaviour {
	// Current callback to execute
	private string CurrentAnimation = null;
	public Action<AnimatorStateInfo> OnComplete = null;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (OnComplete != null) {
			OnComplete.Invoke(stateInfo);
		}
	}
	
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
	
	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	//
	public static IEnumerator StartTrigger(Animator animator, string name, Action cb = null) {
		int waitLoops = 0;

		// Make sure the callback manager has initialized
		AnimationCallbackManager callbackManager = animator.GetBehaviour<AnimationCallbackManager>();
		while (callbackManager == null) {
			yield return null;
			if (animator == null) {
				yield break;
			}

			callbackManager = animator.GetBehaviour<AnimationCallbackManager>();

			waitLoops += 1;
			if (waitLoops % 100 == 0) {
				Debug.LogWarning("[" + name + "] waiting for callbackManager for over 100 ticks");
			}
		}

		// Wait for any running animations to complete first
		while (callbackManager.OnComplete != null) {
			yield return null;

			waitLoops += 1;
			if (waitLoops % 100 == 0) {
				Debug.LogWarning("[" + name + "] waiting for another animation '" + callbackManager.CurrentAnimation + "' for over 100 ticks");
			}
		}
		
		// Set animation state machine callback
		bool animationComplete = false;
		callbackManager.CurrentAnimation = name;
		callbackManager.OnComplete = (AnimatorStateInfo stateInfo) => {
			// Make sure this is the onComplete event for the correct animation state
			if (!stateInfo.IsName(name)) {
				return;
			}
			
			// Reset event and callback
			animationComplete = true;
			callbackManager.CurrentAnimation = null;
			callbackManager.OnComplete = null;
			if (cb != null) {
				cb();
			}
		};
		
		// Trigger state machine
		animator.SetTrigger(name);

		// Wait for animation to complete
		while (!animationComplete) {
			yield return null;
		}
	}
}