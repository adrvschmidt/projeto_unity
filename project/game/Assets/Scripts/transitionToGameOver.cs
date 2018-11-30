using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamekit2D {
	public class transitionToGameOver : MonoBehaviour {

		void onCollisionEnter(Collider collider) {
			StartCoroutine(GameOverCoroutine());
		}

		IEnumerator GameOverCoroutine()
		{
			yield return new WaitForSeconds(1.0f); //wait one second before respawing
			yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.GameOver));
		}
	}
}

