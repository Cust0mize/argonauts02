using Newtonsoft.Json;
using UnityEngine;

public abstract class BaseViewController : MonoBehaviour {
    protected object model;
    protected bool lockEdit = false;

    public virtual void StartInit() { }
    public virtual void Init(object model = null) { this.model = model; }
    public virtual void FromJson(string json) { }
    public virtual void OnUpdateViewController() { }
    public virtual object GetModel() {
        return model;
    }
    public virtual string ToJson() {
        string result = string.Empty;
        if(model != null) {
            result = JsonConvert.SerializeObject(model, ModuleContainer.I.ConfigController.GetJsonSettings());
        }
        return result;
    }

    protected int GetInt(string value) {
        int result = 0;
        if(string.IsNullOrEmpty(value)) return 0;
        int.TryParse(value, out result);
        return result;
    }
    protected float GetFloat(string value) {
        float result = 0;
        if(string.IsNullOrEmpty(value)) return 0;
        float.TryParse(value, out result);
        return result;
    }

    public virtual void Show() {
        gameObject.SetActive(true);
    }
    public virtual void Hide() {
        gameObject.SetActive(false);
    }
}
