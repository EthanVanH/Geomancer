using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Models
{

	public class BoardMap : MonoBehaviour 
	{
		private int[] positions; 
	
		public TileStack[][] tiles;
	
		//TODO Classes which inherit MonoBehaviour CANNOT have constructors.
		public BoardMap()
		{
			positions = new int[7] { 2, 3, 7, 1, 4, 6, 5 };;
			// for (int x = 0; x < 6; x++)
			// {
			// 	for (int y = 0; y < 6; y++)
			// 	{
			// 		Tile[x][y] = new Tile(5 + Random.range (-2, 2), positions[y]);
			// 	}
			// }
			// foreach (var row in tiles)
			// {
			// 	foreach (var tile in row)
			// 	{
			// 		tile = new Tile(5 + Random.range (-2, 2));
			// 	}
			// }
		}

		public void Raise(TileStack tile)
		{
			tile.Raise();
		}
	
		public void Lower(TileStack tile)
	 	{
			tile.Lower();
		}

		public void WaterFlow()
		{

		}

		public int CompareHeight(TileStack src, TileStack dest)
		{
			return src.GetHeight() - dest.GetHeight();
		}

		public int CompareDistance(TileStack src, TileStack dest)
		{
			return 1;
		} 

		public bool Move(TileStack src, TileStack dest)
		{
			//is source player if yes check if dest is legal Move
			//if yes player.src = false, dest.player = true
			//
			//if source or dest is a Source of water (source) return false
			//
			if(src.GetSource() ==true ||dest.GetSource() == true){
                return false;
            }
            if(CompareDistance(src,dest)>1){
                return false;
            }
			else if (src.GetPlayer() !=null){
				if (dest.GetWater() ==true){
                    return false;
                }
                else{
					dest.SetPlayer(src.GetPlayer());
					src.SetPlayer(null);
                    //animate
                }
            }
			else {
                src.Lower();
                dest.Raise();
			}
			

			return true;
		}
        public bool hasPosition(int x, int y){
            // check the bounds of the 2d map array
            if(x > tiles.GetLength(0) || y > tiles.GetLength(1)){
                return false;
            }
            return true;
        }
    
        public int countOpenAdj(BoardMap map, int x, int y){
                int sum;
                sum = 0;
                if(map.hasPosition(x,y+1)){
                    if(map.tiles[x][y+1].GetWater()==false){
                        sum++;
                    }
                }

                if(map.hasPosition(x+1,y+1)){
                    if(map.tiles[x+1][y+1].GetWater()==false){
                        sum++;
                    }
                }

                if(map.hasPosition(x-1,y)){
                    if(map.tiles[x-1][y].GetWater()==false){
                        sum++;
                    }
                }

                if(map.hasPosition(x+1,y)){
                    if(map.tiles[x+1][y].GetWater()==false){
                        sum++;
                    }
                }

            if(map.hasPosition(x-1,y-1)){
                if(map.tiles[x-1][y-1].GetWater()==false){
                    sum++;
                }
            }

            if(map.hasPosition(x,y-1)){
                if(map.tiles[x][y-1].GetWater()==false){
                    sum++;
                }
            }

        return sum; 
        }	
    }	
}
