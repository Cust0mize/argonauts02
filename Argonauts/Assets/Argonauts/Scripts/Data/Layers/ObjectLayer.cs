[System.Serializable]
public class ObjectLayer : BaseLayer {
	public LayerObjectContainer LayerObjectContainer;

	public ObjectLayer(Integer2 position, string name, LayerObjectContainer layerObjectContainer) {
		LayerType = LayerTypes.Object;
		Position = position;
		Name = name;
		LayerObjectContainer = layerObjectContainer;
	}

	public ObjectLayer() {
		LayerType = LayerTypes.Object;
	}

	public ObjectLayer(Integer2 position, string name) {
		LayerType = LayerTypes.Object;
		Position = position;
		Name = name;
	}
}
