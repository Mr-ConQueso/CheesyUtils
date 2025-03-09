using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CheesyUtils.Editor.BuildTools
{
	[InitializeOnLoad]
	public class CompileTimeTrackerWindow : EditorWindow
	{
		// PRAGMA MARK - Static
		static CompileTimeTrackerWindow()
		{
			CompileTimeTracker.KeyframeAdded += LogCompileTimeKeyframe;
		}

		[MenuItem("Tools/Build Tools/Compile Time Tracker")]
		public static void Open()
		{
			GetWindow<CompileTimeTrackerWindow>(false, "Compile Timer Tracker", true);
		}

		private static void LogCompileTimeKeyframe(CompileTimeKeyframe keyframe)
		{
			bool dontLogToConsole = !LogToConsole;
			if (dontLogToConsole)
			{
				return;
			}

			string compilationFinishedLog = "Compilation Finished: " + TrackingUtil.FormatMSTime(keyframe.ElapsedCompileTimeInMS);
			if (keyframe.HadErrors)
			{
				compilationFinishedLog += " (error)";
			}
			Debug.Log(compilationFinishedLog);
		}

		private static bool ShowErrors
		{
			get => EditorPrefs.GetBool("CompileTimeTrackerWindow.ShowErrors");
			set => EditorPrefs.SetBool("CompileTimeTrackerWindow.ShowErrors", value);
		}

		private static bool OnlyToday
		{
			get => EditorPrefs.GetBool("CompileTimeTrackerWindow.OnlyToday");
			set => EditorPrefs.SetBool("CompileTimeTrackerWindow.OnlyToday", value);
		}

		private static bool OnlyYesterday
		{
			get => EditorPrefs.GetBool("CompileTimeTrackerWindow.OnlyYesterday");
			set => EditorPrefs.SetBool("CompileTimeTrackerWindow.OnlyYesterday", value);
		}

		private static bool LogToConsole
		{
			get => EditorPrefs.GetBool("CompileTimeTrackerWindow.LogToConsole", defaultValue: true);
			set => EditorPrefs.SetBool("CompileTimeTrackerWindow.LogToConsole", value);
		}

		// PRAGMA MARK - Internal
		private Vector2 _scrollPosition;

		private void OnGUI()
		{
			Rect screenRect = this.position;
			int totalCompileTimeInMS = 0;

			// show filters
			EditorGUILayout.BeginHorizontal(GUILayout.Height(20.0f));
			EditorGUILayout.Space();
			float toggleRectWidth = screenRect.width / 4.0f;
			Rect toggleRect = new Rect(0.0f, 0.0f, width: toggleRectWidth, height: 20.0f);

			// Psuedo enum logic here
			if (OnlyToday && OnlyYesterday)
			{
				OnlyYesterday = false;
			}

			if (!OnlyToday && !OnlyYesterday)
			{
				OnlyToday = true;
			}

			bool newOnlyToday = GUI.Toggle(toggleRect, OnlyToday, "Today", (GUIStyle)"Button");
			if (newOnlyToday != OnlyToday)
			{
				OnlyToday = newOnlyToday;
				OnlyYesterday = !newOnlyToday;
			}

			toggleRect.position = toggleRect.position.AddX(toggleRectWidth);
			bool newOnlyYesterday = GUI.Toggle(toggleRect, OnlyYesterday, "Yesterday", (GUIStyle)"Button");
			if (newOnlyYesterday != OnlyYesterday)
			{
				OnlyYesterday = newOnlyYesterday;
				OnlyToday = !newOnlyYesterday;
			}
			// End psuedo enum logic

			toggleRect.position = toggleRect.position.AddX(2.0f * toggleRectWidth);
			ShowErrors = GUI.Toggle(toggleRect, ShowErrors, "Errors", (GUIStyle)"Button");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal(GUILayout.Height(20.0f));
			LogToConsole = EditorGUILayout.Toggle("Log Compile Time", LogToConsole);
			EditorGUILayout.EndHorizontal();

			this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition, GUILayout.Height(screenRect.height - 64.0f));
			foreach (CompileTimeKeyframe keyframe in this.GetFilteredKeyframes())
			{
				string compileText = string.Format("({0:hh:mm tt}): ", keyframe.Date);
				compileText += TrackingUtil.FormatMSTime(keyframe.ElapsedCompileTimeInMS);
				if (keyframe.HadErrors)
				{
					compileText += " (error)";
				}
				GUILayout.Label(compileText);

				totalCompileTimeInMS += keyframe.ElapsedCompileTimeInMS;
			}
			EditorGUILayout.EndScrollView();

			string statusBarText = "Total compile time: " + TrackingUtil.FormatMSTime(totalCompileTimeInMS);
			if (EditorApplication.isCompiling)
			{
				statusBarText = "Compiling.. || " + statusBarText;
			}

			EditorGUILayout.BeginHorizontal(GUILayout.Height(24.0f));
			GUILayout.Label(statusBarText);
			if (GUILayout.Button("Export CSV", GUILayout.ExpandWidth(false)))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("All"), false, ExportAllCSV);
				menu.AddItem(new GUIContent("Filtered"), false, ExportFilteredCSV);
				menu.ShowAsContext();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void OnEnable()
		{
			EditorApplicationCompilationUtil.StartedCompiling += this.HandleEditorStartedCompiling;
			CompileTimeTracker.KeyframeAdded += this.HandleCompileTimeKeyframeAdded;
		}

		private void OnDisable()
		{
			EditorApplicationCompilationUtil.StartedCompiling -= this.HandleEditorStartedCompiling;
			CompileTimeTracker.KeyframeAdded -= this.HandleCompileTimeKeyframeAdded;
		}

		private IEnumerable<CompileTimeKeyframe> GetFilteredKeyframes()
		{
			IEnumerable<CompileTimeKeyframe> filteredKeyframes = CompileTimeTracker.GetCompileTimeHistory();
			if (!ShowErrors)
			{
				filteredKeyframes = filteredKeyframes.Where(keyframe => !keyframe.HadErrors);
			}

			if (OnlyToday)
			{
				filteredKeyframes = filteredKeyframes.Where(keyframe => DateTimeUtil.SameDay(keyframe.Date, DateTime.Now));
			}
			else if (OnlyYesterday)
			{
				filteredKeyframes = filteredKeyframes.Where(keyframe => DateTimeUtil.SameDay(keyframe.Date, DateTime.Now.AddDays(-1)));
			}

			return filteredKeyframes;
		}
		
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private void ExportAllCSV()
		{
			IEnumerable<CompileTimeKeyframe> allKeyframes = CompileTimeTracker.GetCompileTimeHistory();
			ExportCSV(allKeyframes, "all_compile_times");
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private void ExportFilteredCSV()
		{
			IEnumerable<CompileTimeKeyframe> filteredKeyframes = GetFilteredKeyframes();
			ExportCSV(filteredKeyframes, "filtered_compile_times");
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		private void ExportCSV(IEnumerable<CompileTimeKeyframe> keyframes, string fileName)
		{
			var path = EditorUtility.SaveFilePanel("Export compile times to CSV", "", string.Format("{0}.csv", fileName), "csv");
			var csv = CompileTimeKeyframe.ToCSV(keyframes as List<CompileTimeKeyframe>);
			File.WriteAllText(path, csv);
		}

		private void HandleEditorStartedCompiling()
		{
			this.Repaint();
		}

		private void HandleCompileTimeKeyframeAdded(CompileTimeKeyframe keyframe)
		{
			this.Repaint();
		}
	}
}