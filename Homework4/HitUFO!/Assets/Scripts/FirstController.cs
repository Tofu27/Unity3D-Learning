﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstController : MonoBehaviour, ISceneController {
    GameDirector gameDirector;
    FirstSceneActionManager actionManager;
    public UFOFactory ufoFactory;
	ExplosionFactory explosionFactory;

    ScoreRecorder scoreRecorder;
    TimerController timerController;
    DifficultyController difficulty;

	int gameStatus;
	int round = 1;
	int emitNum;
	int scoreInRound;
	int fpsCount;
	bool running;
	GUIStyle headerStyle;
	GUIStyle buttonStyle;
	Text roundText;

    void Awake()
    {
        gameDirector = GameDirector.getInstance();
        gameDirector.currentSceneController = this;
        actionManager = gameObject.AddComponent<FirstSceneActionManager>();
        ufoFactory = gameObject.AddComponent<UFOFactory>();
		explosionFactory = gameObject.AddComponent<ExplosionFactory> ();
        difficulty = DifficultyController.getInstance();
        timerController = gameObject.AddComponent<TimerController>();
        scoreRecorder = ScoreRecorder.getInstance();

        loadResources();
    }

    public void Start()
    {
		gameStatus = 0;
		difficulty.setDifficulty(0);
		round = 0;
		roundText = (GameObject.Instantiate(Resources.Load("Prefabs/RoundInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
		roundText.text = "" + round;
		fpsCount = 0;
		headerStyle = new GUIStyle();
		headerStyle.fontSize = 40;
		headerStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;
		running = false;
    }

	void OnGUI() 
	{
		if (gameStatus == 0) 
		{
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "Hit the UFO!", headerStyle);
			if (GUI.Button(new Rect(Screen.width / 2 - 45, Screen.height / 2, 100, 50), "Play!",buttonStyle))
			{
				replay ();	
			}
				
		}
		else if (gameStatus == 2) 
		{
			GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 90, 100, 50), "You get "+ scoreRecorder.getScore()+" points in this game!", headerStyle);
			if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2, 200, 50), "Play Again",buttonStyle))
			{
				replay ();	
			}
		}
	}

    void Update()
    {
		if (running && gameStatus == 1)
		{
			fpsCount++;
			if (fpsCount == 60)
			{
				if (emitNum < difficulty.getEmitNum ()) {
					emitUFO ();
					fpsCount = 0;
					emitNum++;
				} else if (emitNum == difficulty.getEmitNum ()) {
					if (scoreInRound < difficulty.getSuccessScore ()) {
						gameStatus = 2;
					}
					else
					{
						Invoke("startRound", 3);
						timerController.setTime(3);
						difficulty.setDifficultyByScore (scoreRecorder.getScore ());
					}
				}
			}
		}
		else
		{
			if (Input.GetKeyDown ("enter") || Input.GetKeyDown ("return"))
				replay ();
		}
    }

    public void loadResources()
    {
        Instantiate(Resources.Load("Prefabs/Land"));
    }

    void startRound()
    {
		running = true;
		roundText.text = "" + round++;
		emitNum = 0;
		scoreInRound = 0;
		fpsCount = 0;
    }

	void emitUFO()
	{
		UFOController ufoCtrl = ufoFactory.GetUFO(difficulty.getUFOAttributes());
		ufoCtrl.appear ();
		actionManager.addActionToUFO(ufoCtrl.GetObject(), difficulty.getUFOAttributes().speed);
	}

    public void shootUFO(UFOController ufo)
    {
		scoreInRound++;
        scoreRecorder.record(difficulty.getDifficulty());
        actionManager.removeActionByObj(ufo.GetObject());
		explosionFactory.explode (ufo.GetObject ().transform.position);
        ufoFactory.recycle(ufo);
    }

	public void shootGround(Vector3 pos) 
	{
		explosionFactory.explode (pos);
	}

	public void replay() 
	{
		gameStatus = 1;
		ufoFactory.recycleAll();
		round = 0;
		roundText.text = "" + round++;
		Invoke("startRound", 3);
		timerController.setTime(3);
		difficulty.resetDifficulty ();
		scoreRecorder.reset ();
	}
}