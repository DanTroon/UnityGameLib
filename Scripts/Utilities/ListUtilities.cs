using System;
using System.Collections.Generic;

namespace UnityGameLib.Utilities {
	/// <summary>
	/// Provides static utility methods and extension methods for handling lists and other collections.
	/// </summary>
	public static class ListUtilities {
		/// <summary>
		/// Return the highest-value candidate from a list.
		/// </summary>
		/// <typeparam name="T">The type of the candidates</typeparam>
		/// <param name="candidates">The list of possible candidates</param>
		/// <param name="valuator">The function used to determine the value of each candidate</param>
		/// <param name="filter">An optional function to determine whether a candidate should be evaluated</param>
		/// <returns>The candidate for which the valuator function returned the highest value</returns>
		public static T FindBestCandidate<T>(this IEnumerable<T> candidates, Func<T, float> valuator, Func<T, bool> filter = null) {
			//Search entire map for water and go to it
			T bestCandidate = default(T);
			float value, bestValue = float.MaxValue;

			foreach (T candidate in candidates) {
				if (filter != null && !filter(candidate))
					continue;

				value = valuator(candidate);
				if (value < bestValue) {
					bestValue = value;
					bestCandidate = candidate;
				}
			}

			return bestCandidate;
		}

		/// <summary>
		/// Returns a random candidate from a list.
		/// </summary>
		/// <typeparam name="T">The type of the candidates in the list</typeparam>
		/// <param name="candidates">The list of candidates</param>
		/// <returns>A random candidate</returns>
		public static T GetRandom<T>(this IList<T> candidates) {
			return candidates[GetRandomIndex(candidates)];
		}

		/// <summary>
		/// Returns a random index within the range of a list.
		/// </summary>
		/// <typeparam name="T">The type of the candidates in the list</typeparam>
		/// <param name="candidates">The list of candidates</param>
		/// <returns>A random index between 0 (inclusive) and list length (exclusive)</returns>
		public static int GetRandomIndex<T>(this IList<T> candidates) {
			return UnityEngine.Random.Range(0, candidates.Count);
		}

		/// <summary>
		/// Randomizes the order of a list.
		/// </summary>
		/// <typeparam name="T">The type of the values in the list</typeparam>
		/// <param name="list">The list to shuffle</param>
		public static void Shuffle<T>(this IList<T> list) {
			for (int i = list.Count - 1; i > 0; --i) {
				int j = UnityEngine.Random.Range(0, i + 1);
				T value = list[i];
				list[i] = list[j];
				list[j] = value;
			}
		}

		/// <summary>
		/// Generates a list of sequential integers.
		/// </summary>
		/// <param name="count">The number of integers to generate</param>
		/// <param name="startIndex">The first integer to generate</param>
		/// <returns>A list of integers of the specified length</returns>
		public static List<int> GetIndexList(int count, int startIndex = 0) {
			List<int> result = new List<int>(count);
			for (int i = 0; i < count; ++i) {
				result.Add(startIndex + i);
			}
			return result;
		}
	}
}
