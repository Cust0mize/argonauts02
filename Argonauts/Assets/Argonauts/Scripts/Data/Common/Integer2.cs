using UnityEngine;

[System.Serializable]
public class Integer2 {
	public int X;
	public int Y;

	public Integer2() { }
	public Integer2(int x, int y) {
		X = x;
		Y = y;
	}

	public static Integer2 One { get { return new Integer2(1, 1); } }
	public static Integer2 Zero { get { return new Integer2(0, 0); } }
	public static Integer2 Right { get { return new Integer2(1, 0); } }
	public static Integer2 Up { get { return new Integer2(0, 1); } }

	public static Integer2 operator -(Integer2 a, Integer2 b) { return new Integer2(a.X - b.X, a.Y - b.Y); }
	public static Integer2 operator +(Integer2 a, Integer2 b) { return new Integer2(a.X + b.X, a.Y + b.Y); }
	public static Integer2 operator *(Integer2 a, Integer2 b) { return new Integer2(a.X * b.X, a.Y * b.Y); }
	public static Integer2 operator /(Integer2 a, Integer2 b) { return new Integer2(a.X / b.X, a.Y / b.Y); }

	public Vector2 GetVector2() {
		return new Vector2(X, Y);
	}

	public static Integer2 GetInteger2(Vector2 v) {
		return new Integer2((int)v.x, (int)v.y);
	}

	public static Integer2 GetInteger2(Vector3 v) {
		return new Integer2((int)v.x, (int)v.y);
	}

	public override string ToString() {
		return string.Format("({0} - {1})", X, Y);
	}
}
