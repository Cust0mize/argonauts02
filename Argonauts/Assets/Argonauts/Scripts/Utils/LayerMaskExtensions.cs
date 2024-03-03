using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions {
	public static bool IsInLayerMask(this LayerMask layermask, int layer) {
		return layermask == (layermask | (1 << layer));
	}
}
