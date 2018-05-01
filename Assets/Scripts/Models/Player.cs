using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models{

	public class Player : MonoBehaviour 
	{
		public BoardMap gameMap;
        public const int startingMana = 5;
		private int mana { get; set; }
		private bool team; //false is blue team, true is red team

        private int posX;
        private int posY;
		private static int tick = 0;

		public void Start() {
			mana = startingMana;
		}

		public void setInitials(int x, int y, bool t) {
			posX = x;
			posY = y;
			int layer = y - x;
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "sortrow" + layer.ToString ();
			gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 7;
			team = t;
		}

        public void Update() {
			if (tick == 0) {
				if (AvailableMoves () == 1) {
					GameController.instance.endGame (team);
				}
				tick = 30;
			} else {
				tick--;
			}
        }

		public bool getTeam() {
			return team;
		}

    	public int AvailableMoves()
    	{
			return GameController.instance.gameMap.GetComponent<BoardMap>().countOpenAdj(posX,posY);
		}
	}

}
