using System;
using System.Collections.Generic;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods for handling methods and functors.
	/// </summary>
	public static class MethodUtilities {
		/// <summary>
		/// Generates a list of non-parameterized callbacks that pass each index to the specified callback.
		/// </summary>
		/// <param name="callback">A callback that takes an index as its only parameter</param>
		/// <param name="count">The number of indexed callbacks to create</param>
		/// <param name="startIndex">The index of the first callback</param>
		/// <returns>The list of non-parameterized callbacks</returns>
		public static List<Action> IndexCallbacks(Action<int> callback, int count, int startIndex = 0) {
			List<Action> result = new List<Action>(count - startIndex);

			for (int i = startIndex; i < count; ++i) {
				int localI = i;
				result.Add(() => callback(localI));
			}

			return result;
		}

		/// <summary>
		/// Generates a list of non-parameterized callbacks that pass each index to the specified callback.
		/// </summary>
		/// <typeparam name="TResult">The return type for the callbacks</typeparam>
		/// <param name="callback">A callback that takes an index as its only parameter</param>
		/// <param name="count">The number of indexed callbacks to create</param>
		/// <param name="startIndex">The index of the first callback</param>
		/// <returns>The list of non-parameterized callbacks</returns>
		public static List<Func<TResult>> IndexCallbacks<TResult>(Func<int, TResult> callback, int count, int startIndex = 0) {
			List<Func<TResult>> result = new List<Func<TResult>>(count - startIndex);

			for (int i = startIndex; i < count; ++i) {
				int localI = i;
				result.Add(() => { return callback(localI); });
			}

			return result;
		}
	}
}
