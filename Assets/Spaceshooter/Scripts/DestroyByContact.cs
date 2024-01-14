using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;

    private void OnTriggerEnter(Collider other) {
        if(other.tag != "Boundary"){
            Instantiate(explosion, transform.position, transform.rotation);
            if(other.TryGetComponent<IDamageable>(out var damageable))
            {
                Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
                damageable.TakeDamage(1);
                // Move over to player take damage
                //gameController.gameIsOver();
            }
            GameController.Instance.addScore(scoreValue);
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
