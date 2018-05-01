using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models {
	public class TileCollision : MonoBehaviour {

		public AudioClip spawnSound;
		public AudioClip hitSound;
		public AudioClip destroySound;

		// Use this for initialization
		void Start () {
			TileSoundManager.instance.PlaySingle(spawnSound);
		}
		
		void OnCollisionEnter2D(Collision2D collision) {
			TileSoundManager.instance.PlaySingle(hitSound);
		}

		void OnDestroy() {
			TileSoundManager.instance.PlaySingle(destroySound);
		}

		void OnMouseDown() {
			GameController.instance.gameMap.GetComponent<BoardMap> ().waterSearch = true;
		}
	}
}
