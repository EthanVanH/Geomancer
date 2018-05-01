using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Models
{

	public class BoardMap : MonoBehaviour 
	{
		public GameObject stackPrefab;
		public GameObject[,] tiles;

		public static float xOffset = 3.553282f;
		public static float yUpOffset = 1.3701687f;
		public static float yDownOffset = 1.3830003f;

		private int[,] heights;
		private int total;
		private int xSize;
		private int ySize;
		public bool waterSearch;
		private int tick = 50;
		private int flowAttempt = 0;

		private GameObject lastMoveSrc;
		private GameObject lastMoveDest;

		// Map generation code
		public void buildMap(int x, int y)
		{
			xSize = x;
			ySize = y;
			waterSearch = false;

			tiles = new GameObject[x, y];
			for (int i = 0; i < x; i++) {
				for (int j = 0; j < y; j++) {
					float newX;
					float newY;

					if (i + j >= (x - 1) / 2 && i + j <= y - 1 + (y - 1) / 2) {
						newX = i * xOffset + j * xOffset;
						newY = i * yUpOffset - j * yDownOffset;
						tiles [i, j] = Instantiate (stackPrefab, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
						tiles [i, j].GetComponent<TileStack> ().initializeStack (j - i, false);
						tiles [i, j].transform.SetParent (gameObject.transform);
					} else {
						tiles [i, j] = null;
					}
				}
			}

			heights = new int[x, y];

			total = 0;
			for (int i = 0; i < x; i++) {
				for (int j = 0; j < y; j++) {
					if (tiles [i, j] != null) {
						heights [i, j] = Random.Range (1, 6);
						total += heights [i, j];
					} else {
						heights [i, j] = 0;
					}
				}
			}
		}

		public void Update() {
			if(total > 0) {
				int rngX = Random.Range (0, xSize);
				int rngY = Random.Range (0, ySize);

				if (heights [rngX, rngY] != 0) {
					if (tiles [rngX, rngY].GetComponent<TileStack> ().getHeight () < heights [rngX, rngY]) {
						raise (tiles [rngX, rngY], false);
						total--;
						//Wait a small amount of time
						//continue;
					}
				}
				//Wait a VERY small amount of time
			}

			if (waterSearch) {
				if (tick == 0) {
					waterSearch = WaterFlow ();
					if (flowAttempt > 30) {
						waterSearch = false;
						flowAttempt = 0;
					}
					tick = 50;
				} else {
					tick--;
				}
			}
		}

		public void raise(GameObject stack, bool team)
		{
			stack.GetComponent<TileStack> ().raise (false, team);
		}
	
		public void lower(GameObject stack)
	 	{
			stack.GetComponent<TileStack> ().lower ();
		}

		public bool WaterFlow()
		{
			bool foundFlag = false;
			List<GameObject> tilesToAdd = new List<GameObject> ();
			List<Boolean> tileTeam = new List<Boolean> ();
			for (int i = 0; i < tiles.GetLength (0); i++) {
				for (int j = 0; j < tiles.GetLength (1); j++) {
					if (tiles [i, j] != null && tiles [i, j].GetComponent<TileStack> ().getWater ()) {
						GameObject obj = findAddWater (i, j);
						if (obj != null) {
							tilesToAdd.Add (obj);
							tileTeam.Add (tiles [i, j].GetComponent<TileStack> ().getWaterTeam ());
							foundFlag = true;
						}
					}
				}
			}

			for (int i = 0; i < tilesToAdd.Count; i++) {
				if (!tilesToAdd [i].GetComponent<TileStack> ().getWater())
					tilesToAdd [i].GetComponent<TileStack> ().raise (true, tileTeam [i]);
			}

			flowAttempt++;
			return foundFlag;
		}

		public GameObject findAddWater(int x, int y) {
			List<GameObject> list = getAdjacent (x, y);
			GameObject lowestObj = null;
			for (int i = 0; i < list.Count; i++) {
				if (list [i].GetComponent<TileStack> ().getHeight () <= tiles [x, y].GetComponent<TileStack> ().getHeight ()) {
					if (list [i].GetComponent<TileStack> ().getWater ()) {
						if (list [i].GetComponent<TileStack> ().getWaterTeam () == tiles [x, y].GetComponent<TileStack> ().getWaterTeam ()) {
							return null;
						}
					} else if (lowestObj == null) {
						lowestObj = list [i];
					} else {
						if (list [i].GetComponent<TileStack> ().getHeight () < lowestObj.GetComponent<TileStack> ().getHeight ()) {
							lowestObj = list [i];
						} else if (list [i].GetComponent<TileStack> ().getHeight () == lowestObj.GetComponent<TileStack> ().getHeight ()) {
							int rng = Random.Range (0, 2);
							if (rng == 1) {
								lowestObj = list [i];
							}
						}
					}
				}
			}
			return lowestObj;
		}

		public int CompareHeight(GameObject src, GameObject dest)
		{
			return src.GetComponent<TileStack>().getHeight() - dest.GetComponent<TileStack>().getHeight();
		}

		public int CompareDistance(GameObject src, GameObject dest)
		{
			return 1;
		} 

		public GameObject getTile(int x, int y) {
			return tiles [x, y];
		}

		public bool Move(GameObject src, GameObject dest)
		{
			//is source player if yes check if dest is legal Move
			//if yes player.src = false, dest.player = true
			//
			//if source or dest is a Source of water (source) return false
			//
	        int x,y;
	        x = 0;
	        y = 0;
			TileStack srcStack = src.GetComponent<TileStack> ();
			TileStack destStack = dest.GetComponent<TileStack> ();

			if (src.Equals (lastMoveDest) && dest.Equals (lastMoveSrc))
				return false;

			if (srcStack.getSource () == true || destStack.getSource () == true) {
				return false;
			}

			if (CompareDistance (src, dest) > 1) {
				return false;
			}

			if (srcStack.getPlayer() != null) {
				if (destStack.getWater() == true){
					return false;
				}
				if(destStack.getPlayer() != null){
                    return false;
                }
				if (srcStack.getPlayer ().GetComponent<Player> ().getTeam () != GameController.instance.teamTurn) {
					return false;
				}

				if(srcStack.getPlayer().GetComponent<Player>().getTeam() == false){
                    findPos(dest, out x, out y);
                    GameController.instance.newBluePlayer(x,y);
                } else {
                    findPos(dest, out x, out y);
                    GameController.instance.newRedPlayer(x,y);
                }
				src.GetComponent<TileStack>().setPlayer(null);
            } else {
				if (src.Equals (dest))
					return false;
				if (srcStack.getHeight () == 1)
					return false;
				if (destStack.getHeight () == 6)
					return false;
				lower(src);
				raise(dest, GameController.instance.teamTurn);
			}
			

			lastMoveSrc = src;
			lastMoveDest = dest;
			return true;
		}

        public bool hasPosition(int x, int y) {
			// check the bounds of the 2d map array
			if (x < tiles.GetLength (0) && y < tiles.GetLength (1) && x >= 0 && y >= 0) {
				if (tiles [x, y] != null) {
					return true;
				}
			}
			return false;
		}

		public bool findPos(GameObject stack, out int x, out int y) {
			for (int i = 0; i < tiles.GetLength (0); i++) {
				for (int j = 0; j < tiles.GetLength (1); j++) {
					if (stack.Equals (tiles [i, j])) {
						x = i;
						y = j;
						return true;
					}
				}
			}
                        x = 0;
                        y = 0;
			return false;
		}

		public List<GameObject> getAdjacent(int x, int y) {
			List<GameObject> list = new List<GameObject> ();
			if (hasPosition (x, y + 1)) {
				if (tiles [x, y + 1].GetComponent<TileStack> ().getPlayer () == null) {
					if (tiles [x, y + 1].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x, y + 1]);
					}
				}
			}

			if (hasPosition (x - 1, y + 1)) {
				if (tiles [x - 1, y + 1].GetComponent<TileStack> ().getPlayer () == false) {
					if (tiles [x - 1, y + 1].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x - 1, y + 1]);
					}
				}
			}

			if (hasPosition (x - 1, y)) {
				if (tiles [x - 1, y].GetComponent<TileStack> ().getPlayer () == false) {
					if (tiles [x - 1, y].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x - 1, y]);
					}
				}
			}

			if (hasPosition (x + 1, y)) {
				if (tiles [x + 1, y].GetComponent<TileStack> ().getPlayer () == false) {
					if (tiles [x + 1, y].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x + 1, y]);
					}
				}
			}

			if (hasPosition (x + 1, y - 1)) {
				if (tiles [x + 1, y - 1].GetComponent<TileStack> ().getPlayer () == false) {
					if (tiles [x + 1, y - 1].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x + 1, y - 1]);
					}
				}
			}

			if (hasPosition (x, y - 1)) {
				if (tiles [x, y - 1].GetComponent<TileStack> ().getPlayer () == false) {
					if (tiles [x, y - 1].GetComponent<TileStack> ().getSource () == false) {
						list.Add (tiles [x, y - 1]);
					}
				}
			}

			return list;
		}

        public int countOpenAdj(int x, int y) {
			int sum;
			sum = 0;
			if (hasPosition (x, y + 1)) {
				if (tiles [x, y + 1].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x, y + 1].GetComponent<TileStack> ().getPlayer () == null) {
						if (tiles [x, y + 1].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			if (hasPosition (x + 1, y - 1)) {
				if (tiles [x + 1, y - 1].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x + 1, y - 1].GetComponent<TileStack> ().getPlayer () == false) {
						if (tiles [x + 1, y - 1].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			if (hasPosition (x - 1, y)) {
				if (tiles [x - 1, y].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x - 1, y].GetComponent<TileStack> ().getPlayer () == false) {
						if (tiles [x - 1, y].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			if (hasPosition (x + 1, y)) {
				if (tiles [x + 1, y].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x + 1, y].GetComponent<TileStack> ().getPlayer () == false) {
						if (tiles [x + 1, y].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			if (hasPosition (x - 1, y + 1)) {
				if (tiles [x - 1, y + 1].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x - 1, y + 1].GetComponent<TileStack> ().getPlayer () == false) {
						if (tiles [x - 1, y + 1].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			if (hasPosition (x, y - 1)) {
				if (tiles [x, y - 1].GetComponent<TileStack> ().getWater () == false) {
					if (tiles [x, y - 1].GetComponent<TileStack> ().getPlayer () == false) {
						if (tiles [x, y - 1].GetComponent<TileStack> ().getSource () == false) {
							sum++;
						}
					}
				}
			}

			return sum; 
		}

		public int getTotal() {
			return total;
		}
	}
}
