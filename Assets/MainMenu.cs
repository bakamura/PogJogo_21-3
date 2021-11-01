﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public CanvasGroup mainCanvas;
    public CanvasGroup savesCanvas;
    public CanvasGroup configsCanvas;
    public CanvasGroup confirmQuitCanvas;

    private void Start() {
        DeactivateAllCanvas();
        ActivateCanvas(mainCanvas, true);
    }

    private void DeactivateAllCanvas() {
        ActivateCanvas(mainCanvas, false);
        ActivateCanvas(savesCanvas, false);
        ActivateCanvas(configsCanvas, false);
        ActivateCanvas(confirmQuitCanvas, false);
    }

    private void ActivateCanvas(CanvasGroup canvas,bool bol) {
        canvas.alpha = bol ? 1 : 0;
        canvas.interactable = bol;
        canvas.blocksRaycasts = bol;
    }

    //Buttons

    public void Return2Main() {
        DeactivateAllCanvas();
        ActivateCanvas(mainCanvas, true);
    }

    public void PlayButton() {
        DeactivateAllCanvas();
        ActivateCanvas(savesCanvas, true);
    }

    public void ConfigsButton() {
        DeactivateAllCanvas();
        ActivateCanvas(configsCanvas, true);
    }

    public void QuitButton() {
        ActivateCanvas(confirmQuitCanvas, true);
    }

    public void ConfirmQuitButton() {
        Application.Quit();
    }

}
