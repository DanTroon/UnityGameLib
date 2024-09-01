using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityGameLib.Attributes;

namespace UnityGameLib.UI {
	/// <summary>
	/// Displays a real-time feed of Debug log output.
	/// </summary>
	public class DebugLogCtrl : MonoBehaviour {
		[SerializeField, Required, RuntimeLocked] protected Text _outputField;
		[SerializeField, Required, RuntimeLocked] protected Text _versionField;
		[SerializeField, RuntimeLocked] protected int _messageLimit = 50;
		[SerializeField, RuntimeLocked] protected LogType _minLogLevel = LogType.Log;

		[Header("Message Colors")]
		[SerializeField, RuntimeLocked] protected Color32 _logColor = new Color(1f, 1f, 1f);
		[SerializeField, RuntimeLocked] protected Color32 _warningColor = new Color(1f, 1f, 0f);
		[SerializeField, RuntimeLocked] protected Color32 _errorColor = new Color(1f, .375f, 0f);
		[SerializeField, RuntimeLocked] protected Color32 _assertColor = new Color(1f, 0f, 0f);
		[SerializeField, RuntimeLocked] protected Color32 _exceptionColor = new Color(1f, 0f, 0f);
		
		protected List<string> _messages;
		protected string _colorStrLog;
		protected string _colorStrWarning;
		protected string _colorStrError;
		protected string _colorStrAssert;
		protected string _colorStrException;

		protected int _minLogPriority = 0;

		/// <summary>
		/// The minimum LogType severity to display (LogType.Log by default).
		/// </summary>
		public LogType minLogLevel {
			get { return _minLogLevel; }
			set {
				_minLogLevel = value;
				_minLogPriority = GetTypePriority(value);
			}
		}

		void Awake() {
			_messages = new List<string>(_messageLimit);

			_minLogPriority = GetTypePriority(_minLogLevel);

			_colorStrLog = _logColor.r.ToString("X2") + _logColor.g.ToString("X2") + _logColor.b.ToString("X2");
			_colorStrWarning = _warningColor.r.ToString("X2") + _warningColor.g.ToString("X2") + _warningColor.b.ToString("X2");
			_colorStrError = _errorColor.r.ToString("X2") + _errorColor.g.ToString("X2") + _errorColor.b.ToString("X2");
			_colorStrAssert = _assertColor.r.ToString("X2") + _assertColor.g.ToString("X2") + _assertColor.b.ToString("X2");
			_colorStrException = _exceptionColor.r.ToString("X2") + _exceptionColor.g.ToString("X2") + _exceptionColor.b.ToString("X2");
			
			_versionField.text = string.Format("Version {2}\n{0} {1}", SystemInfo.operatingSystem, SystemInfo.deviceModel, Application.version);
		}

		void OnEnable() {
			_outputField.text = "_";
			Application.logMessageReceived += Application_OnLogMessageReceived;
		}

		void OnDisable() {
			Application.logMessageReceived -= Application_OnLogMessageReceived;
		}

		/// <summary>
		/// Logs a message to the display.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="type">The severity of the message</param>
		/// <param name="logAllLevels">Whether to force this message even if <paramref name="type"/> is below <see cref="minLogLevel"/></param>
		/// <param name="stackTrace">The stack trace for the message, only displayed for exceptions and assertions</param>
		public void LogMessage(string message, LogType type = LogType.Log, bool logAllLevels = false, string stackTrace = "") {
			if (!logAllLevels && GetTypePriority(type) < _minLogPriority)
				return;

			string outputMessage = "";

			if (!string.IsNullOrEmpty(stackTrace) && (type == LogType.Exception || type == LogType.Assert)) {
				outputMessage = string.Format("<color=\"#{0}\"><b>[{1}]</b> {2}\n{3}</color>", GetColorHex(type), type.ToString().ToUpper(), message, stackTrace);
			} else {
				outputMessage = string.Format("<color=\"#{0}\"><b>[{1}]</b> {2}</color>", GetColorHex(type), type.ToString().ToUpper(), message);
			}

			DoLogMessage(outputMessage);
		}

		private string GetColorHex(LogType type) {
			switch (type) {
				case LogType.Log: return _colorStrLog;
				case LogType.Warning: return _colorStrWarning;
				case LogType.Error: return _colorStrError;
				case LogType.Assert: return _colorStrAssert;
				case LogType.Exception: return _colorStrException;
				default: return _colorStrLog;
			}
		}

		private int GetTypePriority(LogType type) {
			switch (type) {
				case LogType.Log: return 0;
				case LogType.Warning: return 1;
				case LogType.Error: return 2;
				case LogType.Assert: return 3;
				case LogType.Exception: return 4;
				default: return 0;
			}
		}

		private LogType GetTypeAt(int priority) {
			switch (priority) {
				case 0: return LogType.Log;
				case 1: return LogType.Warning;
				case 2: return LogType.Error;
				case 3: return LogType.Assert;
				case 4: return LogType.Exception;
				default: return LogType.Log;
			}
		}

		private void DoLogMessage(string message) {
			if (_messages.Count >= _messageLimit) {
				_messages.RemoveRange(_messageLimit - 1, _messages.Count - _messageLimit + 1);
			}

			_messages.Insert(0, message);

			RenderMessages();
		}

		private void RenderMessages() {
			_outputField.text = "_\n" + string.Join("\n", _messages.ToArray());
		}

		private void Application_OnLogMessageReceived(string condition, string stackTrace, LogType type) {
			LogMessage(condition, type, false, stackTrace);
		}
	}
}
