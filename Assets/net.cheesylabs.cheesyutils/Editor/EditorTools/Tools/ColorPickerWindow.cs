using UnityEditor;
using UnityEngine;

namespace CheesyUtils.EditorTools
{
	public class ColorPickerWindow : EditorWindow
	{
		protected Color Color = Color.white;
		protected Color32 Color32 = new Color32(255, 255, 255, 255);
		protected string HexCode = "FFFFFF";

		[MenuItem("Tools/Color Picker")]
		public static void Init()
		{
			var window = GetWindow<ColorPickerWindow>("Color Picker");
			window.Show();
		}

		protected virtual void OnGUI()
		{
			this.Color = EditorGUILayout.ColorField("Color", this.Color);
			if (GUI.changed)
			{
				this.Color32 = this.Color;
				this.HexCode = ColorUtility.ToHtmlStringRGB(this.Color);
			}

			this.HexCode = EditorGUILayout.TextField("Hex Code", this.HexCode);
			if (GUI.changed)
			{
				ColorUtility.TryParseHtmlString(this.HexCode, out this.Color);
			}

			this.Color32.r = (byte)EditorGUILayout.IntSlider("Red", this.Color32.r, 0, 255);
			this.Color32.g = (byte)EditorGUILayout.IntSlider("Green", this.Color32.g, 0, 255);
			this.Color32.b = (byte)EditorGUILayout.IntSlider("Blue", this.Color32.b, 0, 255);
			this.Color32.a = (byte)EditorGUILayout.IntSlider("Alpha", this.Color32.a, 0, 255);
			if (GUI.changed)
			{
				this.Color = this.Color32;
				this.HexCode = ColorUtility.ToHtmlStringRGB(this.Color);
			}

			EditorGUILayout.TextField(
				"Color Code",
				string.Format(
					"new Color ( {0}f, {1}f, {2}f, {3}f )",
					this.Color.r,
					this.Color.g,
					this.Color.b,
					this.Color.a));
			EditorGUILayout.TextField(
				"Color32 Code",
				string.Format(
					"new Color32 ( {0}, {1}, {2}, {3} )",
					this.Color32.r,
					this.Color32.g,
					this.Color32.b,
					this.Color32.a));
		}
	}
}
