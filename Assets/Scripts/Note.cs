using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

		public float speed;

    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

		void Start() {
				rb.velocity = new Vector3(0, 0, -speed);
		}

    // Update is called once per frame
    void Update() {

    }
}
