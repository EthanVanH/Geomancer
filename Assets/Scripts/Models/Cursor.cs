using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models{
    public class Cursor : MonoBehaviour {
        
		public Sprite blue;
		public Sprite red;

        private int posX;
        private int posY;
		private bool team;
		private bool moved = false;
		private static int tick = 0;
        public AudioClip moveSound;
                
		public void initializeCursor(int x, int y, bool t) {
			posX = x;
			posY = y;
			team = t;
			setColour ();
		}

		// Update is called once per frame
		public void Update () {
			if (!GameController.instance.gameMap.GetComponent<BoardMap> ().waterSearch) {
				if (tick == 0) {
					moved = false;
					int x;
					int y;

					x = (int)Input.GetAxisRaw ("Horizontal");
					y = (int)Input.GetAxisRaw ("Vertical") * -1;

					if (Input.GetKeyDown ("space")) {
						GameController.instance.selectTile ();
					}

					if (x != 0 || y != 0) {
						moved = true;
					}
					
					if (moved && GameController.instance.isPosSafe (posX + x, posY + y)) {
						posX += x;
						posY += y;
						tick = 10;
						moveCursor ();
					}
				} else {
					tick--;
				}
			}
		}
        
		public int getX() {
			return posX;
		}

		public int getY() {
			return posY;
		}

		public bool getTeam () {
			return team;
		}

		private void moveCursor () {
            SoundManager.instance.PlaySingle(moveSound);
			GameController.instance.newCursor (posX, posY, team);
		}

        public void getPosition(out int X, out int Y) {
            X = posX;
            Y = posY;
        }

		public void setColour() {
			if (team) {
				gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = red;
			} else {
				gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = blue;
			}
		}

		void OnCollisionEnter2D(Collision2D collision) {
			gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger ("CursorCollision");
		}
    }
}
