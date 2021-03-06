﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCharacterController {
	GameObject firstCharacter;
	public FirstCharacterController()
	{
		firstCharacter = GameObject.Instantiate(Resources.Load("Prefabs/GUN", typeof(GameObject))) as GameObject;
	}

	public GameObject getObject()
	{
		return firstCharacter;
	}

	public Vector3 getPosition()
	{
		return firstCharacter.transform.position;
	}
}
