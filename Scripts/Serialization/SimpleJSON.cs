﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A third-party library for JSON parsing and handling.
/// Provides JSONNode, JSONClass, and JSONArray.
/// </summary>
namespace UnityGameLib.Serialization.SimpleJSON {
	public enum JSONBinaryTag {
		Array = 1,
		Class = 2,
		Value = 3,
		IntValue = 4,
		DoubleValue = 5,
		BoolValue = 6,
		FloatValue = 7,
	}

	public abstract class JSONNode {
		#region common interface

		public virtual void Add(string aKey, JSONNode aItem) {
		}

		public virtual JSONNode this[int aIndex] { get { return null; } set { } }

		public virtual JSONNode this[string aKey] { get { return null; } set { } }

		public virtual string Value { get { return ""; } set { } }

		public virtual int Count { get { return 0; } }

		public virtual void Add(JSONNode aItem) {
			Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey) {
			return null;
		}

		public virtual JSONNode Remove(int aIndex) {
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode) {
			return aNode;
		}

		public virtual IEnumerable<JSONNode> Children {
			get {
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChildren {
			get {
				foreach (var C in Children)
					foreach (var D in C.DeepChildren)
						yield return D;
			}
		}

		public override string ToString() {
			return "JSONNode";
		}

		public virtual string ToString(string aPrefix) {
			return "JSONNode";
		}

		public abstract string ToJSON(int prefix);

		#endregion common interface

		#region typecasting properties

		public virtual JSONBinaryTag Tag { get; set; }

		public virtual int AsInt {
			get {
				int v = 0;
				if (int.TryParse(Value, out v))
					return v;
				return 0;
			}
			set {
				Value = value.ToString();
				Tag = JSONBinaryTag.IntValue;
			}
		}

		public virtual float AsFloat {
			get {
				float v = 0.0f;
				if (float.TryParse(Value, out v))
					return v;
				return 0.0f;
			}
			set {
				Value = value.ToString();
				Tag = JSONBinaryTag.FloatValue;
			}
		}

		public virtual double AsDouble {
			get {
				double v = 0.0;
				if (double.TryParse(Value, out v))
					return v;
				return 0.0;
			}
			set {
				Value = value.ToString();
				Tag = JSONBinaryTag.DoubleValue;

			}
		}

		public virtual bool AsBool {
			get {
				bool v = false;
				if (bool.TryParse(Value, out v))
					return v;
				return !string.IsNullOrEmpty(Value);
			}
			set {
				Value = (value) ? "true" : "false";
				Tag = JSONBinaryTag.BoolValue;

			}
		}

		public virtual JSONArray AsArray {
			get {
				return this as JSONArray;
			}
		}

		public virtual JSONClass AsObject {
			get {
				return this as JSONClass;
			}
		}


		#endregion typecasting properties

		#region operators

		public static implicit operator JSONNode(string s) {
			return new JSONData(s);
		}

		public static implicit operator string(JSONNode d) {
			return (d == null) ? null : d.Value;
		}

		public static bool operator ==(JSONNode a, object b) {
			if (b == null && a is JSONLazyCreator)
				return true;
			return System.Object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONNode a, object b) {
			return !(a == b);
		}

		public override bool Equals(object obj) {
			return System.Object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}


		#endregion operators

		internal static string Escape(string aText) {
			string result = "";
			foreach (char c in aText) {
				switch (c) {
					case '\\':
						result += "\\\\";
						break;
					case '\"':
						result += "\\\"";
						break;
					case '\n':
						result += "\\n";
						break;
					case '\r':
						result += "\\r";
						break;
					case '\t':
						result += "\\t";
						break;
					case '\b':
						result += "\\b";
						break;
					case '\f':
						result += "\\f";
						break;
					default:
						result += c;
						break;
				}
			}
			return result;
		}

		static JSONData Numberize(string token) {
			bool flag = false;
			int integer = 0;
			double real = 0;

			if (int.TryParse(token, out integer)) {
				return new JSONData(integer);
			}

			if (double.TryParse(token, out real)) {
				return new JSONData(real);
			}

			if (bool.TryParse(token, out flag)) {
				return new JSONData(flag);
			}

			throw new NotImplementedException(token);
		}

		static void AddElement(JSONNode ctx, string token, string tokenName, bool tokenIsString) {
			if (tokenIsString) {
				if (ctx is JSONArray)
					ctx.Add(token);
				else
					ctx.Add(tokenName, token); // assume dictionary/object
			}
			// Edited by Spencer Evans	4.26.16
			// SimpleJSON cannot handle null values, converting to "null" instead
			else if (token == null || token == "null")
			{
				ctx.Add(tokenName, "null");
			}
			// End Edit
			else {
				JSONData number = Numberize(token);
				if (ctx is JSONArray)
					ctx.Add(number);
				else
					ctx.Add(tokenName, number);

			}
		}

		public static JSONNode Parse(string aJSON) {
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode ctx = null;
			int i = 0;
			string Token = "";
			string TokenName = "";
			bool QuoteMode = false;
			bool TokenIsString = false;
			while (i < aJSON.Length) {
				switch (aJSON[i]) {
					case '{':
						if (QuoteMode) {
							Token += aJSON[i];
							break;
						}
						stack.Push(new JSONClass());
						if (ctx != null) {
							TokenName = TokenName.Trim();
							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (TokenName != "")
								ctx.Add(TokenName, stack.Peek());
						}
						TokenName = "";
						Token = "";
						ctx = stack.Peek();
						break;

					case '[':
						if (QuoteMode) {
							Token += aJSON[i];
							break;
						}

						stack.Push(new JSONArray());
						if (ctx != null) {
							TokenName = TokenName.Trim();

							if (ctx is JSONArray)
								ctx.Add(stack.Peek());
							else if (TokenName != "")
								ctx.Add(TokenName, stack.Peek());
						}
						TokenName = "";
						Token = "";
						ctx = stack.Peek();
						break;

					case '}':
					case ']':
						if (QuoteMode) {
							Token += aJSON[i];
							break;
						}
						if (stack.Count == 0)
							throw new Exception("JSON Parse: Too many closing brackets");

						stack.Pop();
						if (Token != "") {
							TokenName = TokenName.Trim();
							/*
							if (ctx is JSONArray)
								ctx.Add (Token);
							else if (TokenName != "")
								ctx.Add (TokenName, Token);
								*/
							AddElement(ctx, Token, TokenName, TokenIsString);
							TokenIsString = false;
						}
						TokenName = "";
						Token = "";
						if (stack.Count > 0)
							ctx = stack.Peek();
						break;

					case ':':
						if (QuoteMode) {
							Token += aJSON[i];
							break;
						}
						TokenName = Token;
						Token = "";
						TokenIsString = false;
						break;

					case '"':
						QuoteMode ^= true;
						TokenIsString = QuoteMode == true ? true : TokenIsString;
						break;

					case ',':
						if (QuoteMode) {
							Token += aJSON[i];
							break;
						}
						if (Token != "") {
							/*
							if (ctx is JSONArray) {
								ctx.Add (Token);
							} else if (TokenName != "") {
								ctx.Add (TokenName, Token);
							}
							*/
							AddElement(ctx, Token, TokenName, TokenIsString);
							TokenIsString = false;

						}
						TokenName = "";
						Token = "";
						TokenIsString = false;
						break;

					case '\r':
					case '\n':
						break;

					case ' ':
					case '\t':
						if (QuoteMode)
							Token += aJSON[i];
						break;

					case '\\':
						++i;
						if (QuoteMode) {
							char C = aJSON[i];
							switch (C) {
								case 't':
									Token += '\t';
									break;
								case 'r':
									Token += '\r';
									break;
								case 'n':
									Token += '\n';
									break;
								case 'b':
									Token += '\b';
									break;
								case 'f':
									Token += '\f';
									break;
								case 'u': {
										string s = aJSON.Substring(i + 1, 4);
										Token += (char) int.Parse(
											s,
											System.Globalization.NumberStyles.AllowHexSpecifier);
										i += 4;
										break;
									}
								default:
									Token += C;
									break;
							}
						}
						break;

					default:
						Token += aJSON[i];
						break;
				}
				++i;
			}
			if (QuoteMode) {
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return ctx;
		}

		public virtual void Serialize(System.IO.BinaryWriter aWriter) {
		}

		public void SaveToStream(System.IO.Stream aData) {
			var W = new System.IO.BinaryWriter(aData);
			Serialize(W);
		}

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}
 
		public void SaveToCompressedFile(string aFileName)
		{
 
#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
 
#else
			throw new Exception("Can't use File IO stuff in webplayer");
#endif
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
 
#else
		public void SaveToCompressedStream(System.IO.Stream aData) {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName) {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64() {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
#endif

		public void SaveToFile(string aFileName) {
#if USE_FileIO
			System.IO.Directory.CreateDirectory ((new System.IO.FileInfo (aFileName)).Directory.FullName);
			using (var F = System.IO.File.OpenWrite (aFileName)) {
				SaveToStream (F);
			}
#else
			throw new Exception("Can't use File IO stuff in webplayer");
#endif
		}

		public string SaveToBase64() {
			using (var stream = new System.IO.MemoryStream()) {
				SaveToStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

		public static JSONNode Deserialize(System.IO.BinaryReader aReader) {
			JSONBinaryTag type = (JSONBinaryTag) aReader.ReadByte();
			switch (type) {
				case JSONBinaryTag.Array: {
						int count = aReader.ReadInt32();
						JSONArray tmp = new JSONArray();
						for (int i = 0; i < count; i++)
							tmp.Add(Deserialize(aReader));
						return tmp;
					}
				case JSONBinaryTag.Class: {
						int count = aReader.ReadInt32();
						JSONClass tmp = new JSONClass();
						for (int i = 0; i < count; i++) {
							string key = aReader.ReadString();
							var val = Deserialize(aReader);
							tmp.Add(key, val);
						}
						return tmp;
					}
				case JSONBinaryTag.Value: {
						return new JSONData(aReader.ReadString());
					}
				case JSONBinaryTag.IntValue: {
						return new JSONData(aReader.ReadInt32());
					}
				case JSONBinaryTag.DoubleValue: {
						return new JSONData(aReader.ReadDouble());
					}
				case JSONBinaryTag.BoolValue: {
						return new JSONData(aReader.ReadBoolean());
					}
				case JSONBinaryTag.FloatValue: {
						return new JSONData(aReader.ReadSingle());
					}

				default: {
						throw new Exception("Error deserializing JSON. Unknown tag: " + type);
					}
			}
		}

#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
#if USE_FileIO
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
#else
			throw new Exception("Can't use File IO stuff in webplayer");
#endif
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
		public static JSONNode LoadFromCompressedFile(string aFileName) {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData) {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(string aBase64) {
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
#endif

		public static JSONNode LoadFromStream(System.IO.Stream aData) {
			using (var R = new System.IO.BinaryReader(aData)) {
				return Deserialize(R);
			}
		}

		public static JSONNode LoadFromFile(string aFileName) {
#if USE_FileIO
			using (var F = System.IO.File.OpenRead (aFileName)) {
				return LoadFromStream (F);
			}
#else
			throw new Exception("Can't use File IO stuff in webplayer");
#endif
		}

		public static JSONNode LoadFromBase64(string aBase64) {
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}
	}
	// End of JSONNode

	public class JSONArray : JSONNode, IEnumerable {
		private List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNode this[int aIndex] {
			get {
				if (aIndex < 0 || aIndex >= m_List.Count)
					return new JSONLazyCreator(this);
				return m_List[aIndex];
			}
			set {
				if (aIndex < 0 || aIndex >= m_List.Count)
					m_List.Add(value);
				else
					m_List[aIndex] = value;
			}
		}

		public override JSONNode this[string aKey] {
			get { return new JSONLazyCreator(this); }
			set { m_List.Add(value); }
		}

		public override int Count {
			get { return m_List.Count; }
		}

		public override void Add(string aKey, JSONNode aItem) {
			m_List.Add(aItem);
		}

		public override JSONNode Remove(int aIndex) {
			if (aIndex < 0 || aIndex >= m_List.Count)
				return null;
			JSONNode tmp = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return tmp;
		}

		public override JSONNode Remove(JSONNode aNode) {
			m_List.Remove(aNode);
			return aNode;
		}

		public override IEnumerable<JSONNode> Children {
			get {
				foreach (JSONNode N in m_List)
					yield return N;
			}
		}

		public IEnumerator GetEnumerator() {
			foreach (JSONNode N in m_List)
				yield return N;
		}

		public override string ToString() {
			string result = "[ ";
			foreach (JSONNode N in m_List) {
				if (result.Length > 2)
					result += ", ";
				result += N.ToString();
			}
			result += " ]";
			return result;
		}

		public override string ToString(string aPrefix) {
			string result = "[ ";
			foreach (JSONNode N in m_List) {
				if (result.Length > 3)
					result += ", ";
				result += "\n" + aPrefix + "   ";
				result += N.ToString(aPrefix + "   ");
			}
			result += "\n" + aPrefix + "]";
			return result;
		}

		public override string ToJSON(int prefix) {
			string s = new string(' ', (prefix + 1) * 2);
			string ret = "[ ";
			foreach (JSONNode n in m_List) {
				if (ret.Length > 3)
					ret += ", ";
				ret += "\n" + s;
				ret += n.ToJSON(prefix + 1);

			}
			ret += "\n" + s + "]";
			return ret;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter) {
			aWriter.Write((byte) JSONBinaryTag.Array);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++) {
				m_List[i].Serialize(aWriter);
			}
		}
	}
	// End of JSONArray

	public class JSONClass : JSONNode, IEnumerable, IEnumerable<KeyValuePair<string, JSONNode>> {
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

		public override JSONNode this[string aKey] {
			get {
				if (m_Dict.ContainsKey(aKey))
					return m_Dict[aKey];
				else
					return new JSONLazyCreator(this, aKey);
			}
			set {
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = value;
				else
					m_Dict.Add(aKey, value);
			}
		}

		public override JSONNode this[int aIndex] {
			get {
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return null;
				return m_Dict.ElementAt(aIndex).Value;
			}
			set {
				if (aIndex < 0 || aIndex >= m_Dict.Count)
					return;
				string key = m_Dict.ElementAt(aIndex).Key;
				m_Dict[key] = value;
			}
		}

		public override int Count {
			get { return m_Dict.Count; }
		}


		public override void Add(string aKey, JSONNode aItem) {
			if (!string.IsNullOrEmpty(aKey)) {
				if (m_Dict.ContainsKey(aKey))
					m_Dict[aKey] = aItem;
				else
					m_Dict.Add(aKey, aItem);
			} else
				m_Dict.Add(Guid.NewGuid().ToString(), aItem);
		}

		public override JSONNode Remove(string aKey) {
			if (!m_Dict.ContainsKey(aKey))
				return null;
			JSONNode tmp = m_Dict[aKey];
			m_Dict.Remove(aKey);
			return tmp;
		}

		public override JSONNode Remove(int aIndex) {
			if (aIndex < 0 || aIndex >= m_Dict.Count)
				return null;
			var item = m_Dict.ElementAt(aIndex);
			m_Dict.Remove(item.Key);
			return item.Value;
		}

		public override JSONNode Remove(JSONNode aNode) {
			try {
				var item = m_Dict.Where(k => k.Value == aNode).First();
				m_Dict.Remove(item.Key);
				return aNode;
			} catch {
				return null;
			}
		}

		public override IEnumerable<JSONNode> Children {
			get {
				foreach (KeyValuePair<string, JSONNode> N in m_Dict)
					yield return N.Value;
			}
		}
		
		public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator() {
			return m_Dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			foreach (KeyValuePair<string, JSONNode> N in m_Dict)
				yield return N;
		}

		public override string ToString() {
			string result = "{";
			foreach (KeyValuePair<string, JSONNode> N in m_Dict) {
				if (result.Length > 2)
					result += ", ";
				result += "\"" + Escape(N.Key) + "\":" + N.Value.ToString();
			}
			result += "}";
			return result;
		}

		public override string ToString(string aPrefix) {
			string result = "{ ";
			foreach (KeyValuePair<string, JSONNode> N in m_Dict) {
				if (result.Length > 3)
					result += ", ";
				result += "\n" + aPrefix + "   ";
				result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix + "   ");
			}
			result += "\n" + aPrefix + "}";
			return result;
		}

		public override string ToJSON(int prefix) {
			string s = new string(' ', (prefix + 1) * 2);
			string ret = "{ ";
			foreach (KeyValuePair<string, JSONNode> n in m_Dict) {
				if (ret.Length > 3)
					ret += ", ";
				ret += "\n" + s;
				ret += string.Format("\"{0}\": {1}", n.Key, n.Value.ToJSON(prefix + 1));
			}
			ret += "\n" + s + "}";
			return ret;
		}

		public override void Serialize(System.IO.BinaryWriter aWriter) {
			aWriter.Write((byte) JSONBinaryTag.Class);
			aWriter.Write(m_Dict.Count);
			foreach (string K in m_Dict.Keys) {
				aWriter.Write(K);
				m_Dict[K].Serialize(aWriter);
			}
		}
	}
	// End of JSONClass

	public class JSONData : JSONNode {
		private string m_Data;


		public override string Value {
			get { return m_Data; }
			set {
				m_Data = value;
				Tag = JSONBinaryTag.Value;
			}
		}

		public JSONData(string aData) {
			m_Data = aData;
			Tag = JSONBinaryTag.Value;
		}

		public JSONData(float aData) {
			AsFloat = aData;
		}

		public JSONData(double aData) {
			AsDouble = aData;
		}

		public JSONData(bool aData) {
			AsBool = aData;
		}

		public JSONData(int aData) {
			AsInt = aData;
		}

		public override string ToString() {
			return "\"" + Escape(m_Data) + "\"";
		}

		public override string ToString(string aPrefix) {
			return "\"" + Escape(m_Data) + "\"";
		}

		public override string ToJSON(int prefix) {
			switch (Tag) {
				case JSONBinaryTag.DoubleValue:
				case JSONBinaryTag.FloatValue:
				case JSONBinaryTag.IntValue:
					return m_Data;
				case JSONBinaryTag.Value:
					return string.Format("\"{0}\"", Escape(m_Data));
				default:
					throw new NotSupportedException("This shouldn't be here: " + Tag.ToString());
			}
		}

		public override void Serialize(System.IO.BinaryWriter aWriter) {
			var tmp = new JSONData("");

			tmp.AsInt = AsInt;
			if (tmp.m_Data == this.m_Data) {
				aWriter.Write((byte) JSONBinaryTag.IntValue);
				aWriter.Write(AsInt);
				return;
			}
			tmp.AsFloat = AsFloat;
			if (tmp.m_Data == this.m_Data) {
				aWriter.Write((byte) JSONBinaryTag.FloatValue);
				aWriter.Write(AsFloat);
				return;
			}
			tmp.AsDouble = AsDouble;
			if (tmp.m_Data == this.m_Data) {
				aWriter.Write((byte) JSONBinaryTag.DoubleValue);
				aWriter.Write(AsDouble);
				return;
			}

			tmp.AsBool = AsBool;
			if (tmp.m_Data == this.m_Data) {
				aWriter.Write((byte) JSONBinaryTag.BoolValue);
				aWriter.Write(AsBool);
				return;
			}
			aWriter.Write((byte) JSONBinaryTag.Value);
			aWriter.Write(m_Data);
		}
	}
	// End of JSONData

	internal class JSONLazyCreator : JSONNode {
		private JSONNode m_Node = null;
		private string m_Key = null;

		public JSONLazyCreator(JSONNode aNode) {
			m_Node = aNode;
			m_Key = null;
		}

		public JSONLazyCreator(JSONNode aNode, string aKey) {
			m_Node = aNode;
			m_Key = aKey;
		}

		private void Set(JSONNode aVal) {
			if (m_Key == null) {
				m_Node.Add(aVal);
			} else {
				m_Node.Add(m_Key, aVal);
			}
			m_Node = null; // Be GC friendly.
		}

		public override JSONNode this[int aIndex] {
			get {
				return new JSONLazyCreator(this);
			}
			set {
				var tmp = new JSONArray();
				tmp.Add(value);
				Set(tmp);
			}
		}

		public override JSONNode this[string aKey] {
			get {
				return new JSONLazyCreator(this, aKey);
			}
			set {
				var tmp = new JSONClass();
				tmp.Add(aKey, value);
				Set(tmp);
			}
		}

		public override void Add(JSONNode aItem) {
			var tmp = new JSONArray();
			tmp.Add(aItem);
			Set(tmp);
		}

		public override void Add(string aKey, JSONNode aItem) {
			var tmp = new JSONClass();
			tmp.Add(aKey, aItem);
			Set(tmp);
		}

		public static bool operator ==(JSONLazyCreator a, object b) {
			if (b == null)
				return true;
			return System.Object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONLazyCreator a, object b) {
			return !(a == b);
		}

		public override bool Equals(object obj) {
			if (obj == null)
				return true;
			return System.Object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return "";
		}

		public override string ToString(string aPrefix) {
			return "";
		}

		public override string ToJSON(int prefix) {
			return "";
		}

		public override int AsInt {
			get {
				JSONData tmp = new JSONData(0);
				Set(tmp);
				return 0;
			}
			set {
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}

		public override float AsFloat {
			get {
				JSONData tmp = new JSONData(0.0f);
				Set(tmp);
				return 0.0f;
			}
			set {
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}

		public override double AsDouble {
			get {
				JSONData tmp = new JSONData(0.0);
				Set(tmp);
				return 0.0;
			}
			set {
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}

		public override bool AsBool {
			get {
				JSONData tmp = new JSONData(false);
				Set(tmp);
				return false;
			}
			set {
				JSONData tmp = new JSONData(value);
				Set(tmp);
			}
		}

		public override JSONArray AsArray {
			get {
				JSONArray tmp = new JSONArray();
				Set(tmp);
				return tmp;
			}
		}

		public override JSONClass AsObject {
			get {
				JSONClass tmp = new JSONClass();
				Set(tmp);
				return tmp;
			}
		}
	}
	// End of JSONLazyCreator

	public static class JSON {
		public static JSONNode Parse(string aJSON) {
			return JSONNode.Parse(aJSON);
		}
	}
}