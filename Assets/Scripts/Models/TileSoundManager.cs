﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSoundManager : MonoBehaviour {
        public AudioSource efxSource; //Needs to be a refrence to the audio source tile move sound
        public static TileSoundManager instance = null;
        public float lowPitchRange = .95f;
        public float highPitchRange = 1.05f;

        void Awake (){
            if(instance == null){
                instance = this;
            }
            else if (instance != this){
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        
        public void PlaySingle(AudioClip clip){
            
            efxSource.clip = clip;

            efxSource.Play();
        }
 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
