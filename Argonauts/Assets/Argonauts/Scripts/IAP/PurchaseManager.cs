using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseManager : GlobalSingletonBehaviour<PurchaseManager>, IStoreListener
{
#if UNITY_ANDROID
	public const string ALL_LEVELS_IAP = "com.eightfloor.argonauts02.freemium.googleplay.alllevels";
#else
	public const string ALL_LEVELS_IAP = "com.8floor.argonauts02.alllevels";
#endif

	private IStoreController _controller;
	private IExtensionProvider _extensions;

	public delegate void PurchaseFinishedDelegate(string id, bool success);

	public event PurchaseFinishedDelegate OnPurchaseFinished;

	public override void DoAwake()
	{
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct(ALL_LEVELS_IAP, ProductType.NonConsumable);
		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		_controller = controller;
		_extensions = extensions;

		CheckBoughtIAP();

		Debug.Log("PurchaseManager OnInitialized");
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log(error.ToString());
	}

	public void OnInitializeFailed(InitializationFailureReason error, string? message)
	{
		Debug.Log(error.ToString());
	}

	private bool CheckBoughtIAP()
	{
		Product product = _controller?.products.WithID(ALL_LEVELS_IAP);
		if (product != null && product.hasReceipt)
		{
			InternalPurchase(product.definition.id);
			return true;
		}

		return false;
	}

	public bool RestorePurchases()
	{
		bool result = false;

		_extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
		{
			if (result)
			{
				result = CheckBoughtIAP();

				// This does not mean anything was restored, merely that the restoration process succeeded.
				// ProcessPurchase should be called for each non-consumable
			}
			else
			{
				// Restoration failed.
				result = false;
			}
		});

		return result;
	}

	public void Purchase(string id)
	{
		_controller.InitiatePurchase(id);
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.Log("Purchase Failed: " + i.definition.id + "  Reason: " + p);

		OnPurchaseFinished?.Invoke(i.definition.id, false);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		InternalPurchase(e.purchasedProduct.definition.id);
		return PurchaseProcessingResult.Complete;
	}

	private void InternalPurchase(string id)
	{
		if (id == ALL_LEVELS_IAP)
		{
			UserData.I.UnlockAllLevels();
		}
		OnPurchaseFinished?.Invoke(id, true);
	}

	public string GetLocalizedPrice(string id)
	{
		if (_controller != null)
		{
			foreach (Product product in _controller.products.all)
			{
				if (product.definition.id.Equals(id))
				{
					return product.metadata.localizedPriceString;
				}
			}
		}

		return "";
	}

	public float GetPrice(string id)
	{
		if (_controller != null)
		{
			foreach (Product product in _controller.products.all)
			{
				if (product.definition.id.Equals(id))
				{
					return (float) product.metadata.localizedPrice;
				}
			}
		}

		return 0f;
	}
}
