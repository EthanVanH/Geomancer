
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
	public class TileStack : MonoBehaviour
    {
        public const int MAXHEIGHT = 6;
        public const int MINHEIGHT = 1;
        //image Tile
		public GameObject[] tilePrefabs;

		public const float yOffSet = 0.51208827f;

		public List<GameObject> tiles;
        private int height = 0;
        private int tileRow;
        private bool water;
		private bool waterTeam;
		public GameObject player;
        private bool source;
		private bool raiseFlag;
		private int tick;
    
		public void initializeStack(int row, bool src){
            tileRow = row;
            water = false;
			raiseFlag = false;
			tick = 0;
            player = null;
			source = src;
			tiles = new List<GameObject> ();
			gameObject.layer = row + 18;
			gameObject.transform.GetChild (0).gameObject.layer = row + 18;
        }

		void Update() {
			if (raiseFlag) {
				if (tick == 0) {
					raiseFlag = false;
					GameObject toInstantiate = tilePrefabs [Random.Range (0, 4)];
					GameObject instance = Instantiate (toInstantiate, gameObject.transform.position + new Vector3 (0f, yOffSet, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (gameObject.transform);
					instance.layer = gameObject.layer;
					instance.GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + tileRow.ToString ();
					instance.GetComponent<SpriteRenderer> ().sortingOrder = 0;
					tiles.Insert (0, instance);

					for (int i = 0; i < tiles.Count; i++)
						tiles [i].GetComponent<SpriteRenderer> ().sortingOrder += 1;
					
					GameController.instance.gameMap.GetComponent<BoardMap> ().waterSearch = true;
				} else {
					tick--;
				}
			} else {
				if (tick == 0) {
					tiles.Sort (delegate (GameObject obj1, GameObject obj2) {
						return obj1.transform.position.y.CompareTo (obj2.transform.position.y);
					});

					for(int i = 0; i < tiles.Count; i++)
						tiles [i].GetComponent<SpriteRenderer> ().sortingOrder = i;

					tick = Random.Range(35,56);
				} else {
					tick--;
				}
			}
		}

        public int getTileRow()
        {
            return tileRow;
        }

        public int getHeight()
        {
            return height;
        }

		public bool getWater()
		{
			return water;
		}

		public bool getWaterTeam()
		{
			return waterTeam;
		}

		public bool getSource()
		{
			return source;
		}

		public GameObject getPlayer()
		{
			return player;
		}

		public void setPlayer(GameObject newPlayer)
		{
			player = newPlayer;
		}
        
		public void setSource(bool set)
		{
            source = set;
        }

		public void raise(bool putWater, bool team)
        {
			if (water == false) {
				if (height < MAXHEIGHT || putWater == true) {
					int min = 0;
					if (putWater) {
						water = true;
						waterTeam = team;
						min = 4;
						if (team)
							min *= 2;
					} else {
						height++;
					}

					GameObject toInstantiate = tilePrefabs [Random.Range (min, min + 4)];
					GameObject instance = Instantiate (toInstantiate, gameObject.transform.position + new Vector3 (0f, 3f + height * yOffSet * 2, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (gameObject.transform);
					instance.layer = gameObject.layer;
					instance.GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + tileRow.ToString ();
					instance.GetComponent<SpriteRenderer> ().sortingOrder = height;
					tiles.Add (instance);
					GameController.instance.gameMap.GetComponent<BoardMap> ().waterSearch = true;
				}
			} else {
				height++;
				tiles [0].GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, 400f + 400f * height));
				tick = 15;
				raiseFlag = true;
			}
        }

        public void lower()
        {
			if (height > MINHEIGHT && !source)
            {
				GameObject deadTile = tiles [0];
				tiles.RemoveAt (0);
				Destroy (deadTile);
				height--;
			}
        }
    }
}
