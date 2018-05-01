using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;	

namespace Models 
{
	using UnityEngine.UI;
	
    public class GameController : MonoBehaviour 
	{
		public float levelStartDelay = 2f;
		public static GameController instance = null;
		public GameObject bluePlayer;
		public GameObject redPlayer;
		public GameObject gameMap;
		public GameObject cursor;
		public GameObject neutralCursor;
        public GameObject blueSource;
        public GameObject redSource;
		public int levelSize = 7;
		public bool teamTurn = false;
		public Text endText;

		private int setupState = 0;
		private GameObject currentBluePlayer;
		private GameObject currentRedPlayer;
		private GameObject currentCursor;
		private GameObject currentNeutralCursor;
        private GameObject currentBlueSource;
        private GameObject currentRedSource;
            
		public bool selected = false;
		private int selectX = 0;
		private int selectY = 0;
		private int tick = 0;

        void Awake()
		{
			if(instance == null)
                instance = this;
            
            else if (instance != this)
                Destroy(gameObject);

			DontDestroyOnLoad (gameObject);

			endText.enabled = false;
			gameMap.GetComponent<BoardMap> ().buildMap (levelSize, levelSize);
		}

		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			instance.InitGame();
		}

		void InitGame()
		{
			setupState = 0;
		}

	    // Update is called once per frame
	    void Update () 
	    {
			if (setupState == 0) {
				if (gameMap.GetComponent<BoardMap> ().getTotal () == 0) {
					setupState++;
					tick = 50;
				}
			} else if (setupState == 1) {
				if (tick == 0) {
					setupState++;
					newBluePlayer (3, 0);
					newRedPlayer (3, 6);
					tick = 50;
				} else {
					tick--;
				}
			} else if (setupState == 2) {
				if (tick == 0) {
					GameObject obj = gameMap.GetComponent<BoardMap> ().findAddWater (6, 0);
					if (obj != null) {
						obj = gameMap.GetComponent<BoardMap> ().findAddWater (0, 6);
						if (obj != null) {
							setupState++;
							tick = 50;
							//Place fountains
							float newX = 6 * BoardMap.xOffset + 0 * BoardMap.xOffset;
							float newY = 6 * BoardMap.yUpOffset - 0 * BoardMap.yDownOffset + 10;
							currentBlueSource = Instantiate (blueSource, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
							currentBlueSource.layer = 0 - 6 + 18;
							currentBlueSource.GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + (0 - 6).ToString ();
							currentBlueSource.GetComponent<SpriteRenderer> ().sortingOrder = 7;
							gameMap.GetComponent<BoardMap> ().getTile (6, 0).GetComponent<TileStack> ().setSource (true);

							newX = 0 * BoardMap.xOffset + 6 * BoardMap.xOffset;
							newY = 0 * BoardMap.yUpOffset - 6 * BoardMap.yDownOffset + 10;
							currentRedSource = Instantiate (redSource, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
							currentRedSource.layer = 6 - 0 + 18;
							currentRedSource.GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + (6 - 0).ToString ();
							currentRedSource.GetComponent<SpriteRenderer> ().sortingOrder = 7;
							gameMap.GetComponent<BoardMap> ().getTile (0, 6).GetComponent<TileStack> ().setSource (true);
						} else {
							gameMap.GetComponent<BoardMap> ().getTile (0, 6).GetComponent<TileStack> ().raise (false, false);
							tick = 10;
						}
					} else {
						gameMap.GetComponent<BoardMap> ().getTile (6, 0).GetComponent<TileStack> ().raise (false, false);
						tick = 10;
					}
				} else {
					tick--;
				}
			} else if (setupState == 3) {
				if (tick == 0) {
					GameObject waterTile = gameMap.GetComponent<BoardMap> ().findAddWater (6, 0);
					waterTile.GetComponent<TileStack> ().raise (true, false);

					waterTile = gameMap.GetComponent<BoardMap> ().findAddWater (0, 6);
					waterTile.GetComponent<TileStack> ().raise (true, true);

					gameMap.GetComponent<BoardMap> ().waterSearch = true;
					tick = 20;
					setupState++;
				} else {
					tick--;
				}
			} else if (setupState == 4) {
				if (gameMap.GetComponent<BoardMap> ().waterSearch == false) {
					setupState++;
				}
			} else if (setupState == 5) {
				if (tick == 0) {
					setupState++;
					newCursor (3, 3, false);
					tick = 50;
				} else {
					tick--;
				}
			}
	    }

		public void newCursor(int x, int y, bool t) {
			DestroyImmediate (currentCursor);
			float newX = x * BoardMap.xOffset + y * BoardMap.xOffset;
			float newY = x * BoardMap.yUpOffset - y * BoardMap.yDownOffset + 13f;
			currentCursor = Instantiate (cursor, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
			currentCursor.layer = y - x + 18;
			currentCursor.transform.GetChild (0).gameObject.layer = currentCursor.layer;
			currentCursor.transform.GetChild (0).GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + (y - x).ToString();
			currentCursor.transform.GetChild (0).GetComponent<SpriteRenderer> ().sortingOrder = 8;
			currentCursor.GetComponent<Cursor> ().initializeCursor (x, y, t);
			teamTurn = t;
		}
                
        public bool isPosSafe(int x, int y) {
			if(gameMap.GetComponent<BoardMap> ().hasPosition(x,y)){
                return true;
            }
            else{
                return false;
            }
    
        }

		public void endGame(bool team) {
			//TODO ends the game
			//Play a sound1
			//Display some text saying Blue/Red team wins
			if (team) {
				endText.text = "Blue Geomancer Wins!";
			} else {
				endText.text = "Red Geomancer Wins!";
			}
			endText.enabled = true;
			Destroy(currentCursor);
		}

		public void selectTile() {
			Cursor c = currentCursor.GetComponent<Cursor> ();
			if (!selected) {
				TileStack ts = gameMap.GetComponent<BoardMap> ().getTile (c.getX (), c.getY ()).GetComponent<TileStack> ();
				if (ts.getHeight() != 1 || ts.getPlayer() != null) {
					if (ts.getPlayer () == null || ts.getPlayer ().GetComponent<Player> ().getTeam () == c.getTeam ()) {
						selectX = c.getX ();
						selectY = c.getY ();
						selected = true;

						newNeutralCursor (c.getX (), c.getY (), c.getTeam ());
					}
				}
			} else {
				BoardMap bm = gameMap.GetComponent<BoardMap> ();
				Deselect ();
				if (bm.Move (bm.getTile (selectX, selectY), bm.getTile (c.getX (), c.getY ()))) {
					newCursor (c.getX(), c.getY(), !c.getTeam());
				}
			}
		}

		public void newNeutralCursor(int x, int y, bool t) {
			float newX = x * BoardMap.xOffset + y * BoardMap.xOffset;
			float newY = x * BoardMap.yUpOffset - y * BoardMap.yDownOffset + 13f;
			currentNeutralCursor = Instantiate (neutralCursor, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
			currentNeutralCursor.layer = y - x + 18;
			currentNeutralCursor.transform.GetChild (0).gameObject.layer = currentNeutralCursor.layer;
			currentNeutralCursor.transform.GetChild (0).GetComponent<SpriteRenderer> ().sortingLayerName = "sortrow" + (y - x).ToString();
			currentNeutralCursor.transform.GetChild (0).GetComponent<SpriteRenderer> ().sortingOrder = 8;
			currentNeutralCursor.GetComponent<NeutralCursor>().setColour(t);
		}

		public void Deselect() {
			Destroy (currentNeutralCursor);
			currentNeutralCursor = null;
			selected = false;
		}

		public void newBluePlayer(int x, int y) {
			DestroyImmediate (currentBluePlayer);
			float newX = x * BoardMap.xOffset + y * BoardMap.xOffset;
			float newY = x * BoardMap.yUpOffset - y * BoardMap.yDownOffset + 6;
			currentBluePlayer = Instantiate (bluePlayer, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
			currentBluePlayer.layer = y - x + 18;
			currentBluePlayer.GetComponent<Player> ().setInitials (x, y, false);
			gameMap.GetComponent<BoardMap> ().getTile (x, y).GetComponent<TileStack> ().setPlayer (currentBluePlayer);
		}

		public void newRedPlayer(int x, int y) {
			DestroyImmediate (currentRedPlayer);
			float newX = x * BoardMap.xOffset + y * BoardMap.xOffset;
			float newY = x * BoardMap.yUpOffset - y * BoardMap.yDownOffset + 10;
			currentRedPlayer = Instantiate (redPlayer, new Vector3 (newX, newY, 0f), Quaternion.identity) as GameObject;
			currentRedPlayer.layer = y - x + 18;
			currentRedPlayer.GetComponent<Player> ().setInitials (x, y, true);
			gameMap.GetComponent<BoardMap> ().getTile (x, y).GetComponent<TileStack> ().setPlayer (currentRedPlayer);
		}
    }
}

