using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models {
	public class NeutralCursor : MonoBehaviour {

		public Sprite blue;
		public Sprite red;

		public void setColour(bool team) {
			if(team) {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = red;
			} else {
				gameObject.transform.GetChild (0).gameObject.GetComponent<SpriteRenderer> ().sprite = blue;
			}
		}
	}
}
