using UnityEngine;
using System.Collections;

public class msgAnim : MonoBehaviour {

    void Start()
    {
        //animation = GetComponent<Animation>();
    //    StartCoroutine(MyCoroutine());
    }
    void playAnim()
    {
        StartCoroutine(playAnimation());
    }
    private IEnumerator playAnimation()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
        yield return null;
    }
}
