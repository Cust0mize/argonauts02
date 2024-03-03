using System;
using System.Collections.Generic;

[Serializable]
public class RandomBlock<T> {
    public List<T> TemplateItems = new List<T>();
    public List<T> Items = new List<T>();
    public int LevelNumber;

    public RandomBlock(List<T> templateItems) {
        this.TemplateItems = templateItems;
        this.Items = ObjectCopier.Clone(templateItems);
    }

    public RandomBlock() { }

    public T Next()
    {
        if (Items.Count <= 0) {
            if (TemplateItems.Count <= 0) {
                return default(T);
            } else {
                Fill();
            }
        }
        T t = Items[UnityEngine.Random.Range(0, Items.Count)];
        Items.Remove(t);
        return t;
    }

    public void Fill()
    {
        Items.Clear();
        Items = ObjectCopier.Clone(TemplateItems);
    }

    public void AddItem(T item, int proportion)
    {
        for (int i = 0; i < proportion; i++) {
            Items.Add(item);
        }
    }

    public void AddTemplateItem(T item, int proportion)
    {
        for (int i = 0; i < proportion; i++) {
            TemplateItems.Add(item);
        }
        AddItem(item, proportion);
    }

    public bool IsEmpty {
        get {
            return Items.Count <= 0;
        }
    }
}
