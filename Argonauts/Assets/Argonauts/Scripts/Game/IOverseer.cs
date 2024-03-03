using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOverseer {
    IEnumerator GetFreeCharacters (int countWorkers, Node target, List<Resource> needResources, bool jaison = false, bool medea = false);
}
