using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResourceManager : LocalSingletonBehaviour<GameResourceManager> {
	public int UnblockedCharacters = 0;
    public int AdditionalCharacters = 0;

	public BaseCharacter Jason;
	public BaseCharacter Medea;
	public List<BaseCharacter> Characters = new List<BaseCharacter>();
	public List<Resource> Resources = new List<Resource>();
	public List<BaseCharacter> BlockedCharacters = new List<BaseCharacter>();

	public void TakeResources(List<Resource> resources) {
		int counter = 0;

		foreach (Resource o in resources) {
			counter = o.Count;

			for (int i = 0; i < Resources.Count; i++) {
				if (counter > 0) {
					if (Resources[i].Type.Equals(o.Type)) {
						if (counter > Resources[i].Count) {
							counter -= Resources[i].Count;
							Resources.RemoveAt(i);
						} else if (counter < Resources[i].Count) {
							Resources[i].Count -= counter;
							counter = 0;
							break;
						} else {
							counter = 0;
							Resources.RemoveAt(i);
							break;
						}
					}
				} else {
					continue;
				}
			}
		}
	}

	public void AddResources(List<Resource> resources) {

		bool isFound = false;

		foreach (Resource o in resources) {
			isFound = false;

			for (int i = 0; i < Resources.Count; i++) {
				if (Resources [i].Type.Equals (o.Type)) {
					Resources [i].Count += o.Count;
					isFound = true;
				}
			}

			if (!isFound) {
				Resources.Add (o);
			}
		}
	}

	public bool RequiredResourcesExist(List<Resource> resources) {
        int countFood = GetCountResource(Resource.Types.Food, Resources),
        countWood = GetCountResource(Resource.Types.Wood, Resources),
        countStone = GetCountResource(Resource.Types.Stone, Resources),
        countGold = GetCountResource(Resource.Types.Gold, Resources),
        countJason = GetCountResource(Resource.Types.Jaison, Resources),
        countMedea = GetCountResource(Resource.Types.Medea, Resources),
        countWorkers = GameManager.I.CampObject.CountCharacters,
        countGems = GetCountResource(Resource.Types.Gem, Resources);

        int requiredFood = GetCountResource(Resource.Types.Food, resources),
        requiredWood = GetCountResource(Resource.Types.Wood, resources),
        requiredStone = GetCountResource(Resource.Types.Stone, resources),
        requiredGold = GetCountResource(Resource.Types.Gold, resources),
        requiredJason = GetCountResource(Resource.Types.Jaison, resources),
        requiredMedea = GetCountResource(Resource.Types.Medea, resources),
        requiredWorkers = GetCountResource(Resource.Types.Worker, resources),
        requriedGems = GetCountResource(Resource.Types.Gem, resources);

        return
            countFood >= requiredFood &&
        countWood >= requiredWood &&
        countStone >= requiredStone &&
        countGold >= requiredGold &&
        countJason >= requiredJason &&
        countMedea >= requiredMedea &&
        countWorkers >= requiredWorkers &&
        countGems >= requriedGems;
	}

	public bool RequiredResourcesExistMaxCharacters(List<Resource> resources) {
		int countFood = GetCountResource(Resource.Types.Food, Resources),
		countWood = GetCountResource(Resource.Types.Wood, Resources),
		countStone = GetCountResource(Resource.Types.Stone, Resources),
		countGold = GetCountResource(Resource.Types.Gold, Resources),
		countJason = Jason != null ? 1 : 0,
		countMedea = Medea != null ? 1 : 0,
		countWorkers = GameManager.I.CampObject.MaxCharacters,
        countGems = GetCountResource(Resource.Types.Gem, Resources);

        int requiredFood = GetCountResource(Resource.Types.Food, resources),
		requiredWood = GetCountResource(Resource.Types.Wood, resources),
		requiredStone = GetCountResource(Resource.Types.Stone, resources),
		requiredGold = GetCountResource(Resource.Types.Gold, resources),
		requiredJason = GetCountResource(Resource.Types.Jaison, resources),
		requiredMedea = GetCountResource(Resource.Types.Medea, resources),
		requiredWorkers = GetCountResource(Resource.Types.Worker, resources),
        requriedGems = GetCountResource(Resource.Types.Gem, resources);

        return
			countFood >= requiredFood &&
		countWood >= requiredWood &&
		countStone >= requiredStone &&
		countGold >= requiredGold &&
		countJason >= requiredJason &&
		countMedea >= requiredMedea &&
		countWorkers >= requiredWorkers &&
        countGems >= requriedGems;
	}

    public bool RequiredCharactersExits(List<Resource> resources) {
        int countJason = Jason != null ? 1 : 0,
        countMedea = Medea != null ? 1 : 0,
        countWorkers = GameManager.I.CampObject.MaxCharacters;

        int requiredJason = GetCountResource(Resource.Types.Jaison, resources),
        requiredMedea = GetCountResource(Resource.Types.Medea, resources),
        requiredWorkers = GetCountResource(Resource.Types.Worker, resources);

        return countJason >= requiredJason &&
        countMedea >= requiredMedea &&
        countWorkers >= requiredWorkers;
    }

    public bool RequiredResourceExistMaxCharacters(Resource resource) {
        switch (resource.Type) {
            case Resource.Types.Worker:
                return GameManager.I.CampObject.MaxCharacters >= resource.Count;
            case Resource.Types.Jaison:
                return (Jason != null ? 1 : 0) >= resource.Count;
            case Resource.Types.Medea:
                return (Medea != null ? 1 : 0) >= resource.Count;
            default:
                return GetCountResource(resource.Type, Resources) >= resource.Count;
        }
    }

	public int GetCountResource(Resource.Types type, List<Resource> resources) {
		int count = 0;
		foreach (Resource o in resources) {
			if (o.Type.Equals(type))
				count += o.Count;
		}
		return count;
	}

	public void SetResources(Resource.Types type, int count) {
		Resource r = Resources.Find(x => x.Type == type);
		if (r != null)
			r.Count = count;
		else
			Resources.Add(new Resource(type, count));

        GameManager.I.DoResourcesChanged();
	}

    public List<Resource> GetDeltaResource (List<Resource> source, List<Resource> dest) {
        List<Resource> result = new List<Resource>();
        for (int i = 0; i < source.Count; i++) {
            result.Add(new Resource(source[i].Type, Mathf.Abs(source[i].Count - GetCountResource(source[i].Type, dest))));
        }
        return result;
    }

    public List<Resource> ChangeSign(List<Resource> source){
        List<Resource> result = new List<Resource>();
        foreach(Resource r in source){
            result.Add(new Resource(r.Type, -r.Count));
        }
        return result;
    }
}
