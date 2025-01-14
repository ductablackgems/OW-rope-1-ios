using System.Collections;
using UnityEngine;

namespace com.ootii.Utilities.Debug
{
	public class Log : MonoBehaviour
	{
		public bool _PrefixTime = true;

		public bool _IsConsoleEnabled = true;

		public bool _IsScreenEnabled = true;

		public int _LineCount = 30;

		public int _ScreenFontSize = 12;

		public Color _ScreenForeColor = Color.black;

		public bool _ClearScreenEachFrame = true;

		public bool _IsFileEnabled;

		public string _FilePath = ".\\Log.txt";

		public bool _FileFlushPerWrite;

		private static string mFilePath;

		private static bool mPrefixTime;

		private static int mLineHeight;

		private static bool mClearScreenEachFrame;

		private static bool mIsEnabled;

		private static bool mIsFileEnabled;

		private static bool mIsScreenEnabled;

		private static bool mIsConsoleEnabled;

		private static bool mFileFlushPerWrite;

		private static int mLineCount;

		private static int mFontSize;

		private static Color mForeColor;

		private static LogText[] mLines;

		private static int mLineIndex;

		private static Rect mLineRect;

		public static string FilePath
		{
			get
			{
				return mFilePath;
			}
			set
			{
				mFilePath = value;
			}
		}

		public static bool PrefixTime
		{
			get
			{
				return mPrefixTime;
			}
			set
			{
				mPrefixTime = value;
			}
		}

		public static int LineHeight
		{
			get
			{
				return mLineHeight;
			}
			set
			{
				mLineHeight = value;
			}
		}

		public static bool ClearScreenEachFrame
		{
			get
			{
				return mClearScreenEachFrame;
			}
			set
			{
				mClearScreenEachFrame = value;
			}
		}

		public static bool IsEnabled
		{
			get
			{
				return mIsEnabled;
			}
			set
			{
				mIsEnabled = value;
			}
		}

		public static bool IsFileEnabled
		{
			get
			{
				return mIsFileEnabled;
			}
			set
			{
				mIsFileEnabled = value;
			}
		}

		public static bool IsScreenEnabled
		{
			get
			{
				return mIsScreenEnabled;
			}
			set
			{
				mIsScreenEnabled = value;
			}
		}

		public static bool IsConsoleEnabled
		{
			get
			{
				return mIsConsoleEnabled;
			}
			set
			{
				mIsConsoleEnabled = value;
			}
		}

		public static bool FileFlushPerWrite
		{
			get
			{
				return mFileFlushPerWrite;
			}
			set
			{
				mFileFlushPerWrite = value;
			}
		}

		public static int LineCount
		{
			get
			{
				return mLineCount;
			}
			set
			{
				if (mLineCount != value)
				{
					mLineCount = value;
					mLines = new LogText[mLineCount];
					for (int i = 0; i < mLines.Length; i++)
					{
						LogText logText = new LogText();
						logText.X = 10;
						logText.Y = i * mLineHeight;
						logText.Text = "";
						mLines[i] = logText;
					}
				}
			}
		}

		public static int FontSize
		{
			get
			{
				return mFontSize;
			}
			set
			{
				mFontSize = value;
			}
		}

		public static Color ForeColor
		{
			get
			{
				return mForeColor;
			}
			set
			{
				mForeColor = value;
			}
		}

		public IEnumerator Start()
		{
			FilePath = _FilePath;
			FontSize = _ScreenFontSize;
			ForeColor = _ScreenForeColor;
			LineCount = _LineCount;
			LineHeight = _ScreenFontSize + 6;
			ClearScreenEachFrame = _ClearScreenEachFrame;
			PrefixTime = _PrefixTime;
			IsFileEnabled = _IsFileEnabled;
			IsScreenEnabled = _IsScreenEnabled;
			IsConsoleEnabled = _IsConsoleEnabled;
			FileFlushPerWrite = _FileFlushPerWrite;
			WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();
			while (true)
			{
				yield return lWaitForEndOfFrame;
				if (mClearScreenEachFrame)
				{
					Clear();
				}
			}
		}

		public void OnDestroy()
		{
			Close();
		}

		private void OnGUI()
		{
			Render();
		}

		static Log()
		{
			mFilePath = ".\\Log.txt";
			mPrefixTime = true;
			mLineHeight = 18;
			mClearScreenEachFrame = true;
			mIsEnabled = true;
			mIsFileEnabled = false;
			mIsScreenEnabled = true;
			mIsConsoleEnabled = true;
			mFileFlushPerWrite = true;
			mLineCount = 30;
			mFontSize = 12;
			mForeColor = Color.black;
			mLines = null;
			mLineIndex = 0;
			mLineRect = default(Rect);
			if (mLines == null)
			{
				mLines = new LogText[mLineCount];
			}
			for (int i = 0; i < mLines.Length; i++)
			{
				LogText logText = new LogText
				{
					X = 10,
					Y = i * mLineHeight,
					Text = ""
				};
				mLines[i] = logText;
			}
		}

		public static void Write(string rText)
		{
			if (mIsEnabled)
			{
				if (mIsFileEnabled)
				{
					FileWrite(rText);
				}
				if (mIsScreenEnabled)
				{
					ScreenWrite(rText);
				}
				if (mIsConsoleEnabled)
				{
					ConsoleWrite(rText);
				}
			}
		}

		public static void FileScreenWrite(string rText, int rLine)
		{
			if (mIsEnabled && mIsScreenEnabled)
			{
				ScreenWrite(rText, rLine);
			}
		}

		public static void FileWrite(string rText)
		{
			if (mIsEnabled && mPrefixTime)
			{
				rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
			}
		}

		public static void FileWrite(string rText, bool rPrefixTime)
		{
			if (mIsEnabled && mPrefixTime && rPrefixTime)
			{
				rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
			}
		}

		public static void ConsoleScreenWrite(string rText)
		{
			if (mIsEnabled)
			{
				ConsoleWrite(rText);
				ScreenWrite(rText);
			}
		}

		public static void ConsoleScreenWrite(string rText, int rLine)
		{
			if (mIsEnabled)
			{
				ConsoleWrite(rText);
				ScreenWrite(rText, rLine);
			}
		}

		public static void ConsoleWrite(string rText)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				UnityEngine.Debug.Log(rText);
			}
		}

		public static void ConsoleWrite(string rText, bool rPrefixTime)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime && rPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				UnityEngine.Debug.Log(rText);
			}
		}

		public static void ConsoleWriteWarning(string rText)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				UnityEngine.Debug.LogWarning(rText);
			}
		}

		public static void ConsoleWriteError(string rText)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				UnityEngine.Debug.LogError(rText);
			}
		}

		public static void ScreenWrite(string rText)
		{
			if (mIsEnabled)
			{
				ScreenWrite(rText, 10, mLineIndex * mLineHeight);
			}
		}

		public static void ScreenWrite(int rLine, params string[] rText)
		{
			if (mIsEnabled)
			{
				ScreenWrite(string.Join(" ", rText), 10, rLine * mLineHeight);
			}
		}

		public static void ScreenWrite(string rText, int rLine)
		{
			if (mIsEnabled)
			{
				ScreenWrite(rText, 10, rLine * mLineHeight);
			}
		}

		public static void ScreenWrite(string rText, int rX, int rY)
		{
			if (!mIsEnabled)
			{
				return;
			}
			if (mPrefixTime)
			{
				rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
			}
			int num = rY / mLineHeight;
			if (num < mLines.Length)
			{
				if (mLines[num] == null)
				{
					LogText logText = new LogText();
					logText.X = rX;
					logText.Y = num * mLineHeight;
					logText.Text = rText;
					mLines[num] = logText;
				}
				else
				{
					mLines[num].Text = rText;
				}
			}
			mLineIndex++;
			if (mLineIndex >= mLineCount)
			{
				mLineIndex = mLineCount - 1;
			}
		}

		public static void ScreenWriteTop(string rText)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				for (int num = mLines.Length - 1; num > 0; num--)
				{
					mLines[num].Text = mLines[num - 1].Text;
				}
				mLines[0].Text = rText;
			}
		}

		public static void ScreenWriteBottom(string rText)
		{
			if (mIsEnabled)
			{
				if (mPrefixTime)
				{
					rText = $"[{Time.realtimeSinceStartup:f4}] {rText}";
				}
				for (int i = 0; i < mLines.Length - 1; i++)
				{
					mLines[i].Text = mLines[i + 1].Text;
				}
				mLines[mLines.Length - 1].Text = rText;
			}
		}

		public static void Render()
		{
			if (!mIsEnabled || mLines.Length == 0)
			{
				return;
			}
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.alignment = TextAnchor.UpperLeft;
			gUIStyle.normal.textColor = Color.white;
			gUIStyle.fontSize = mFontSize;
			GUI.contentColor = mForeColor;
			GUI.backgroundColor = Color.green;
			for (int i = 0; i < mLines.Length; i++)
			{
				LogText logText = mLines[i];
				if (logText.Text.Length != 0)
				{
					mLineRect.x = logText.X;
					mLineRect.y = logText.Y;
					mLineRect.width = 900f;
					mLineRect.height = mLineHeight;
					GUI.Label(mLineRect, logText.Text, gUIStyle);
				}
			}
		}

		public static void Clear()
		{
			for (int i = 0; i < mLines.Length; i++)
			{
				mLines[i].Text = "";
			}
			mLineIndex = 0;
		}

		public static void Close()
		{
		}
	}
}
