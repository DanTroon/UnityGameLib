using System;
using UnityEngine;

namespace UnityGameLib.Utilities.Yields {
	/// <summary>
	/// A yield instruction that uses a static callback, allowing all instances to share a common
	/// <see cref="evaluator"/>.
	/// </summary>
	/// <remarks>
	/// If <see cref="evaluator"/> is never set, it will always return <c>false</c>, which means
	/// code execution will always continue on the next frame, similar to <c>yield return null</c>.
	/// The static callback can be changed at any time, including while a <see cref="DefaultYield"/>
	/// is waiting, to change the behavior for the next evaluation.
	/// </remarks>
	/// <example>
	/// Simple "pause" functionality can be easily implemented as a static <see cref="evaluator"/>:
	/// <code>
	/// DefaultWait.evaluator = () => MyGame.isPaused;
	/// </code>
	/// Or a dynamic <see cref="evaluator"/>:
	/// <code>
	/// public bool isPaused {
	///		set { DefaultWait.evaluator = value ? DefaultWait.HALT : DefaultWait.PROCEED; }
	/// }
	/// </code>
	/// </example>
	public class DefaultYield : CustomYieldInstruction {
		/// <summary>An <see cref="evaluator"/> callback that halts code execution indefinitely.</summary>
		public static readonly Func<bool> HALT = () => true;

		/// <summary>An <see cref="evaluator"/> callback that allows code execution to continue.</summary>
		public static readonly Func<bool> PROCEED = () => false;

		/// <summary>
		/// An instance of the DefaultYield instruction. Use this to limit allocation,
		/// since all instances are alike.
		/// </summary>
		public static readonly DefaultYield instance = new DefaultYield();

		private static Func<bool> _evaluator = PROCEED;
		/// <summary>
		/// A callback providing the wait condition for all <see cref="DefaultYield"/> instructions.
		/// </summary>
		/// <remarks>
		/// The callback should return <c>true</c> to keep waiting or <c>false</c> to resume execution.
		/// </remarks>
		public static Func<bool> evaluator {
			get { return _evaluator; }
			set { _evaluator = value ?? PROCEED; }
		}

		public override bool keepWaiting {
			get { return _evaluator(); }
		}
	}
}
