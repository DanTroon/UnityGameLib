using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityGameLib.Editor.Commands {
	/// <summary>
	/// Provides menu items and commands for object-specific operations.
	/// </summary>
	public class ObjectCommands : MonoBehaviour {
		
		[MenuItem("CONTEXT/Animator/Zero All Transition Times", false, 201)]
		public static void CommandZeroTransitionTimes(MenuCommand command) {
			Animator animator = command.context as Animator;
			AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
			
			ForEachControllerTransition(controller, (transition) => {
				transition.duration = 0f;
				transition.offset = 0f;
			});
		}

		[MenuItem("CONTEXT/Animator/Add non-conditional Exit Time", false, 202)]
		public static void CommandAddExitTimes(MenuCommand command) {
			Animator animator = command.context as Animator;
			AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
			
			ForEachControllerTransition(controller, (transition) => {
				if (transition.conditions.Length == 0) {
					transition.exitTime = 0f;
					transition.hasExitTime = true;
				}
			});
		}

		[MenuItem("CONTEXT/Animator/Remove conditional Exit Time", false, 203)]
		public static void CommandRemoveExitTimes(MenuCommand command) {
			Animator animator = command.context as Animator;
			AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

			ForEachControllerTransition(controller, (transition) => {
				if (transition.conditions.Length > 0) {
					transition.exitTime = 0f;
					transition.hasExitTime = false;
				}
			});
		}

		[MenuItem("CONTEXT/Animator/[Repair All]", false, 210)]
		public static void CommandRepairAll(MenuCommand command) {
			Animator animator = command.context as Animator;
			AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

			ForEachControllerTransition(controller, (transition) => {
				transition.duration = 0f;
				transition.offset = 0f;
				transition.exitTime = 0f;
				transition.hasExitTime = transition.conditions.Length == 0;
			});
		}

		[MenuItem("CONTEXT/Animator/Zero All Transition Times", true, 201)]
		[MenuItem("CONTEXT/Animator/Add non-conditional Exit Time", true, 202)]
		[MenuItem("CONTEXT/Animator/Remove conditional Exit Time", true, 203)]
		[MenuItem("CONTEXT/Animator/[Fix All]", true, 210)]
		private static bool ValidateNonOverrideAnimatorController(MenuCommand command) {
			Animator animator = command.context as Animator;
			if (!animator)
				return false;
			if (!animator.runtimeAnimatorController)
				return false;
			if (animator.runtimeAnimatorController is AnimatorOverrideController)
				return false;
			if (!(animator.runtimeAnimatorController as AnimatorController))
				return false;
			return true;
		}

		private static void ForEachControllerTransition(AnimatorController controller, Action<AnimatorStateTransition> callback) {
			foreach (AnimatorControllerLayer layer in controller.layers) {
				foreach (AnimatorStateTransition transition in layer.stateMachine.anyStateTransitions) {
					callback(transition);
				}
				foreach (ChildAnimatorState state in layer.stateMachine.states) {
					foreach (AnimatorStateTransition transition in state.state.transitions) {
						callback(transition);
					}
				}
			}
		}
	}
}
