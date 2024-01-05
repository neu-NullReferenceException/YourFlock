using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogButton : MonoBehaviour
{
    public DialogeElement myDialogFollowup;
    public DialogOptionModifyer modifyer;
    private bool flip = false;

    public void NextDialog()
    {
        if (flip) { return; }
        flip = true;
        //Debug.Log("NextDialog:" + myDialogFollowup);
        GameObject.Find("MANAGER").GetComponent<GameManager>().NextDialog(myDialogFollowup,this.gameObject, modifyer);
    }

    public void ResetButton()
    {
        flip = false;
        myDialogFollowup = null;
    }
}
