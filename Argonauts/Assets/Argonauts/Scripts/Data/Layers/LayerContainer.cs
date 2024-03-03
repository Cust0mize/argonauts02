[System.Serializable]
public class LayerContainer {
	public BaseLayer[] Layers;

	public LayerContainer() {
		Layers = new BaseLayer[0];
	}

	public LayerContainer(BaseLayer[] layers) {
		Layers = layers;
	}
}
