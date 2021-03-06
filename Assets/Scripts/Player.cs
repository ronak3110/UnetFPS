﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	[SyncVar]
	private bool _isDead = false;

	public bool isDead
	{
		get {return _isDead;}

		protected set {_isDead = value;}
	}

	

	[SerializeField]
	private int maxHealth = 100;


	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	public void Setup()
	{
		wasEnabled = new bool[disableOnDeath.Length];

		for (int i = 0; i < wasEnabled.Length; i++) 
		{
			wasEnabled [i] = disableOnDeath [i].enabled;
		}

		setDefaults ();
	}

	[ClientRpc]
	public void RpcTakeDamage(int amount)
	{
		if (isDead)
			return;
		
		currentHealth -= amount;

		Debug.Log (transform.name + " now has " + currentHealth + " Health");
		if (currentHealth <= 0) 
		{
			Die ();

		}
	}

//	void Update()
//	{
//
//		if (!isLocalPlayer)
//			return;
//
//		if (Input.GetKeyDown (KeyCode.K))
//			RpcTakeDamage (9999); 
//
//	}
	private void Die()
	{
		isDead = true;	

		for (int i = 0; i < disableOnDeath.Length; i++) 
		{
			disableOnDeath [i].enabled = false;
		}
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}
		Debug.Log (transform.name + " Is DEAD");

		StartCoroutine (Respawn ());

	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);

		setDefaults ();
		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;
		Debug.Log (transform.name+  " Respawned");
	}

	public void setDefaults()
	{
		isDead = false;
		currentHealth = maxHealth;
		for (int i = 0; i < disableOnDeath.Length; i++) 
		{			
			disableOnDeath [i].enabled = wasEnabled [i];
		}
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}
	}

}