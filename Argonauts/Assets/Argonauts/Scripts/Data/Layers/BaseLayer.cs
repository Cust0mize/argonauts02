[System.Serializable]
public abstract class BaseLayer {
	public enum LayerTypes { Base, Decor, Object, Path, Road, Tile }
	public LayerTypes LayerType = LayerTypes.Base;
	public Integer2 Position;
	public string Name;
}
